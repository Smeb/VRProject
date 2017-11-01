using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public event Action BeforeSceneUnload;
    public event Action AfterSceneLoad;

    public CanvasGroup faderCanvasGroup;
    public float fadeDuration = 1f;
    public string startSceneName = "startScene";

    private bool isFading;

    private IEnumerator Start()
    {
        faderCanvasGroup.alpha = 1f;
        yield break;
    }

    public void FadeAndLoadScene(SceneReaction sceneReaction)
    {

    }

    private IEnumerator LoadSceneAsync(string name)
    {
        yield return SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);

        Scene loadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        SceneManager.SetActiveScene(loadedScene);
    }

    private IEnumerator FadeAndSwitchScene(string name)
    {
        yield return StartCoroutine(Fade(1f));

        if (BeforeSceneUnload != null)
        {
            BeforeSceneUnload();
        }

        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        yield return StartCoroutine(LoadSceneAsync(name));

        if (AfterSceneLoad != null)
        {
            AfterSceneLoad();
        }

        yield return StartCoroutine(Fade(0f));
    }

    private IEnumerator Fade(float targetAlpha)
    {
        isFading = true;
        faderCanvasGroup.blocksRaycasts = true;

        float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - targetAlpha) / fadeDuration;

        while (!Mathf.Approximately(faderCanvasGroup.alpha, targetAlpha))
        {
            faderCanvasGroup.alpha =
                Mathf.MoveTowards(faderCanvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }

        faderCanvasGroup.blocksRaycasts = false;
        isFading = false;
    }
}
