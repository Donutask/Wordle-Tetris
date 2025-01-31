using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class SceneLoadingTransition : MonoBehaviour
{
    public static SceneLoadingTransition Instance { get; private set; }
    [SerializeField] Slider slider;
    [SerializeField] float transitionDuration;
    [SerializeField] AudioClip slideIn, slideOut;
    [SerializeField] AudioSource audioSource;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }


    bool loadingScene;
    public void LoadScene(string scene, Slider.Direction direction)
    {
        if (loadingScene)
        {
            return;
        }
        loadingScene = true;

        AsyncOperation op = SceneManager.LoadSceneAsync(scene);
        op.allowSceneActivation = false;

        slider.direction = direction;
        StartCoroutine(Transition(op));
    }

    IEnumerator Transition(AsyncOperation asyncOperation)
    {
        float currentTime = 0;
        slider.value = 0;

        audioSource.PlayOneShot(slideIn);

        while (currentTime < transitionDuration)
        {
            currentTime += Time.deltaTime;
            //slider.value = Mathf.Lerp(0, 1, currentTime / transitionDuration);
            slider.value = InSine(currentTime / transitionDuration);
            yield return null;
        }

        asyncOperation.allowSceneActivation = true;
        yield return new WaitUntil(() => asyncOperation.isDone == true);

        currentTime = 0;
        slider.value = 1;

        audioSource.PlayOneShot(slideOut);

        while (currentTime < transitionDuration)
        {
            currentTime += Time.deltaTime;
            slider.value = 1 - OutSine(currentTime / transitionDuration);
            yield return null;
        }

        loadingScene = false;
    }

    public static float InSine(float t) => 1 - (float)Mathf.Cos(t * Mathf.PI / 2);
    public static float OutSine(float t) => (float)Mathf.Sin(t * Mathf.PI / 2);
}
