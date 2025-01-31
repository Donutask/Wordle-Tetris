using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace Donutask.Wordfall
{
    public class GameOver : MonoBehaviour
    {
        public static GameOver Instance;
        private void Awake()
        {
            Instance = this;
            gameOver = false;
        }

        [SerializeField] GameObject normalUI, gameOverUI;
        [SerializeField] ParticleSystem particles;
        [SerializeField] TextMeshProUGUI titleText, scoreText, highScoreText, cheatIndicationText;
        public static bool gameOver { get; private set; }

        public void EndGame()
        {
            gameOver = true;
            normalUI.SetActive(false);
            gameOverUI.SetActive(true);

            //If you manage to get max score, there's an easter egg
            if (WordChecker.score >= int.MaxValue)
            {
                titleText.text = "You Win!";
            }
            else
            {
                titleText.text = "Game Over";
            }

            //Calculate and show high score
            int highScore = PlayerPrefs.GetInt("HighScore");
            if (WordChecker.score > highScore && !Cheats.wereCheatsUsed)
            {
                highScoreText.text = "New High Score!";
                PlayerPrefs.SetInt("HighScore", WordChecker.score);
                PlayerPrefs.Save();

                particles.transform.position = new Vector3(2, 0);
                particles.Play();

                AudioManager.instance.Play("New High Score");
            }
            else
            {
                highScoreText.text = "High Score: " + highScore;
                AudioManager.instance.Play("Lose");
            }
            scoreText.text = "Score: " + WordChecker.score;

            //Show if used secret button to show what word its giving you
            cheatIndicationText.text = Cheats.wereCheatsUsed ? "Cheats Used" : "";


            ControlsManager.startEvent.AddListener(PlayAgain);

            MusicManager.FadeOutMusic();
        }

        bool loadingScene;
        public void PlayAgain()
        {
            if (loadingScene)
            {
                return;
            }

            loadingScene = true;

            SceneLoadingTransition.Instance.LoadScene("Game", UnityEngine.UI.Slider.Direction.BottomToTop);

            //SceneManager.LoadScene(0);
        }
    }
}