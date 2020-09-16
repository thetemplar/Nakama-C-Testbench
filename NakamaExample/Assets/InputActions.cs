// GENERATED AUTOMATICALLY FROM 'Assets/InputActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputActions : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputActions"",
    ""maps"": [
        {
            ""name"": ""Standard"",
            ""id"": ""f20a4a6c-ac0e-4f7f-afae-a0751b32de4a"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""PassThrough"",
                    ""id"": ""f239f9ae-0a5e-42b0-a1ab-abbbec49ea33"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Turn"",
                    ""type"": ""PassThrough"",
                    ""id"": ""677850b7-0ef6-44dc-b4de-eb306d47d347"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Walk"",
                    ""type"": ""Button"",
                    ""id"": ""e6d6aa54-0abd-4740-acc9-1695b79d6c29"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Toggle Move"",
                    ""type"": ""Button"",
                    ""id"": ""4986fdc4-9813-4138-b070-11b092ad89a9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""1b83a6ab-bdf3-4770-be46-45fe32f55e09"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Stop"",
                    ""type"": ""Button"",
                    ""id"": ""cc405cb4-b1c3-4bc8-8fec-736c7561fe79"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Zoom"",
                    ""type"": ""Value"",
                    ""id"": ""c49bd5d6-baae-45dd-b0cc-2418ec66a091"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Mouse"",
                    ""type"": ""Value"",
                    ""id"": ""7a78ad41-f718-4dba-b8ce-e0b10d63c9c1"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LeftButton"",
                    ""type"": ""Button"",
                    ""id"": ""0d80e831-8997-473a-a9cd-c03f40a2170b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Point"",
                    ""type"": ""Value"",
                    ""id"": ""14c35c72-5bde-45c8-98c1-ebcc1481d87b"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""94177924-a9c2-47b4-b37c-bb7885f9d215"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Turn"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""33cf1742-d949-45d1-ae9f-5f2a902b7ccc"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Standard"",
                    ""action"": ""Turn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""a1d79074-20fa-438a-b5f6-0e45674aef88"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Standard"",
                    ""action"": ""Turn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""357d9c1e-3201-4c3d-9a3b-23d87ae29488"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Standard"",
                    ""action"": ""Walk"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cfcb06e9-ee4f-4e41-9672-ab1795866363"",
                    ""path"": ""<Mouse>/backButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Standard"",
                    ""action"": ""Toggle Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9532ea8b-9c9f-496c-b77e-19836bd4824a"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Standard"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2dd6c5fd-3276-4fe9-a5f9-19957ec83608"",
                    ""path"": ""<Mouse>/forwardButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Standard"",
                    ""action"": ""Stop"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""9479cc3d-db45-451e-9841-cd3f0344512d"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Zoom"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""7dd72d21-34e2-488d-b99b-a96e662f2462"",
                    ""path"": ""<Mouse>/scroll"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""42bd8804-d21e-450d-8442-f9881cd6abb8"",
                    ""path"": ""<Mouse>/scroll"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""444042fb-f948-47d7-8ca4-fe0bdf8eb0fc"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""141cf10b-3c53-4a4f-b854-39d1a36b83e6"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": ""InvertVector2"",
                    ""groups"": """",
                    ""action"": ""Mouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""837dcdac-8fc8-4838-b48c-b2262a324201"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""1723e049-b2c8-4c43-8211-aa4c06349350"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""fbeeefeb-3a08-4f85-a287-c40014f5b891"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""babcacbc-b22a-4bca-aa12-32eff870e89f"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""6d282966-2948-451e-a121-83113391bd6a"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""e7250405-c614-42a9-acf2-c2a9a513b2a3"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""0611fa2c-1545-4841-950b-04cb84993419"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""ec4e9dc3-5ccc-48b3-b0c8-002da5458ac0"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""f3023356-abb7-4cfd-81d5-34323ac5e756"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""6516ac0e-fea0-4a40-a655-9d9115b621a6"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""0ceca048-dc7e-46fd-8274-a48b857bfcb6"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Standard"",
            ""bindingGroup"": ""Standard"",
            ""devices"": []
        }
    ]
}");
        // Standard
        m_Standard = asset.FindActionMap("Standard", throwIfNotFound: true);
        m_Standard_Movement = m_Standard.FindAction("Movement", throwIfNotFound: true);
        m_Standard_Turn = m_Standard.FindAction("Turn", throwIfNotFound: true);
        m_Standard_Walk = m_Standard.FindAction("Walk", throwIfNotFound: true);
        m_Standard_ToggleMove = m_Standard.FindAction("Toggle Move", throwIfNotFound: true);
        m_Standard_Jump = m_Standard.FindAction("Jump", throwIfNotFound: true);
        m_Standard_Stop = m_Standard.FindAction("Stop", throwIfNotFound: true);
        m_Standard_Zoom = m_Standard.FindAction("Zoom", throwIfNotFound: true);
        m_Standard_Mouse = m_Standard.FindAction("Mouse", throwIfNotFound: true);
        m_Standard_LeftButton = m_Standard.FindAction("LeftButton", throwIfNotFound: true);
        m_Standard_Point = m_Standard.FindAction("Point", throwIfNotFound: true);
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

    // Standard
    private readonly InputActionMap m_Standard;
    private IStandardActions m_StandardActionsCallbackInterface;
    private readonly InputAction m_Standard_Movement;
    private readonly InputAction m_Standard_Turn;
    private readonly InputAction m_Standard_Walk;
    private readonly InputAction m_Standard_ToggleMove;
    private readonly InputAction m_Standard_Jump;
    private readonly InputAction m_Standard_Stop;
    private readonly InputAction m_Standard_Zoom;
    private readonly InputAction m_Standard_Mouse;
    private readonly InputAction m_Standard_LeftButton;
    private readonly InputAction m_Standard_Point;
    public struct StandardActions
    {
        private @InputActions m_Wrapper;
        public StandardActions(@InputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Standard_Movement;
        public InputAction @Turn => m_Wrapper.m_Standard_Turn;
        public InputAction @Walk => m_Wrapper.m_Standard_Walk;
        public InputAction @ToggleMove => m_Wrapper.m_Standard_ToggleMove;
        public InputAction @Jump => m_Wrapper.m_Standard_Jump;
        public InputAction @Stop => m_Wrapper.m_Standard_Stop;
        public InputAction @Zoom => m_Wrapper.m_Standard_Zoom;
        public InputAction @Mouse => m_Wrapper.m_Standard_Mouse;
        public InputAction @LeftButton => m_Wrapper.m_Standard_LeftButton;
        public InputAction @Point => m_Wrapper.m_Standard_Point;
        public InputActionMap Get() { return m_Wrapper.m_Standard; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(StandardActions set) { return set.Get(); }
        public void SetCallbacks(IStandardActions instance)
        {
            if (m_Wrapper.m_StandardActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_StandardActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_StandardActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_StandardActionsCallbackInterface.OnMovement;
                @Turn.started -= m_Wrapper.m_StandardActionsCallbackInterface.OnTurn;
                @Turn.performed -= m_Wrapper.m_StandardActionsCallbackInterface.OnTurn;
                @Turn.canceled -= m_Wrapper.m_StandardActionsCallbackInterface.OnTurn;
                @Walk.started -= m_Wrapper.m_StandardActionsCallbackInterface.OnWalk;
                @Walk.performed -= m_Wrapper.m_StandardActionsCallbackInterface.OnWalk;
                @Walk.canceled -= m_Wrapper.m_StandardActionsCallbackInterface.OnWalk;
                @ToggleMove.started -= m_Wrapper.m_StandardActionsCallbackInterface.OnToggleMove;
                @ToggleMove.performed -= m_Wrapper.m_StandardActionsCallbackInterface.OnToggleMove;
                @ToggleMove.canceled -= m_Wrapper.m_StandardActionsCallbackInterface.OnToggleMove;
                @Jump.started -= m_Wrapper.m_StandardActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_StandardActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_StandardActionsCallbackInterface.OnJump;
                @Stop.started -= m_Wrapper.m_StandardActionsCallbackInterface.OnStop;
                @Stop.performed -= m_Wrapper.m_StandardActionsCallbackInterface.OnStop;
                @Stop.canceled -= m_Wrapper.m_StandardActionsCallbackInterface.OnStop;
                @Zoom.started -= m_Wrapper.m_StandardActionsCallbackInterface.OnZoom;
                @Zoom.performed -= m_Wrapper.m_StandardActionsCallbackInterface.OnZoom;
                @Zoom.canceled -= m_Wrapper.m_StandardActionsCallbackInterface.OnZoom;
                @Mouse.started -= m_Wrapper.m_StandardActionsCallbackInterface.OnMouse;
                @Mouse.performed -= m_Wrapper.m_StandardActionsCallbackInterface.OnMouse;
                @Mouse.canceled -= m_Wrapper.m_StandardActionsCallbackInterface.OnMouse;
                @LeftButton.started -= m_Wrapper.m_StandardActionsCallbackInterface.OnLeftButton;
                @LeftButton.performed -= m_Wrapper.m_StandardActionsCallbackInterface.OnLeftButton;
                @LeftButton.canceled -= m_Wrapper.m_StandardActionsCallbackInterface.OnLeftButton;
                @Point.started -= m_Wrapper.m_StandardActionsCallbackInterface.OnPoint;
                @Point.performed -= m_Wrapper.m_StandardActionsCallbackInterface.OnPoint;
                @Point.canceled -= m_Wrapper.m_StandardActionsCallbackInterface.OnPoint;
            }
            m_Wrapper.m_StandardActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Turn.started += instance.OnTurn;
                @Turn.performed += instance.OnTurn;
                @Turn.canceled += instance.OnTurn;
                @Walk.started += instance.OnWalk;
                @Walk.performed += instance.OnWalk;
                @Walk.canceled += instance.OnWalk;
                @ToggleMove.started += instance.OnToggleMove;
                @ToggleMove.performed += instance.OnToggleMove;
                @ToggleMove.canceled += instance.OnToggleMove;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Stop.started += instance.OnStop;
                @Stop.performed += instance.OnStop;
                @Stop.canceled += instance.OnStop;
                @Zoom.started += instance.OnZoom;
                @Zoom.performed += instance.OnZoom;
                @Zoom.canceled += instance.OnZoom;
                @Mouse.started += instance.OnMouse;
                @Mouse.performed += instance.OnMouse;
                @Mouse.canceled += instance.OnMouse;
                @LeftButton.started += instance.OnLeftButton;
                @LeftButton.performed += instance.OnLeftButton;
                @LeftButton.canceled += instance.OnLeftButton;
                @Point.started += instance.OnPoint;
                @Point.performed += instance.OnPoint;
                @Point.canceled += instance.OnPoint;
            }
        }
    }
    public StandardActions @Standard => new StandardActions(this);
    private int m_StandardSchemeIndex = -1;
    public InputControlScheme StandardScheme
    {
        get
        {
            if (m_StandardSchemeIndex == -1) m_StandardSchemeIndex = asset.FindControlSchemeIndex("Standard");
            return asset.controlSchemes[m_StandardSchemeIndex];
        }
    }
    public interface IStandardActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnTurn(InputAction.CallbackContext context);
        void OnWalk(InputAction.CallbackContext context);
        void OnToggleMove(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnStop(InputAction.CallbackContext context);
        void OnZoom(InputAction.CallbackContext context);
        void OnMouse(InputAction.CallbackContext context);
        void OnLeftButton(InputAction.CallbackContext context);
        void OnPoint(InputAction.CallbackContext context);
    }
}
