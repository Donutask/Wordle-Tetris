using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

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

            StartCoroutine(WaitSlightlyBeforeAllowingStart());
        }

        //Otherwise pressing space to go to main menu will instantly start the game
        IEnumerator WaitSlightlyBeforeAllowingStart()
        {
            yield return new WaitForSeconds(0.25f);
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