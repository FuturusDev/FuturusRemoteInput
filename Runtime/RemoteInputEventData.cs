using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Futurus.RemoteInput
{
    public class RemoteInputEventData : PointerEventData
    {
        static LayerMask EmptyLayerMask = new LayerMask();

        IRemoteInputProvider _provider = null;

        public IRemoteInputProvider Provider => _provider;
        public bool Select { get; set; }
        public ButtonDeltaState SelectDelta { get; set; }
        public Vector3 RemotePosition { get; set; }
        public Quaternion RemoteRotation { get; set; }
        public bool CheckOcclusion { get; set; }
        public LayerMask BlockingOcclusionMask { get; set; }
        public float MaxDistance { get; set; }
        public RaycastResult LastRaycastResult { get; set; }

        public RemoteInputEventData(IRemoteInputProvider provider, EventSystem eventSystem) : base(eventSystem)
        {
            _provider = provider;
        }

        /// <summary>
        /// Resets this object back to defaults.
        /// </summary>
        public void UpdateFromRemote()
        {
            Select = _provider.SelectDown;
            SelectDelta = _provider.SelectDelta;
            RemotePosition = _provider.transform.position;
            RemoteRotation = _provider.transform.rotation;
            CheckOcclusion = _provider.CheckOcclusion;
            MaxDistance = _provider.MaxDistance;
            BlockingOcclusionMask = _provider.BlockingOcclusionMask;
            base.Reset();
        }

        /// <summary>
        /// Resets this object back to defaults.
        /// </summary>
        public override void Reset()
        {
            Select = false;
            SelectDelta = ButtonDeltaState.NoChange;
            RemotePosition = Vector3.zero;
            RemoteRotation = Quaternion.identity;
            CheckOcclusion = false;
            MaxDistance = 0;
            LastRaycastResult = default;
            BlockingOcclusionMask = EmptyLayerMask;
            scrollDelta = Vector2.zero;
            base.Reset();
        }


        /// <summary>
        /// To be called at the end of each frame to reset any tracking of changes within the frame.
        /// </summary>
        /// <seealso cref="SelectDelta"/>
        /// <seealso cref="ChangedThisFrame"/>
        public void OnFrameFinished()
        {
            scrollDelta = Vector2.zero;
        }
    }
}


