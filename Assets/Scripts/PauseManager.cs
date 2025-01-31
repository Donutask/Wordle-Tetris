using UnityEngine;
using UnityEngine.UI;

namespace Donutask.Wordfall
{
    public class PauseManager : MonoBehaviour
    {
        public static bool paused { get; private set; }
        [SerializeField] Canvas pauseCanvas;

        private void Start()
        {
            //only allow pausing after the game has started
            StartScreen.startGame.AddListener(delegate
            {
                ControlsManager.pauseEvent.AddListener(TogglePause);
            });
        }

        public void TogglePause()
        {
            //Can't pause when game isn't playing
            if (!StartScreen.started || GameOver.gameOver || BlankLetterChooser.choosingLetter)
            {
                return;
            }

            paused = !paused;

            Time.timeScale = paused ? 0 : 1;

            pauseCanvas.enabled = paused;
        }
    }
}
