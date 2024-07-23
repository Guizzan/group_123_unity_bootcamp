using UnityEngine;
using UnityEngine.InputSystem;
using Guizzan.Input.GIM;
using Guizzan.Input.GIM.Player;
using System.Linq;
using UnityEngine.UI;

using InputValue = Guizzan.Input.GIM.InputValue;

[RequireComponent(typeof(PlayerInput))]
public class GuizzanInputManager : MonoBehaviour
{
    [Header(">>>>>>>>>>>>> UI Parameters")]
    public UICanvas MenuUI;
    public UICanvas GameUI;
    public UICanvas DeathMenuUI;
    public UICanvas PauseMenuUI;

    [Header(">>>>>>>>>>>>> Character Movement Parameters <<<<<<<<<<<<<")]
    [Range(0f, 100f)]
    public float MouseSensitivityX;
    [Range(0f, 100f)]
    public float MouseSensitivityY;
    [HideInInspector] public Controls _controls = null;
    [SerializeField] private InputActionReference ScrollControl;

    public Vector2 MovementVector;
    public Vector2 LookVectorDelta;
    public float ScrollDelta;

    [Header(">>>>>>>>>>>>> Runtime Parameters <<<<<<<<<<<<<")]
    public InputModes _inputMode = InputModes.None;
    public Controller _controller = Controller.None;

    public PlayerController _playerController;
    public static GuizzanInputManager Instance;
    private static bool _cursorBool;
    delegate void setControls();
    private setControls SetControls;

