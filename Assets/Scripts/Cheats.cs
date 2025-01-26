using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

namespace Donutask.Wordfall
{
    public class Cheats : MonoBehaviour
    {
        public static bool wereCheatsUsed { get; private set; }

        [SerializeField] TextMeshProUGUI hintText;
        [SerializeField] InputAction hintAction;

        void Start()
        {
            hintAction.Enable();
            wereCheatsUsed = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (hintAction.IsPressed())
            {
                wereCheatsUsed = true;
                hintText.text = LetterSpawner.currentWordUnshuffled;
            }
            else
            {
                hintText.text = "";
            }
        }
    }
}
