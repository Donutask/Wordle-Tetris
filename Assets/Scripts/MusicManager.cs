using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Donutask.Wordfall
{
    public class MusicManager : MonoBehaviour
    {
        static MusicManager Instance;
        [SerializeField] AudioSource music;

        void Start()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                StartScreen.startGame.AddListener(FadeInMusic);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public static void FadeOutMusic()
        {
            Instance.m_FadeOutMusic();
        }

        void m_FadeOutMusic()
        {
            StartCoroutine(StartFade(music, 2.5f, 0));
        }


        void FadeInMusic()
        {
            StartCoroutine(StartFade(music, 2.5f, 1));
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
