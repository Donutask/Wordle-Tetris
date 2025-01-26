using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class ControlsManager : MonoBehaviour
{
    public static bool usingGamepad
    {
        get
        {
            return Gamepad.all.Count > 0;
        }
    }
    [SerializeField] InputAction moveAction;
    [SerializeField] InputAction dropAction;
    [SerializeField] InputAction storeAction;
    [SerializeField] InputAction startAction;
    [SerializeField] InputAction pauseAction;

    public static UnityEvent storeEvent;
    public static UnityEvent startEvent;
    public static UnityEvent pauseEvent;
    public static UnityEvent<bool> gamepadChangeEvent;

    static ControlsManager Instance;
    void Awake()
    {
        Instance = this;
        moveAction.Enable();
        dropAction.Enable();

        storeAction.Enable();
        storeEvent = new UnityEvent();

        startAction.Enable();
        startEvent = new UnityEvent();

        pauseAction.Enable();
        pauseEvent = new UnityEvent();

        InputSystem.onDeviceChange += InputSystem_onDeviceChange;
        gamepadChangeEvent = new UnityEvent<bool>();
    }


    string prevName;
    private void InputSystem_onDeviceChange(InputDevice arg1, InputDeviceChange arg2)
    {
        string name = arg1.displayName;
        if (name == prevName)
        {
            //duplicate
        }
        else
        {
            Debug.Log(name);
            gamepadChangeEvent.Invoke(usingGamepad);
        }
    }

    public static Vector2 GetLetterMovement()
    {
        return Instance.moveAction.ReadValue<Vector2>();
    }
    public static bool IsDropping()
    {
        return Instance.dropAction.inProgress;
    }

    private void Update()
    {
        if (storeAction.WasPerformedThisFrame())
        {
            storeEvent.Invoke();
        }

        if (startAction.WasPerformedThisFrame())
        {
            startEvent.Invoke();
        }

        if (pauseAction.WasPerformedThisFrame())
        {
            pauseEvent.Invoke();
        }
    }
}
