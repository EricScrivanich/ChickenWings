//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/InputController.inputactions
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

public partial class @InputController: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputController()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputController"",
    ""maps"": [
        {
            ""name"": ""Movement"",
            ""id"": ""af9450ba-218f-4f79-a3d6-827cbaa3990d"",
            ""actions"": [
                {
                    ""name"": ""Fireball"",
                    ""type"": ""Button"",
                    ""id"": ""65cbda46-3c59-45c5-84c0-f4670e81305b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""DropEgg"",
                    ""type"": ""Button"",
                    ""id"": ""c8406085-9a15-49ea-abe0-cfeaa5057678"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""9df870a7-2b9e-4307-b0ce-553eb6ecbcc1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Drop"",
                    ""type"": ""Button"",
                    ""id"": ""03838bde-c4e5-4a85-b37a-f61f8f371e17"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""JumpRight"",
                    ""type"": ""Button"",
                    ""id"": ""711ca112-25f4-4f11-9efe-0c329e2f5064"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""JumpLeft"",
                    ""type"": ""Button"",
                    ""id"": ""9b324f8c-5dcd-463d-927a-6b176a5951c5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""JumpHold"",
                    ""type"": ""Button"",
                    ""id"": ""7a1b2628-cdb6-44df-96c2-b30778de634d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold(duration=0.2)"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""TouchPress"",
                    ""type"": ""Button"",
                    ""id"": ""c8666c9d-9f9a-44c8-bb01-83f974e4900a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""TouchPosition"",
                    ""type"": ""Value"",
                    ""id"": ""2f75b153-ec9f-4888-b592-e5c396cc1467"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Parachute"",
                    ""type"": ""Button"",
                    ""id"": ""267adbde-d536-4f69-acf1-6296c8214c71"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold(duration=0.1)"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""6a7bb245-7739-4352-a7ae-757bdda03107"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""DashSlash"",
                    ""type"": ""Button"",
                    ""id"": ""1cbbf4c4-de14-4525-b189-264d8197cd0b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold(duration=0.32)"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""HoldJumpRight"",
                    ""type"": ""Button"",
                    ""id"": ""342af9b5-332f-49db-8399-4e22cff16772"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""HoldJumpLeft"",
                    ""type"": ""Button"",
                    ""id"": ""1f04c387-79af-48fc-a9a7-83c8527cca1b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold"",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""1509b281-cd61-4491-b3f1-45db31dbb1e8"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""JumpHold"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""355bd070-138f-4545-96cf-bdcafae4e0e9"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mobile"",
                    ""action"": ""JumpHold"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2b7079e5-c39a-4c93-9bc7-410e1ac8487c"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""JumpLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fd76ad71-f0a6-4336-9bd4-73384afa7ea9"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mobile"",
                    ""action"": ""JumpLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7c57ff5b-84f5-48d5-a121-b44e03cce482"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""JumpRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1b08dbd0-a67f-40f2-bffa-22366bd91f60"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mobile"",
                    ""action"": ""JumpRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f8f9e6a0-1c12-40f2-ab4c-42dfceff0d90"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Drop"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6985edf9-1ce9-497d-a597-f45b7275fa2c"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mobile"",
                    ""action"": ""Drop"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1118d868-368b-43b5-88e6-4caae8c07017"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9749ddd7-ac4f-4761-8675-9edfef8728e1"",
                    ""path"": ""<Gamepad>/leftStickPress"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mobile"",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d5ef087a-7730-41b6-b474-5933a45b45f7"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""DropEgg"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8b7f91a3-6adb-4672-8351-0d8d5e2a09f4"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mobile"",
                    ""action"": ""DropEgg"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b1d3526f-f1c8-41dd-9c85-ad65b57ce2bb"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Fireball"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""60037d80-6c76-40cd-b93a-47e30005c252"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mobile"",
                    ""action"": ""Fireball"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cf6de740-0bd6-4f1d-abb7-6d15f43f259f"",
                    ""path"": ""<Touchscreen>/primaryTouch/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TouchPress"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""70d7505d-0113-4039-8d8f-7839975a94fc"",
                    ""path"": ""<Touchscreen>/primaryTouch/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TouchPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6d3349ca-646a-426f-a376-dbb4c1bd7a64"",
                    ""path"": ""<Keyboard>/#(C)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Parachute"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""02baf8d2-731d-48a2-9e7c-d6fdb850d492"",
                    ""path"": ""<Gamepad>/rightStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mobile"",
                    ""action"": ""Parachute"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""71785b00-5b5a-4d60-a60a-bfe45c783bc6"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": ""Hold(duration=0.07,pressPoint=0.2),Tap(pressPoint=0.2)"",
                    ""processors"": """",
                    ""groups"": ""Mobile"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""47e0b5fe-6038-46bf-aba8-7552f7a46f65"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6624c8c8-2105-4363-885c-22ba6cf97fdf"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""DashSlash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e7a56b1f-185b-4242-b3b9-7cee360a64d4"",
                    ""path"": ""<Gamepad>/leftStickPress"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mobile"",
                    ""action"": ""DashSlash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""65402cf2-ba68-4ea2-a80e-b110f918e486"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""HoldJumpRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f7008ccd-30d7-44ae-b96a-f9d908d8ffc4"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mobile"",
                    ""action"": ""HoldJumpRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a59648c9-b057-459d-88ef-736d8d34bbcc"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""HoldJumpLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5f07c72c-6c06-4a63-b397-039e568ca96e"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mobile"",
                    ""action"": ""HoldJumpLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Special"",
            ""id"": ""89737611-948f-48ca-9565-39ddf240200a"",
            ""actions"": [
                {
                    ""name"": ""ResetGame"",
                    ""type"": ""Button"",
                    ""id"": ""cd5e4810-d90d-4db2-af42-5e682f6b0ddb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""736e6183-b0e0-4a19-bf47-5c3589aa0ae2"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ResetGame"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c1886e7d-e7b3-4feb-9977-82229d0f8918"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ResetGame"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""368892b5-9405-43e0-8235-f32537793a6b"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mobile"",
                    ""action"": ""ResetGame"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": true,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Mobile"",
            ""bindingGroup"": ""Mobile"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": true,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Movement
        m_Movement = asset.FindActionMap("Movement", throwIfNotFound: true);
        m_Movement_Fireball = m_Movement.FindAction("Fireball", throwIfNotFound: true);
        m_Movement_DropEgg = m_Movement.FindAction("DropEgg", throwIfNotFound: true);
        m_Movement_Dash = m_Movement.FindAction("Dash", throwIfNotFound: true);
        m_Movement_Drop = m_Movement.FindAction("Drop", throwIfNotFound: true);
        m_Movement_JumpRight = m_Movement.FindAction("JumpRight", throwIfNotFound: true);
        m_Movement_JumpLeft = m_Movement.FindAction("JumpLeft", throwIfNotFound: true);
        m_Movement_JumpHold = m_Movement.FindAction("JumpHold", throwIfNotFound: true);
        m_Movement_TouchPress = m_Movement.FindAction("TouchPress", throwIfNotFound: true);
        m_Movement_TouchPosition = m_Movement.FindAction("TouchPosition", throwIfNotFound: true);
        m_Movement_Parachute = m_Movement.FindAction("Parachute", throwIfNotFound: true);
        m_Movement_Jump = m_Movement.FindAction("Jump", throwIfNotFound: true);
        m_Movement_DashSlash = m_Movement.FindAction("DashSlash", throwIfNotFound: true);
        m_Movement_HoldJumpRight = m_Movement.FindAction("HoldJumpRight", throwIfNotFound: true);
        m_Movement_HoldJumpLeft = m_Movement.FindAction("HoldJumpLeft", throwIfNotFound: true);
        // Special
        m_Special = asset.FindActionMap("Special", throwIfNotFound: true);
        m_Special_ResetGame = m_Special.FindAction("ResetGame", throwIfNotFound: true);
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

    // Movement
    private readonly InputActionMap m_Movement;
    private List<IMovementActions> m_MovementActionsCallbackInterfaces = new List<IMovementActions>();
    private readonly InputAction m_Movement_Fireball;
    private readonly InputAction m_Movement_DropEgg;
    private readonly InputAction m_Movement_Dash;
    private readonly InputAction m_Movement_Drop;
    private readonly InputAction m_Movement_JumpRight;
    private readonly InputAction m_Movement_JumpLeft;
    private readonly InputAction m_Movement_JumpHold;
    private readonly InputAction m_Movement_TouchPress;
    private readonly InputAction m_Movement_TouchPosition;
    private readonly InputAction m_Movement_Parachute;
    private readonly InputAction m_Movement_Jump;
    private readonly InputAction m_Movement_DashSlash;
    private readonly InputAction m_Movement_HoldJumpRight;
    private readonly InputAction m_Movement_HoldJumpLeft;
    public struct MovementActions
    {
        private @InputController m_Wrapper;
        public MovementActions(@InputController wrapper) { m_Wrapper = wrapper; }
        public InputAction @Fireball => m_Wrapper.m_Movement_Fireball;
        public InputAction @DropEgg => m_Wrapper.m_Movement_DropEgg;
        public InputAction @Dash => m_Wrapper.m_Movement_Dash;
        public InputAction @Drop => m_Wrapper.m_Movement_Drop;
        public InputAction @JumpRight => m_Wrapper.m_Movement_JumpRight;
        public InputAction @JumpLeft => m_Wrapper.m_Movement_JumpLeft;
        public InputAction @JumpHold => m_Wrapper.m_Movement_JumpHold;
        public InputAction @TouchPress => m_Wrapper.m_Movement_TouchPress;
        public InputAction @TouchPosition => m_Wrapper.m_Movement_TouchPosition;
        public InputAction @Parachute => m_Wrapper.m_Movement_Parachute;
        public InputAction @Jump => m_Wrapper.m_Movement_Jump;
        public InputAction @DashSlash => m_Wrapper.m_Movement_DashSlash;
        public InputAction @HoldJumpRight => m_Wrapper.m_Movement_HoldJumpRight;
        public InputAction @HoldJumpLeft => m_Wrapper.m_Movement_HoldJumpLeft;
        public InputActionMap Get() { return m_Wrapper.m_Movement; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MovementActions set) { return set.Get(); }
        public void AddCallbacks(IMovementActions instance)
        {
            if (instance == null || m_Wrapper.m_MovementActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_MovementActionsCallbackInterfaces.Add(instance);
            @Fireball.started += instance.OnFireball;
            @Fireball.performed += instance.OnFireball;
            @Fireball.canceled += instance.OnFireball;
            @DropEgg.started += instance.OnDropEgg;
            @DropEgg.performed += instance.OnDropEgg;
            @DropEgg.canceled += instance.OnDropEgg;
            @Dash.started += instance.OnDash;
            @Dash.performed += instance.OnDash;
            @Dash.canceled += instance.OnDash;
            @Drop.started += instance.OnDrop;
            @Drop.performed += instance.OnDrop;
            @Drop.canceled += instance.OnDrop;
            @JumpRight.started += instance.OnJumpRight;
            @JumpRight.performed += instance.OnJumpRight;
            @JumpRight.canceled += instance.OnJumpRight;
            @JumpLeft.started += instance.OnJumpLeft;
            @JumpLeft.performed += instance.OnJumpLeft;
            @JumpLeft.canceled += instance.OnJumpLeft;
            @JumpHold.started += instance.OnJumpHold;
            @JumpHold.performed += instance.OnJumpHold;
            @JumpHold.canceled += instance.OnJumpHold;
            @TouchPress.started += instance.OnTouchPress;
            @TouchPress.performed += instance.OnTouchPress;
            @TouchPress.canceled += instance.OnTouchPress;
            @TouchPosition.started += instance.OnTouchPosition;
            @TouchPosition.performed += instance.OnTouchPosition;
            @TouchPosition.canceled += instance.OnTouchPosition;
            @Parachute.started += instance.OnParachute;
            @Parachute.performed += instance.OnParachute;
            @Parachute.canceled += instance.OnParachute;
            @Jump.started += instance.OnJump;
            @Jump.performed += instance.OnJump;
            @Jump.canceled += instance.OnJump;
            @DashSlash.started += instance.OnDashSlash;
            @DashSlash.performed += instance.OnDashSlash;
            @DashSlash.canceled += instance.OnDashSlash;
            @HoldJumpRight.started += instance.OnHoldJumpRight;
            @HoldJumpRight.performed += instance.OnHoldJumpRight;
            @HoldJumpRight.canceled += instance.OnHoldJumpRight;
            @HoldJumpLeft.started += instance.OnHoldJumpLeft;
            @HoldJumpLeft.performed += instance.OnHoldJumpLeft;
            @HoldJumpLeft.canceled += instance.OnHoldJumpLeft;
        }

        private void UnregisterCallbacks(IMovementActions instance)
        {
            @Fireball.started -= instance.OnFireball;
            @Fireball.performed -= instance.OnFireball;
            @Fireball.canceled -= instance.OnFireball;
            @DropEgg.started -= instance.OnDropEgg;
            @DropEgg.performed -= instance.OnDropEgg;
            @DropEgg.canceled -= instance.OnDropEgg;
            @Dash.started -= instance.OnDash;
            @Dash.performed -= instance.OnDash;
            @Dash.canceled -= instance.OnDash;
            @Drop.started -= instance.OnDrop;
            @Drop.performed -= instance.OnDrop;
            @Drop.canceled -= instance.OnDrop;
            @JumpRight.started -= instance.OnJumpRight;
            @JumpRight.performed -= instance.OnJumpRight;
            @JumpRight.canceled -= instance.OnJumpRight;
            @JumpLeft.started -= instance.OnJumpLeft;
            @JumpLeft.performed -= instance.OnJumpLeft;
            @JumpLeft.canceled -= instance.OnJumpLeft;
            @JumpHold.started -= instance.OnJumpHold;
            @JumpHold.performed -= instance.OnJumpHold;
            @JumpHold.canceled -= instance.OnJumpHold;
            @TouchPress.started -= instance.OnTouchPress;
            @TouchPress.performed -= instance.OnTouchPress;
            @TouchPress.canceled -= instance.OnTouchPress;
            @TouchPosition.started -= instance.OnTouchPosition;
            @TouchPosition.performed -= instance.OnTouchPosition;
            @TouchPosition.canceled -= instance.OnTouchPosition;
            @Parachute.started -= instance.OnParachute;
            @Parachute.performed -= instance.OnParachute;
            @Parachute.canceled -= instance.OnParachute;
            @Jump.started -= instance.OnJump;
            @Jump.performed -= instance.OnJump;
            @Jump.canceled -= instance.OnJump;
            @DashSlash.started -= instance.OnDashSlash;
            @DashSlash.performed -= instance.OnDashSlash;
            @DashSlash.canceled -= instance.OnDashSlash;
            @HoldJumpRight.started -= instance.OnHoldJumpRight;
            @HoldJumpRight.performed -= instance.OnHoldJumpRight;
            @HoldJumpRight.canceled -= instance.OnHoldJumpRight;
            @HoldJumpLeft.started -= instance.OnHoldJumpLeft;
            @HoldJumpLeft.performed -= instance.OnHoldJumpLeft;
            @HoldJumpLeft.canceled -= instance.OnHoldJumpLeft;
        }

        public void RemoveCallbacks(IMovementActions instance)
        {
            if (m_Wrapper.m_MovementActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IMovementActions instance)
        {
            foreach (var item in m_Wrapper.m_MovementActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_MovementActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public MovementActions @Movement => new MovementActions(this);

    // Special
    private readonly InputActionMap m_Special;
    private List<ISpecialActions> m_SpecialActionsCallbackInterfaces = new List<ISpecialActions>();
    private readonly InputAction m_Special_ResetGame;
    public struct SpecialActions
    {
        private @InputController m_Wrapper;
        public SpecialActions(@InputController wrapper) { m_Wrapper = wrapper; }
        public InputAction @ResetGame => m_Wrapper.m_Special_ResetGame;
        public InputActionMap Get() { return m_Wrapper.m_Special; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(SpecialActions set) { return set.Get(); }
        public void AddCallbacks(ISpecialActions instance)
        {
            if (instance == null || m_Wrapper.m_SpecialActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_SpecialActionsCallbackInterfaces.Add(instance);
            @ResetGame.started += instance.OnResetGame;
            @ResetGame.performed += instance.OnResetGame;
            @ResetGame.canceled += instance.OnResetGame;
        }

        private void UnregisterCallbacks(ISpecialActions instance)
        {
            @ResetGame.started -= instance.OnResetGame;
            @ResetGame.performed -= instance.OnResetGame;
            @ResetGame.canceled -= instance.OnResetGame;
        }

        public void RemoveCallbacks(ISpecialActions instance)
        {
            if (m_Wrapper.m_SpecialActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(ISpecialActions instance)
        {
            foreach (var item in m_Wrapper.m_SpecialActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_SpecialActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public SpecialActions @Special => new SpecialActions(this);
    private int m_KeyboardSchemeIndex = -1;
    public InputControlScheme KeyboardScheme
    {
        get
        {
            if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
            return asset.controlSchemes[m_KeyboardSchemeIndex];
        }
    }
    private int m_MobileSchemeIndex = -1;
    public InputControlScheme MobileScheme
    {
        get
        {
            if (m_MobileSchemeIndex == -1) m_MobileSchemeIndex = asset.FindControlSchemeIndex("Mobile");
            return asset.controlSchemes[m_MobileSchemeIndex];
        }
    }
    public interface IMovementActions
    {
        void OnFireball(InputAction.CallbackContext context);
        void OnDropEgg(InputAction.CallbackContext context);
        void OnDash(InputAction.CallbackContext context);
        void OnDrop(InputAction.CallbackContext context);
        void OnJumpRight(InputAction.CallbackContext context);
        void OnJumpLeft(InputAction.CallbackContext context);
        void OnJumpHold(InputAction.CallbackContext context);
        void OnTouchPress(InputAction.CallbackContext context);
        void OnTouchPosition(InputAction.CallbackContext context);
        void OnParachute(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnDashSlash(InputAction.CallbackContext context);
        void OnHoldJumpRight(InputAction.CallbackContext context);
        void OnHoldJumpLeft(InputAction.CallbackContext context);
    }
    public interface ISpecialActions
    {
        void OnResetGame(InputAction.CallbackContext context);
    }
}
