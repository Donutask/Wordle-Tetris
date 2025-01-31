using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

namespace Donutask.Wordfall
{
    public class Cheats : MonoBehaviour
    {
        public static bool wereCheatsUsed { get; private set; }
        readonly bool allowCheats = false;

        [SerializeField] TextMeshProUGUI hintText;
        [SerializeField] InputAction hintAction;
        [SerializeField] InputAction spawnAction;

        void Start()
        {
            hintAction.Enable();
            spawnAction.Enable();
            wereCheatsUsed = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (!allowCheats)
            {
                return;
            }
            if (!StartScreen.started || GameOver.gameOver || BlankLetterChooser.choosingLetter || PauseManager.paused)
            {
                return;
            }

            if (hintAction.IsPressed())
            {
                wereCheatsUsed = true;
                hintText.text = LetterSpawner.currentWordUnshuffled;
            }
            else
            {
                hintText.text = "";
            }

            if (spawnAction.WasPerformedThisFrame())
            {
                wereCheatsUsed = true;
                LetterSpawner.OverwriteLetter('!');
            }
        }
    }
}
