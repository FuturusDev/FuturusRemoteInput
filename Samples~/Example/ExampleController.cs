using UnityEngine;
using UnityEngine.InputSystem;

namespace Futurus.RemoteInput.Samples
{
    public class ExampleController : MonoBehaviour
    {
        public static ExampleActions ExampleActions;

        [SerializeField] float _speed = 100;
        [SerializeField] BoxCollider _boundsHolder;
        [SerializeField] ExampleRemoteInputSender _sender;

        Vector2 _currentMovementVector = Vector2.zero;
        Vector3 boundsMin => _boundsHolder.transform.TransformPoint(_boundsHolder.center + (-_boundsHolder.size * 0.5f));
        Vector3 boundsMax => _boundsHolder.transform.TransformPoint(_boundsHolder.center + (_boundsHolder.size * 0.5f));

        void Start()
        {
            ExampleActions = new ExampleActions();
            ExampleActions.Enable();
            ExampleActions.Map.Click.performed += Click_Performed;
            ExampleActions.Map.Click.canceled += Click_Performed;
            ExampleActions.Map.Move.performed += Move_Performed;
            ExampleActions.Map.Move.canceled += Move_Performed;
        }
        void Move_Performed(InputAction.CallbackContext obj) => _currentMovementVector = obj.ReadValue<Vector2>();
        void Click_Performed(InputAction.CallbackContext obj) => _sender.SelectDown = obj.ReadValueAsButton();

        // Update is called once per frame
        void Update()
        {
            var currentPosition = transform.position;
            currentPosition += transform.right * _currentMovementVector.x * _speed * Time.deltaTime;
            currentPosition += transform.up * _currentMovementVector.y * _speed * Time.deltaTime;
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
    }
}

