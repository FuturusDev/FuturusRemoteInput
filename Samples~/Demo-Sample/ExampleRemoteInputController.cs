using Futurus.RemoteInput;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Futurus.RemoteInput.Samples
{
    public class ExampleRemoteInputController : MonoBehaviour
    {
        [SerializeField] float _speed = 100;
        [SerializeField] BoxCollider _boundsHolder;
        [SerializeField] RemoteInputSender _sender;

        Vector3 boundsMin => _boundsHolder.transform.TransformPoint(_boundsHolder.center + (-_boundsHolder.size * 0.5f));
        Vector3 boundsMax => _boundsHolder.transform.TransformPoint(_boundsHolder.center + (_boundsHolder.size * 0.5f));

        // Update is called once per frame
        void Update()
        {
            _sender.SelectDown = Keyboard.current?.spaceKey.isPressed ?? false;
            var currentPosition = transform.position;
            var xFactor = GetMovementFactor(
                Keyboard.current?.rightArrowKey.isPressed ?? false,
                Keyboard.current?.leftArrowKey.isPressed ?? false);
            var yFactor = GetMovementFactor(
                Keyboard.current?.upArrowKey.isPressed ?? false,
                Keyboard.current?.downArrowKey.isPressed ?? false);
            currentPosition += transform.right * xFactor * _speed * Time.deltaTime;
            currentPosition += transform.up * yFactor * _speed * Time.deltaTime;
            if (PositionInsideBounds(currentPosition))
                transform.position = currentPosition;
        }
        bool PositionInsideBounds(Vector3 pos)
        {
            var min = boundsMin;
            var max = boundsMax;
            if (pos.x < min.x || pos.x > max.x)
                return false;
            else if (pos.y < min.y || pos.y > max.y)
                return false;
            else if (pos.z < min.z || pos.z > max.z)
                return false;
            return true;
        }
        float GetMovementFactor(bool positive, bool negative)
        {
            if (positive == negative) // Either both or neither pressed
                return 0f;
            else
                return (positive) ? 1f : -1f;
        }
    }
}
