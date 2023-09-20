//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.6.3
//     from Assets/Actions/controls.inputactions
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

public partial class @Controls: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""controls"",
    ""maps"": [
        {
            ""name"": ""gameplay"",
            ""id"": ""c02bed80-8585-45c5-ad1e-42489199231e"",
            ""actions"": [
                {
                    ""name"": ""leftClick"",
                    ""type"": ""Button"",
                    ""id"": ""b0b63894-df48-4896-8863-e9bc3565e0ba"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""cursorPos"",
                    ""type"": ""Value"",
                    ""id"": ""da71409d-8820-4319-a31b-ccdad98b6fbf"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""rightClick"",
                    ""type"": ""Button"",
                    ""id"": ""514b0cea-c137-455e-b63e-811470fc26b0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""rightHold"",
                    ""type"": ""Button"",
                    ""id"": ""4ea1b30d-2e46-4348-b4e0-c65d4bb235fc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""scrollValue"",
                    ""type"": ""Value"",
                    ""id"": ""1b1c2047-ced4-44c9-aece-2fda687c4652"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""addButton"",
                    ""type"": ""Button"",
                    ""id"": ""a1460cd7-33df-42cb-aa00-3ed3d1458694"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""subtractButton"",
                    ""type"": ""Button"",
                    ""id"": ""2941d047-a87e-4b29-8b35-75abef3277b5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""300c8643-cd9d-4ce0-9233-bb4476fcc3d2"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""leftClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d5783c05-08a9-4276-99e6-ea0a33a3f63f"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""cursorPos"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""54c0d65d-c593-46dd-828e-e3c61413d820"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""rightClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""caa4f150-ffef-4102-8b41-3a0de91ebaf0"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": ""Hold(duration=0.2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""rightHold"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""041d56e3-8f8b-4c5a-94aa-5bd49802a9f1"",
                    ""path"": ""<Mouse>/scroll"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""scrollValue"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f891f847-8a82-4d91-915f-3371d6abfca3"",
                    ""path"": ""<Keyboard>/equals"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""addButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0d009207-93ca-4f2b-aa15-1a4b864218b3"",
                    ""path"": ""<Keyboard>/minus"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""subtractButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // gameplay
        m_gameplay = asset.FindActionMap("gameplay", throwIfNotFound: true);
        m_gameplay_leftClick = m_gameplay.FindAction("leftClick", throwIfNotFound: true);
        m_gameplay_cursorPos = m_gameplay.FindAction("cursorPos", throwIfNotFound: true);
        m_gameplay_rightClick = m_gameplay.FindAction("rightClick", throwIfNotFound: true);
        m_gameplay_rightHold = m_gameplay.FindAction("rightHold", throwIfNotFound: true);
        m_gameplay_scrollValue = m_gameplay.FindAction("scrollValue", throwIfNotFound: true);
        m_gameplay_addButton = m_gameplay.FindAction("addButton", throwIfNotFound: true);
        m_gameplay_subtractButton = m_gameplay.FindAction("subtractButton", throwIfNotFound: true);
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

    // gameplay
    private readonly InputActionMap m_gameplay;
    private List<IGameplayActions> m_GameplayActionsCallbackInterfaces = new List<IGameplayActions>();
    private readonly InputAction m_gameplay_leftClick;
    private readonly InputAction m_gameplay_cursorPos;
    private readonly InputAction m_gameplay_rightClick;
    private readonly InputAction m_gameplay_rightHold;
    private readonly InputAction m_gameplay_scrollValue;
    private readonly InputAction m_gameplay_addButton;
    private readonly InputAction m_gameplay_subtractButton;
    public struct GameplayActions
    {
        private @Controls m_Wrapper;
        public GameplayActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @leftClick => m_Wrapper.m_gameplay_leftClick;
        public InputAction @cursorPos => m_Wrapper.m_gameplay_cursorPos;
        public InputAction @rightClick => m_Wrapper.m_gameplay_rightClick;
        public InputAction @rightHold => m_Wrapper.m_gameplay_rightHold;
        public InputAction @scrollValue => m_Wrapper.m_gameplay_scrollValue;
        public InputAction @addButton => m_Wrapper.m_gameplay_addButton;
        public InputAction @subtractButton => m_Wrapper.m_gameplay_subtractButton;
        public InputActionMap Get() { return m_Wrapper.m_gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void AddCallbacks(IGameplayActions instance)
        {
            if (instance == null || m_Wrapper.m_GameplayActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_GameplayActionsCallbackInterfaces.Add(instance);
            @leftClick.started += instance.OnLeftClick;
            @leftClick.performed += instance.OnLeftClick;
            @leftClick.canceled += instance.OnLeftClick;
            @cursorPos.started += instance.OnCursorPos;
            @cursorPos.performed += instance.OnCursorPos;
            @cursorPos.canceled += instance.OnCursorPos;
            @rightClick.started += instance.OnRightClick;
            @rightClick.performed += instance.OnRightClick;
            @rightClick.canceled += instance.OnRightClick;
            @rightHold.started += instance.OnRightHold;
            @rightHold.performed += instance.OnRightHold;
            @rightHold.canceled += instance.OnRightHold;
            @scrollValue.started += instance.OnScrollValue;
            @scrollValue.performed += instance.OnScrollValue;
            @scrollValue.canceled += instance.OnScrollValue;
            @addButton.started += instance.OnAddButton;
            @addButton.performed += instance.OnAddButton;
            @addButton.canceled += instance.OnAddButton;
            @subtractButton.started += instance.OnSubtractButton;
            @subtractButton.performed += instance.OnSubtractButton;
            @subtractButton.canceled += instance.OnSubtractButton;
        }

        private void UnregisterCallbacks(IGameplayActions instance)
        {
            @leftClick.started -= instance.OnLeftClick;
            @leftClick.performed -= instance.OnLeftClick;
            @leftClick.canceled -= instance.OnLeftClick;
            @cursorPos.started -= instance.OnCursorPos;
            @cursorPos.performed -= instance.OnCursorPos;
            @cursorPos.canceled -= instance.OnCursorPos;
            @rightClick.started -= instance.OnRightClick;
            @rightClick.performed -= instance.OnRightClick;
            @rightClick.canceled -= instance.OnRightClick;
            @rightHold.started -= instance.OnRightHold;
            @rightHold.performed -= instance.OnRightHold;
            @rightHold.canceled -= instance.OnRightHold;
            @scrollValue.started -= instance.OnScrollValue;
            @scrollValue.performed -= instance.OnScrollValue;
            @scrollValue.canceled -= instance.OnScrollValue;
            @addButton.started -= instance.OnAddButton;
            @addButton.performed -= instance.OnAddButton;
            @addButton.canceled -= instance.OnAddButton;
            @subtractButton.started -= instance.OnSubtractButton;
            @subtractButton.performed -= instance.OnSubtractButton;
            @subtractButton.canceled -= instance.OnSubtractButton;
        }

        public void RemoveCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IGameplayActions instance)
        {
            foreach (var item in m_Wrapper.m_GameplayActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_GameplayActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public GameplayActions @gameplay => new GameplayActions(this);
    public interface IGameplayActions
    {
        void OnLeftClick(InputAction.CallbackContext context);
        void OnCursorPos(InputAction.CallbackContext context);
        void OnRightClick(InputAction.CallbackContext context);
        void OnRightHold(InputAction.CallbackContext context);
        void OnScrollValue(InputAction.CallbackContext context);
        void OnAddButton(InputAction.CallbackContext context);
        void OnSubtractButton(InputAction.CallbackContext context);
    }
}
