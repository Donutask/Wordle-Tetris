using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace Donutask.Wordfall
{
    public class StartScreen : MonoBehaviour
    {
        [SerializeField] GameObject startUI, normalUI;
        public static UnityEvent startGame = new();

        private void Start()
        {
            startUI.SetActive(true);
            normalUI.SetActive(false);

            ControlsManager.startEvent.AddListener(Play);
        }

        bool playing = false;
        private void Play()
        {
            if (playing)
            {
                return;
            }

            playing = true;

            startUI.SetActive(false);
            normalUI.SetActive(true);

            startGame.Invoke();
        }

        public void OpenHelp()
        {
            SceneManager.LoadSceneAsync("How to Play");
        }
        public void OpenCredits()
        {
            SceneManager.LoadSceneAsync("Credits");
        }
    }
}