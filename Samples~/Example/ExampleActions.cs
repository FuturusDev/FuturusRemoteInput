//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.3.0
//     from Assets/Samples/Futurus Remote Input/0.2.0/Example/ExampleActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Futurus.RemoteInput.Samples
{
    public partial class @ExampleActions : IInputActionCollection2, IDisposable
    {
        public InputActionAsset asset { get; }
        public @ExampleActions()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""ExampleActions"",
    ""maps"": [
        {
            ""name"": ""Map"",
            ""id"": ""b6fbc078-63ae-4320-ba72-4563bd637f2d"",
            ""actions"": [
                {
                    ""name"": ""Click"",
                    ""type"": ""Value"",
                    ""id"": ""909ad6c1-b36d-4b89-9ab6-83cc7fc57a83"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""0f1591aa-ed6c-4ded-8ca1-c2d7ef4d619c"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""229e3737-9a1a-4599-be8f-47e89aeb13a4"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""daf832c7-4e23-4f61-9fe8-e287ad6e31e8"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""8904f24e-b1d1-4438-b2f0-b5977b81a583"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""5c0f1be6-ee1c-46f7-878b-fec0545b9688"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""cdac0b7a-c671-4e8b-9195-7753c8ae46ee"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""5ae048df-fac2-4123-a163-fb5e851847e4"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""ce5c5f28-05b4-4dbc-84b0-ecdf7c437e6f"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrows"",
                    ""id"": ""86e0e577-2029-4544-9b55-3e6be4951e08"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""28aa376f-711f-4696-b1d8-87d2859970bf"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""542cd725-5c57-44f6-8bf0-cb83209cca91"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""1c3e36d3-e625-41dc-b9f0-3e300f133047"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""9a984b78-68c0-470c-927d-cd06279c3948"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""b0ec106f-b864-4077-81bc-852c7f24e74c"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Map
            m_Map = asset.FindActionMap("Map", throwIfNotFound: true);
            m_Map_Click = m_Map.FindAction("Click", throwIfNotFound: true);
            m_Map_Move = m_Map.FindAction("Move", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }
        public IEnumerable<InputBinding> bindings => asset.bindings;

        public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
        {
            return asset.FindAction(actionNameOrId, throwIfNotFound);
        }
        public int FindBinding(InputBinding bindingMask, out InputAction action)
        {
            return asset.FindBinding(bindingMask, out action);
        }

        // Map
        private readonly InputActionMap m_Map;
        private IMapActions m_MapActionsCallbackInterface;
        private readonly InputAction m_Map_Click;
        private readonly InputAction m_Map_Move;
        public struct MapActions
        {
            private @ExampleActions m_Wrapper;
            public MapActions(@ExampleActions wrapper) { m_Wrapper = wrapper; }
            public InputAction @Click => m_Wrapper.m_Map_Click;
            public InputAction @Move => m_Wrapper.m_Map_Move;
            public InputActionMap Get() { return m_Wrapper.m_Map; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(MapActions set) { return set.Get(); }
            public void SetCallbacks(IMapActions instance)
            {
                if (m_Wrapper.m_MapActionsCallbackInterface != null)
                {
                    @Click.started -= m_Wrapper.m_MapActionsCallbackInterface.OnClick;
                    @Click.performed -= m_Wrapper.m_MapActionsCallbackInterface.OnClick;
                    @Click.canceled -= m_Wrapper.m_MapActionsCallbackInterface.OnClick;
                    @Move.started -= m_Wrapper.m_MapActionsCallbackInterface.OnMove;
                    @Move.performed -= m_Wrapper.m_MapActionsCallbackInterface.OnMove;
                    @Move.canceled -= m_Wrapper.m_MapActionsCallbackInterface.OnMove;
                }
                m_Wrapper.m_MapActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Click.started += instance.OnClick;
                    @Click.performed += instance.OnClick;
                    @Click.canceled += instance.OnClick;
                    @Move.started += instance.OnMove;
                    @Move.performed += instance.OnMove;
                    @Move.canceled += instance.OnMove;
                }
            }
        }
        public MapActions @Map => new MapActions(this);
        public interface IMapActions
        {
            void OnClick(InputAction.CallbackContext context);
            void OnMove(InputAction.CallbackContext context);
        }
    }
}