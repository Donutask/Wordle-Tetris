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
            ControlsManager.pauseEvent.AddListener(TogglePause);
        }

        void TogglePause()
        {
            paused = !paused;

            Time.timeScale = paused ? 0 : 1;

            pauseCanvas.enabled = paused;
        }
    }
}
