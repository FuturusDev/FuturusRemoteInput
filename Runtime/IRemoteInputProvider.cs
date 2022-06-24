using UnityEngine;

namespace Futurus.RemoteInput
{
    public interface IRemoteInputProvider
    {
        /// <summary>
        /// Enforces that the interface is either implemented on a Monobehavior or on an object with access to one
        /// </summary>
        public Transform transform { get; }

        /// <summary>
        /// Whether or not to perform additional Physics.Raycast to make sure there's no 3D objects blocking the remote input 
        /// </summary>
        public bool CheckOcclusion { get; }

        /// <summary>
        /// Used if CheckOcclusion is set to true to find blocking physics layers
        /// </summary>
        public LayerMask BlockingOcclusionMask { get; }

        /// <summary>
        /// The maximum distance from the provider that will be considered for remote inpuit
        /// </summary>
        public float MaxDistance { get; }

        /// <summary>
        /// Whether the state of the select option has changed this frame.
        /// </summary>
        public ButtonDeltaState SelectDelta { get; }

        /// <summary>
        /// Whether or not the model should be selecting UI at this moment. This is the equivalent of left mouse down for a mouse.
        /// </summary>
        public bool SelectDown { get; }
    }
}