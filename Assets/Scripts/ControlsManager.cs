using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace Donutask.Wordfall
{
    public class ControlsManager : MonoBehaviour
    {
        [SerializeField] InputAction moveAction;
        [SerializeField] InputAction dropAction;
        [SerializeField] InputAction storeAction;
        [SerializeField] InputAction startAction;
        [SerializeField] InputAction pauseAction;
        [SerializeField] InputAction tempMusicAction;
        [SerializeField] AudioSource music;

        public static UnityEvent storeEvent;
        public static UnityEvent startEvent;
        public static UnityEvent pauseEvent;

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

            tempMusicAction.Enable();
            tempMusicAction.performed += TempMusicAction_performed;
        }

        private void TempMusicAction_performed(InputAction.CallbackContext obj)
        {
            music.enabled = !music.enabled;
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
}