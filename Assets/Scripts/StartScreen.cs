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
        public static bool started { get; private set; }
        [SerializeField] GameObject startUI, normalUI, loadingIndicator, startHint;
        public static UnityEvent startGame = new();

        private void Start()
        {
            started = false;

            startUI.SetActive(true);
            normalUI.SetActive(false);

            if (WordManager.wordList == null)
            {
                loadingIndicator.SetActive(true);
                startHint.SetActive(false);

                WordManager.onWordListsLoaded.AddListener(delegate
                {
                    loadingIndicator.SetActive(false);
                    startHint.SetActive(true);
                });
            }
            else
            {
                loadingIndicator.SetActive(false);
                startHint.SetActive(true);

            }

            StartCoroutine(WaitSlightlyBeforeAllowingStart());


        }

        //Otherwise pressing space to go to main menu will instantly start the game
        IEnumerator WaitSlightlyBeforeAllowingStart()
        {
            yield return new WaitUntil(() => WordManager.wordList != null);
            yield return new WaitForSeconds(0.25f);

            ControlsManager.startEvent.AddListener(Play);
        }

        private void Play()
        {
            if (started)
            {
                return;
            }

            started = true;

            startUI.SetActive(false);
            normalUI.SetActive(true);

            startGame.Invoke();
        }


        public void OpenHelp()
        {
            LoadScene("How to Play");
        }
        public void OpenCredits()
        {
            LoadScene("Credits");
        }

        bool loadingScene;
        void LoadScene(string scene)
        {
            if (loadingScene)
            {
                return;
            }
            loadingScene = true;

            SceneManager.LoadSceneAsync(scene);
        }

        private void Update()
        {
            if (loadingScene || started || GameOver.gameOver)
            {
                return;
            }
            //Allow opening the menus with controller or keyboard
            int xMovement = Mathf.RoundToInt(ControlsManager.GetLetterMovement().x);
            if (xMovement == -1)
            {
                OpenHelp();
            }
            else if (xMovement == 1)
            {
                OpenCredits();
            }
        }
    }
}