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
        [SerializeField] TextMeshProUGUI scoreText, highScoreText, cheatIndicationText;
        [SerializeField] AudioSource music;
        public static bool gameOver { get; private set; }

        public void EndGame()
        {
            gameOver = true;
            normalUI.SetActive(false);
            gameOverUI.SetActive(true);

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
            cheatIndicationText.text = Cheats.wereCheatsUsed ? "Hints Used" : "";


            ControlsManager.startEvent.AddListener(PlayAgain);

            //Fade out music
            StartCoroutine(StartFade(music, 2.5f, 0));
        }

        bool loadingScene;
        public void PlayAgain()
        {
            if (loadingScene)
            {
                return;
            }

            loadingScene = true;
            SceneManager.LoadScene(0);
        }

        static IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
        {
            float currentTime = 0;
            float start = audioSource.volume;
            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
                yield return null;
            }
            yield break;
        }
    }
}