    private void OnEnable()
    {
        _controls.Player.Enable();
        InputSystem.onDeviceChange += OnDeviceChange;
        GetComponent<PlayerInput>().onControlsChanged += OnControlsChanged;
        SelectEvent.OnSelectionUpdated += OnSelectionChanged;
    }
    private void OnDisable()
    {
        _controls.Player.Disable();
        InputSystem.onDeviceChange -= OnDeviceChange;
        GetComponent<PlayerInput>().onControlsChanged -= OnControlsChanged;
        SelectEvent.OnSelectionUpdated -= OnSelectionChanged;
    }
    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); }
        Instance = this;
        _controls = new Controls();
        _controls.Player.Enable();
        DontDestroyOnLoad(gameObject);
        _controls.Player.Movement.performed += (ctx) => { MovementVector = ctx.ReadValue<Vector2>(); };
        _controls.Player.Look.performed += (ctx) => { LookVectorDelta = ctx.ReadValue<Vector2>(); };
        _controls.Player.Scroll.performed += (ctx) => { ScrollDelta = ctx.ReadValue<Vector2>().y; };

    }

    private void OnSelectionChanged(GameObject newObj, GameObject prevObj) // UI Selection Callback
    {
        if (newObj == null) return;
        switch (InputMode)
        {
            case InputModes.MenuUI:
                if (newObj.GetComponent<Selectable>() != null)
                {
                    newObj.GetComponent<Selectable>().Select();
                }
                break;
            case InputModes.PauseMenuUI:
                if (newObj.GetComponent<Selectable>() != null)
                {
                    newObj.GetComponent<Selectable>().Select();
                }
                break;
            default:
                break;
        }
    }

    private static void UIModeChanged(bool value)
    {
        if (value)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Instance._controls.UI.Enable();
            return;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Instance._controls.UI.Disable();
    }
    public void SetInputMode(int inputMode) => InputMode = (InputModes)inputMode;
    private void InputModeChanged(InputModes oldMode, InputModes newMode)
    {
        _controls.Player.Disable();
        MenuUI.SetActive(false);
        GameUI.SetActive(false);
        PauseMenuUI.SetActive(false);
        MovementVector = Vector2.zero;
        LookVectorDelta = Vector2.zero;
        switch (newMode)
        {
            case InputModes.MenuUI:
                MenuUI.SetActive(true);
                SetControls = MenuControls;
                break;
            case InputModes.PauseMenuUI:
                PauseMenuUI.SetActive(true);
                SetControls = PauseMenuControls;
                break;
            case InputModes.Player:
                _controls.Player.Enable();
                GameUI.SetActive(true);
                SetControls = PlayerControls;
                break;
            case InputModes.DeathMenuUI:
                DeathMenuUI.SetActive(true);
                SetControls = DeathMenuControls;
                break;
        }
        Debug.Log($"[UIM] Input Mode changed from {oldMode} to {newMode}");
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Added:
                GetComponent<PlayerInput>().SwitchCurrentControlScheme(device);
                Debug.Log($"[UIM] Device Added: {device.name}");
                break;
            case InputDeviceChange.Reconnected:
                Debug.Log($"[UIM] Device Reconnected: {device.name}");
                break;
            case InputDeviceChange.Disconnected:
                Debug.Log($"[UIM] Device Disconnected: {device.name}");
                if (Gamepad.all.Count == 0)
                {
                    GetComponent<PlayerInput>().SwitchCurrentControlScheme("Keyboard And Mouse");
                }
                break;
        }
    }

    private void OnControlsChanged(PlayerInput input)
    {
        MovementVector = Vector2.zero;
        LookVectorDelta = Vector2.zero;
        switch (input.currentControlScheme)
        {
            case "Keyboard And Mouse":
                _controller = Controller.Keyboard;
                break;
        }
        var bindingGroup = _controls?.controlSchemes.First(x => x.name == input.currentControlScheme).bindingGroup;
        if (bindingGroup != null) _controls.bindingMask = InputBinding.MaskByGroup(bindingGroup);
        Debug.Log($"[UIM] Control scheme changed to {input.currentControlScheme}");
    }

    private void Update() => SetControls?.Invoke();

    private void MenuControls()
    {

    }
    private void DeathMenuControls()
    {

    }

    private void PauseMenuControls()
    {
        if (_controls.UI.Cancel.triggered)
        {
            SetInputMode((int)InputModes.Player);
        }
    }

    private void PlayerControls()
    {
        if (_playerController == null) return;
        if (_controls.Player.Run.triggered)
        {
            if (_controls.Player.Run.phase == InputActionPhase.Performed)
            {
                _playerController.SetInput(PlayerInputs.Run, InputValue.Down);
            }
            else if (_controls.Player.Run.phase == InputActionPhase.Waiting)
            {
                _playerController.SetInput(PlayerInputs.Run, InputValue.Up);
            }
        }
        if (_controls.Player.Jump.triggered)
        {
            if (_controls.Player.Jump.phase == InputActionPhase.Performed)
            {
                _playerController.SetInput(PlayerInputs.Jump, InputValue.Down);
            }
            else if (_controls.Player.Jump.phase == InputActionPhase.Waiting)
            {
                _playerController.SetInput(PlayerInputs.Jump, InputValue.Up);
            }
        }
        if (_controls.Player.CrouchToggle.triggered)
        {
            if (_controls.Player.CrouchToggle.phase == InputActionPhase.Performed)
            {
                _playerController.SetInput(PlayerInputs.Crouch, InputValue.Toggle);
            }
        }
        if (_controls.Player.Crouch.triggered)
        {
            if (_controls.Player.Crouch.phase == InputActionPhase.Performed)
            {
                _playerController.SetInput(PlayerInputs.Crouch, InputValue.Down);
            }
            else if (_controls.Player.Crouch.phase == InputActionPhase.Waiting)
            {
                _playerController.SetInput(PlayerInputs.Crouch, InputValue.Up);
            }
        }

        //Mouse Primary Click
        if (_controls.Player.PrimaryButton.triggered)
        {
            if (_controls.Player.PrimaryButton.phase == InputActionPhase.Performed)
            {

            }

            if (_controls.Player.PrimaryButton.phase == InputActionPhase.Waiting) // Button Up
            {

            }
        }

        //Mouse Secondary Click
        if (_controls.Player.SecondaryButton.triggered)
        {
            if (_controls.Player.SecondaryButton.phase == InputActionPhase.Performed) // Button Down
            {
                _playerController.SetInput(PlayerInputs.CamMode, InputValue.Down);
            }

            if (_controls.Player.SecondaryButton.phase == InputActionPhase.Waiting) // Button Up
            {
                _playerController.SetInput(PlayerInputs.CamMode, InputValue.Up);
            }

        }
        if (_controls.Player.Reload.triggered)
        {
            if (_controls.Player.Reload.phase == InputActionPhase.Performed) // Button Down
            {
                // code for reloading
            }

        }

        if (_controls.Player.GetItem.triggered)
        {
            if (_controls.Player.GetItem.phase == InputActionPhase.Performed) // Button Down
            {
                //code for picking up item
            }
        }

        if (_controls.Player.Drop.triggered)
        {
            if (_controls.Player.Drop.phase == InputActionPhase.Performed) // Button Down
            {
                //code for dropping item
            }
        }

        if (_controls.Player.Slot1.triggered)
        {
            if (_controls.Player.Slot1.phase == InputActionPhase.Performed) // Button Down
            {
                //TODO : Select Slot 1
            }
        }

        if (_controls.Player.Slot2.triggered)
        {
            if (_controls.Player.Slot2.phase == InputActionPhase.Performed) // Button Down
            {
                //TODO : Select Slot 2
            }
        }

        if (_controls.Player.Slot3.triggered)
        {
            if (_controls.Player.Slot3.phase == InputActionPhase.Performed) // Button Down
            {
                //TODO : Select Slot 3
            }
        }

        if (ScrollDelta > 0)
        {
            //Mouse Scroll Up

        }
        else if (ScrollDelta < 0)
        {
            //Mouse Scroll Down
        }
        ScrollDelta = 0;


        if (_controls.Player.NextSlot.triggered)
        {
            //Next slot
        }
        else if (_controls.Player.PreviousSlot.triggered)
        {
            //Previous slot
        }

        if (_controls.Player.GodMode.triggered)
        {
            if (_controls.Player.GodMode.phase == InputActionPhase.Performed) // Button Down
            {
                _playerController.ToggleGodMode();
            }
        }

        if (_controls.Player.PauseMenu.triggered)
        {
            SetInputMode((int)InputModes.PauseMenuUI);
        }

        if (_controls.Player.CameraMode.triggered)
        {
            _playerController.SetInput(PlayerInputs.CamMode, InputValue.Toggle);
        }
    }

    public static bool UIMode
    {
        get { return _cursorBool; }
        set
        {
            if (value != _cursorBool) _cursorBool = value;
            UIModeChanged(value);
        }
    }

    public InputModes InputMode
    {
        get { return _inputMode; }
        set
        {
            InputModeChanged(_inputMode, value);
            _inputMode = value;
        }
    }

    public float SensitivityX()
    {
        return MouseSensitivityX / 10;
    }
    public float SensitivityY()
    {
        return MouseSensitivityY / 10;
    }

}
