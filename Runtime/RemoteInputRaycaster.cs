using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Futurus.RemoteInput
{
    public class RemoteInputRaycaster : BaseRaycaster
    {
        public static readonly HashSet<RemoteInputRaycaster> AllRaycasters = new HashSet<RemoteInputRaycaster>();
        public static System.Action OnRaycastersChanged;

        protected struct RaycastHitData
        {
            public RaycastHitData(Graphic graphic, Vector3 worldHitPosition, Vector2 screenPosition, float distance)
            {
                this.graphic = graphic;
                this.worldHitPosition = worldHitPosition;
                this.screenPosition = screenPosition;
                this.distance = distance;
            }

            public Graphic graphic { get; }
            public Vector3 worldHitPosition { get; }
            public Vector2 screenPosition { get; }
            public float distance { get; }
        }

        #region Inspector
        [SerializeField] Canvas _canvas;
        [SerializeField] bool _ignoreReversedGraphics;
        #endregion

        #region Public
        public virtual bool CanRaycast => _canvas != null && _canvas.isActiveAndEnabled;
        public override Camera eventCamera => _canvas?.worldCamera != null ? _canvas.worldCamera : Camera.main;
        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            if (eventData is RemoteInputEventData trackedEventData)
                PerformRaycast(trackedEventData, resultAppendList);
        }
        public virtual void Raycast(RemoteInputEventData eventData, List<RaycastResult> resultAppendList)
        {
            PerformRaycast(eventData, resultAppendList);
        }
        #endregion

        #region Unity
        protected override void OnEnable()
        {
            base.OnEnable();
            AllRaycasters.Add(this);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            AllRaycasters.Remove(this);
        }
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            _canvas = _canvas ?? GetComponent<Canvas>();
        }
#endif
        #endregion

        #region Internal
        protected virtual void PerformRaycast(RemoteInputEventData eventData, List<RaycastResult> resultAppendList)
        {
            if (_canvas == null || eventCamera == null)
                return;
            var ray = new Ray(eventData.RemotePosition, eventData.RemoteRotation * Vector3.forward);
            var hitDistance = eventData.MaxDistance;
            Vector3 physicsRaycastNormal = Vector3.zero;
            if (eventData.CheckOcclusion)
            {
                var hits = Physics.RaycastAll(ray, hitDistance, eventData.BlockingOcclusionMask);
                if (hits.Length > 0 && hits[0].distance < hitDistance)
                {
                    hitDistance = hits[0].distance;
                    physicsRaycastNormal = hits[0].normal;
                }
            }

            _raycastResultsCache.Clear();
            SortedRaycastGraphics(_canvas, ray, _raycastResultsCache);
            // Now that we have a list of sorted hits, process any extra settings and filters.
            for (var i = 0; i < _raycastResultsCache.Count; i++)
            {
                var validHit = true;

                var hitData = _raycastResultsCache[i];

                var go = hitData.graphic.gameObject;
                if (_ignoreReversedGraphics)
                {
                    var forward = ray.direction;
                    var goDirection = go.transform.rotation * Vector3.forward;
                    validHit = Vector3.Dot(forward, goDirection) > 0;
                }

                validHit &= hitData.distance < hitDistance;

                if (validHit)
                {
                    var castResult = new RaycastResult
                    {
                        gameObject = go,
                        module = this,
                        distance = hitData.distance,
                        index = resultAppendList.Count,
                        depth = hitData.graphic.depth,

                        worldPosition = hitData.worldHitPosition,
                        screenPosition = hitData.screenPosition,
                        worldNormal = physicsRaycastNormal != Vector3.zero ? physicsRaycastNormal : -_canvas.transform.forward,
                    };
                    resultAppendList.Add(castResult);
                }
            }
        }
        // Copied from Unity
        List<RaycastHitData> _raycastResultsCache = new List<RaycastHitData>();
        static readonly List<RaycastHitData> _sortedGraphics = new List<RaycastHitData>();
        void SortedRaycastGraphics(Canvas canvas, Ray ray, List<RaycastHitData> results)
        {
            var graphics = GraphicRegistry.GetGraphicsForCanvas(canvas);

            _sortedGraphics.Clear();
            for (var i = 0; i < graphics.Count; ++i)
            {
                var graphic = graphics[i];

                if (graphic.depth == -1)
                    continue;

                Vector3 worldPos;
                float distance;
                if (RayIntersectsRectTransform(graphic.rectTransform, ray, out worldPos, out distance))
                {
                    Vector2 screenPos = eventCamera.WorldToScreenPoint(worldPos);
                    // mask/image intersection - See Unity docs on eventAlphaThreshold for when this does anything
                    if (graphic.Raycast(screenPos, eventCamera))
                    {
                        _sortedGraphics.Add(new RaycastHitData(graphic, worldPos, screenPos, distance));
                    }
                }
            }

            _sortedGraphics.Sort((g1, g2) => g2.graphic.depth.CompareTo(g1.graphic.depth));

            results.AddRange(_sortedGraphics);
        }
        static bool RayIntersectsRectTransform(RectTransform transform, Ray ray, out Vector3 worldPosition, out float distance)
        {
            var corners = new Vector3[4];
            transform.GetWorldCorners(corners);
            var plane = new Plane(corners[0], corners[1], corners[2]);

            float enter;
            if (plane.Raycast(ray, out enter))
            {
                var intersection = ray.GetPoint(enter);

                var bottomEdge = corners[3] - corners[0];
                var leftEdge = corners[1] - corners[0];
                var bottomDot = Vector3.Dot(intersection - corners[0], bottomEdge);
                var leftDot = Vector3.Dot(intersection - corners[0], leftEdge);

                // If the intersection is right of the left edge and above the bottom edge.
                if (leftDot >= 0 && bottomDot >= 0)
                {
                    var topEdge = corners[1] - corners[2];
                    var rightEdge = corners[3] - corners[2];
                    var topDot = Vector3.Dot(intersection - corners[2], topEdge);
                    var rightDot = Vector3.Dot(intersection - corners[2], rightEdge);

                    //If the intersection is left of the right edge, and below the top edge
                    if (topDot >= 0 && rightDot >= 0)
                    {
                        worldPosition = intersection;
                        distance = enter;
                        return true;
                    }
                }
            }
            worldPosition = Vector3.zero;
            distance = 0;
            return false;
        }
        #endregion
    }

}