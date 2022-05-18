using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Futurus.RemoteInput
{
    public class RemoteInputModule : BaseInputModule
    {
        static readonly Comparison<RaycastResult> RaycastCompare = RaycastComparer.Compare;

        #region Inspector
        [SerializeField]
        [Tooltip("Scales the EventSystem.pixelDragThreshold to make selection easier.")]
        float dragThresholdMultiplier = 1.4f;

        [SerializeField]
        [Tooltip("The maximum time (in seconds) between two mouse presses for it to be consecutive click.")]
        float _clickSpeed = 0.3f;
        #endregion

        #region Runtime
        readonly HashSet<IRemoteInputProvider> _inputProviderSet = new HashSet<IRemoteInputProvider>();
        readonly HashSet<RemoteInputEventData> _remoteEventDataSet = new HashSet<RemoteInputEventData>();
        #endregion

        #region Public
        public int ProviderCount => _inputProviderSet.Count;
        public RemoteInputEventData GetRemoteInputEventData(IRemoteInputProvider provider) => GetOrCreateRemoteEventData(provider);
        public bool Register(IRemoteInputProvider provider) => SetRegistration(provider, true);
        public bool Deregister(IRemoteInputProvider provider) => SetRegistration(provider, false);
        public bool SetRegistration(IRemoteInputProvider provider, bool state)
        {
            if (state)
                return _inputProviderSet.Add(provider);
            else if (_inputProviderSet.Remove(provider))
            {
                var data = GetOrCreateRemoteEventData(provider);
                data.Reset();
                ProcessRemote(data);
                return true;
            }
            return false;
        }
        public bool IsRegistered(IRemoteInputProvider provider) => _inputProviderSet.Contains(provider);
        public override void Process()
        {
            // Skip doing process if there are no registered raycasters
            if (RemoteInputRaycaster.AllRaycasters.Count == 0)
                return;
            foreach (var provider in _inputProviderSet)
                ProcessRemote(GetOrCreateRemoteEventData(provider));
        }
        #endregion

        #region Unity Methods
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);
        }
        #endregion

        #region Internal
        RemoteInputEventData GetOrCreateRemoteEventData(IRemoteInputProvider provider)
        {
            // 0n linear search, bad make better later
            foreach (var eventData in _remoteEventDataSet)
            {
                if (provider == eventData.Remote)
                    return eventData;
            }
            var newData = new RemoteInputEventData(provider, eventSystem);
            _remoteEventDataSet.Add(newData);
            return newData;
        }
        void RaycastAllRemoteRaycasters(RemoteInputEventData eventData, List<RaycastResult> raycastResults)
        {
            foreach (var raycaster in RemoteInputRaycaster.AllRaycasters)
            {
                if (!(raycaster == null) && raycaster.IsActive() && raycaster.CanRaycast)
                {
                    raycaster.Raycast(eventData, raycastResults);
                }
            }
            raycastResults.Sort(RaycastCompare);
        }
        // Process is called once per tick
        void ProcessRemote(RemoteInputEventData eventData)
        {
            // Raycasting
            eventData.position = new Vector2(float.MinValue, float.MinValue);
            RaycastAllRemoteRaycasters(eventData, m_RaycastResultCache);
            var result = FindFirstRaycast(m_RaycastResultCache);
            m_RaycastResultCache.Clear();
            eventData.pointerCurrentRaycast = result;

            // Process Result to EventData
            var camera = Camera.main == null ? eventData?.pointerCurrentRaycast.module?.eventCamera : Camera.main;
            if (camera != null)
            {
                Vector2 screenPosition = camera.WorldToScreenPoint(eventData.pointerCurrentRaycast.worldPosition);
                if (!eventData.pointerCurrentRaycast.isValid)
                    eventData.position = screenPosition;

                var thisFrameDelta = screenPosition - eventData.position;
                eventData.position = screenPosition;
                eventData.delta = thisFrameDelta;

                // Process Input Actions
                ProcessMouseButton(eventData);
                ProcessMouseMovement(eventData);
                ProcessMouseScroll(eventData);
                ProcessMouseButtonDrag(eventData, dragThresholdMultiplier);
            }

            eventData.LastRaycastResult = result;
            eventData.OnFrameFinished();
        }

        void ProcessMouseButton(RemoteInputEventData eventData)
        {
            var hoverTarget = eventData.pointerCurrentRaycast.gameObject;

            // Check if pressed THIS frame too
            if (eventData.SelectDelta == ButtonDeltaState.Pressed)
            {
                eventData.eligibleForClick = true;
                eventData.delta = Vector2.zero;
                eventData.dragging = false;
                eventData.pressPosition = eventData.position;
                eventData.pointerPressRaycast = eventData.pointerCurrentRaycast;

                var selectHandler = ExecuteEvents.GetEventHandler<ISelectHandler>(hoverTarget);

                // If we have clicked something new, deselect the old thing
                // and leave 'selection handling' up to the press event.
                if (selectHandler != eventSystem.currentSelectedGameObject)
                    eventSystem.SetSelectedGameObject(null, eventData);

                // search for the control that will receive the press.
                // if we can't find a press handler set the press
                // handler to be what would receive a click.

                //pointerDown?.Invoke(hoverTarget, eventData);
                var newPressed = ExecuteEvents.ExecuteHierarchy(hoverTarget, eventData, ExecuteEvents.pointerDownHandler);

                // We didn't find a press handler, so we search for a click handler.
                if (newPressed == null)
                    newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(hoverTarget);

                var time = Time.unscaledTime;

                if (newPressed == eventData.lastPress && ((time - eventData.clickTime) < _clickSpeed))
                    ++eventData.clickCount;
                else
                    eventData.clickCount = 1;

                eventData.clickTime = time;

                eventData.pointerPress = newPressed;
                eventData.rawPointerPress = hoverTarget;

                // Save the drag handler for drag events during this mouse down.
                var dragObject = ExecuteEvents.GetEventHandler<IDragHandler>(hoverTarget);
                eventData.pointerDrag = dragObject;

                if (dragObject != null)
                {
                    //initializePotentialDrag?.Invoke(dragObject, eventData);
                    ExecuteEvents.Execute(dragObject, eventData, ExecuteEvents.initializePotentialDrag);
                }
            }

            // Listen for release THIS frame
            if (eventData.SelectDelta == ButtonDeltaState.Released)
            {
                var target = eventData.pointerPress;
                //pointerUp?.Invoke(target, eventData);
                ExecuteEvents.Execute(target, eventData, ExecuteEvents.pointerUpHandler);

                var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(hoverTarget);
                var pointerDrag = eventData.pointerDrag;
                if (target == pointerUpHandler && eventData.eligibleForClick)
                {
                    //pointerClick?.Invoke(target, eventData);
                    ExecuteEvents.Execute(target, eventData, ExecuteEvents.pointerClickHandler);
                }
                else if (eventData.dragging && pointerDrag != null)
                {
                    //drop?.Invoke(hoverTarget, eventData);
                    ExecuteEvents.ExecuteHierarchy(hoverTarget, eventData, ExecuteEvents.dropHandler);
                }

                eventData.eligibleForClick = false;
                eventData.pointerPress = null;
                eventData.rawPointerPress = null;

                if (eventData.dragging && pointerDrag != null)
                {
                    //endDrag?.Invoke(pointerDrag, eventData);
                    ExecuteEvents.Execute(pointerDrag, eventData, ExecuteEvents.endDragHandler);
                }

                eventData.dragging = false;
                eventData.pointerDrag = null;
            }

            eventData.Remote.SelectDelta = ButtonDeltaState.NoChange;
        }

        void ProcessMouseMovement(PointerEventData eventData)
        {
            var currentPointerTarget = eventData.pointerCurrentRaycast.gameObject;

            // If we have no target or pointerEnter has been deleted,
            // we just send exit events to anything we are tracking
            // and then exit.
            if (currentPointerTarget == null || eventData.pointerEnter == null)
            {
                foreach (var hovered in eventData.hovered)
                {
                    //pointerExit?.Invoke(hovered, eventData);
                    ExecuteEvents.Execute(hovered, eventData, ExecuteEvents.pointerExitHandler);
                }

                eventData.hovered.Clear();

                if (currentPointerTarget == null)
                {
                    eventData.pointerEnter = null;
                    return;
                }
            }

            if (eventData.pointerEnter == currentPointerTarget)
                return;

            var commonRoot = FindCommonRoot(eventData.pointerEnter, currentPointerTarget);

            // We walk up the tree until a common root and the last entered and current entered object is found.
            // Then send exit and enter events up to, but not including, the common root.
            if (eventData.pointerEnter != null)
            {
                var target = eventData.pointerEnter.transform;

                while (target != null)
                {
                    if (commonRoot != null && commonRoot.transform == target)
                        break;

                    var targetGameObject = target.gameObject;
                    //pointerExit?.Invoke(targetGameObject, eventData);
                    ExecuteEvents.Execute(targetGameObject, eventData, ExecuteEvents.pointerExitHandler);

                    eventData.hovered.Remove(targetGameObject);

                    target = target.parent;
                }
            }

            eventData.pointerEnter = currentPointerTarget;
            if (currentPointerTarget != null)
            {
                var target = currentPointerTarget.transform;

                while (target != null && target.gameObject != commonRoot)
                {
                    var targetGameObject = target.gameObject;
                    //pointerEnter?.Invoke(targetGameObject, eventData);
                    ExecuteEvents.Execute(targetGameObject, eventData, ExecuteEvents.pointerEnterHandler);

                    eventData.hovered.Add(targetGameObject);

                    target = target.parent;
                }
            }
        }
        void ProcessMouseButtonDrag(PointerEventData eventData, float pixelDragThresholdMultiplier = 1.0f)
        {
            if (!eventData.IsPointerMoving() ||
                Cursor.lockState == CursorLockMode.Locked ||
                eventData.pointerDrag == null)
            {
                return;
            }

            if (!eventData.dragging)
            {
                var threshold = eventSystem.pixelDragThreshold * pixelDragThresholdMultiplier;
                if ((eventData.pressPosition - eventData.position).sqrMagnitude >= (threshold * threshold))
                {
                    var target = eventData.pointerDrag;
                    //beginDrag?.Invoke(target, eventData);
                    ExecuteEvents.Execute(target, eventData, ExecuteEvents.beginDragHandler);
                    eventData.dragging = true;
                }
            }

            if (eventData.dragging)
            {
                // If we moved from our initial press object, process an up for that object.
                var target = eventData.pointerPress;
                if (target != eventData.pointerDrag)
                {
                    //pointerUp?.Invoke(target, eventData);
                    ExecuteEvents.Execute(target, eventData, ExecuteEvents.pointerUpHandler);

                    eventData.eligibleForClick = false;
                    eventData.pointerPress = null;
                    eventData.rawPointerPress = null;
                }

                //drag?.Invoke(eventData.pointerDrag, eventData);
                ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.dragHandler);
            }
        }
        void ProcessMouseScroll(PointerEventData eventData)
        {
            var scrollDelta = eventData.scrollDelta;
            if (!Mathf.Approximately(scrollDelta.sqrMagnitude, 0f))
            {
                var scrollHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(eventData.pointerEnter);
                //scroll?.Invoke(scrollHandler, eventData);
                ExecuteEvents.ExecuteHierarchy(scrollHandler, eventData, ExecuteEvents.scrollHandler);
            }
        }
        #endregion
    }
}
