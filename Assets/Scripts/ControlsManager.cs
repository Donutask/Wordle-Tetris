using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class ControlsManager : MonoBehaviour
{
    [SerializeField] InputAction moveAction;
    [SerializeField] InputAction dropAction;
    [SerializeField] InputAction storeAction;
    [SerializeField] InputAction startAction;

    public static UnityEvent storeEvent;
    public static UnityEvent startEvent;

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
    }
}
