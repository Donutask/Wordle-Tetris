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
    public static readonly float transitionDuration = 0.2f;
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

    public void ShowTransition(Slider.Direction direction, GameObject screen)
    {
        slider.direction = direction;
        StartCoroutine(Transition(null, screen));
    }

    /// <summary>
    /// either load scene or show gameobject
    /// </summary>
    IEnumerator Transition(AsyncOperation asyncOperation, GameObject obj = null)
    {
        float currentTime = 0;
        slider.value = 0;

        audioSource.PlayOneShot(slideIn);

        while (currentTime < transitionDuration)
        {
            currentTime += Time.deltaTime;
            slider.value = InSine(currentTime / transitionDuration);
            yield return null;
        }

        if (asyncOperation != null)
        {
            asyncOperation.allowSceneActivation = true;
            yield return new WaitUntil(() => asyncOperation.isDone == true);
        }
        else if (obj != null)
        {
            yield return new WaitForSeconds(transitionDuration);
            obj.SetActive(!obj.activeSelf);
        }

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

    static float InSine(float t) => 1 - (float)Mathf.Cos(t * Mathf.PI / 2);
    static float OutSine(float t) => (float)Mathf.Sin(t * Mathf.PI / 2);
}
