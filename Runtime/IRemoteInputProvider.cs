using UnityEngine;

namespace Futurus.RemoteInput
{
    public interface IRemoteInputProvider
    {
        public Transform transform { get; }

        /// <summary>
        /// Whether or not to perform additional Physics.Raycast to make sure there's no 3D objects blocking the remote input 
        /// </summary>
        public bool CheckOcclusion { get; }
        public LayerMask BlockingOcclusionMask { get; }
        public float MaxDistance { get; }

        /// <summary>
        /// Whether the state of the select option has changed this frame.
        /// </summary>
        public ButtonDeltaState SelectDelta { get; set; }

        /// <summary>
        /// Whether or not the model should be selecting UI at this moment. This is the equivalent of left mouse down for a mouse.
        /// </summary>
        public bool SelectDown { get; }
    }
}