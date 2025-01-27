using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

/// <summary>
/// Detect when changed to controller or keyboard and mous.
/// Different script so as to persist between scenes for the how-to-play screen.
/// </summary>
namespace Donutask.Wordfall
{
    public class ControllerChange : MonoBehaviour
    {
        public static bool usingGamepad
        {
            get
            {
                return Gamepad.all.Count > 0;
            }
        }
        public static UnityEvent<bool> gamepadChangeEvent;
        static bool ready;

        private void Awake()
        {
            if (!ready)
            {
                InputSystem.onDeviceChange += InputSystem_onDeviceChange;
                gamepadChangeEvent = new UnityEvent<bool>();

                DontDestroyOnLoad(gameObject);
                ready = true;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        string prevName;
        private void InputSystem_onDeviceChange(InputDevice arg1, InputDeviceChange arg2)
        {
            string name = arg1.displayName;
            if (name == prevName)
            {
                //duplicate, don't spam with events
            }
            else
            {
                gamepadChangeEvent.Invoke(usingGamepad);
            }
        }
    }
}