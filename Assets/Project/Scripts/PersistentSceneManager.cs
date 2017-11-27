using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PersistentSceneManager : MonoBehaviour {
    string previousScene;

    public delegate void SceneLoadCompleted();
    public event SceneLoadCompleted SceneLoadComplete;

	void Start () {
        LoadLevel("Config");
	}

    public void LoadLevel (string name)
    {
         StartCoroutine(LoadSceneAsync(name));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        if (previousScene != null)
        {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(previousScene);
            while (!asyncUnload.isDone)
            {
                yield return null;
            }
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        if (SceneLoadComplete != null)
        {
            SceneLoadComplete();
        }
        previousScene = sceneName;
    }
}
