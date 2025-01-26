using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Donutask.Wordfall
{
    public class ControllerHint : MonoBehaviour
    {
        [SerializeField] GameObject gamepadHint, keyboardHint;

        private void Start()
        {
            //Force update at start
            lastState = null;
            UpdateHint(ControllerChange.usingGamepad);

            ControllerChange.gamepadChangeEvent.AddListener(UpdateHint);
        }

        bool? lastState;
        void UpdateHint(bool usingGamepad)
        {
            if (usingGamepad == lastState)
            {
                return;
            }
            lastState = usingGamepad;

            if (usingGamepad)
            {
                if (gamepadHint != null)
                    gamepadHint.SetActive(true);
                if (keyboardHint != null)
                    keyboardHint.SetActive(false);
            }
            else
            {
                if (gamepadHint != null)
                    gamepadHint.SetActive(false);
                if (keyboardHint != null)
                    keyboardHint.SetActive(true);
            }

            //Debug.Log("Update controller hint");
        }
    }
}