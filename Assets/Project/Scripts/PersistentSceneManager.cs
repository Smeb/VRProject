using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PersistentSceneManager : MonoBehaviour {
    string previousScene;
    public string startScene = "Config";

	void Start () {
        LoadLevel(startScene);
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
        previousScene = sceneName;
    }
}
