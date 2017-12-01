using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PersistentSceneManager : MonoBehaviour {
    string previousScene;
    public string startScene = "Config";
    public Material statueNormal;
    public Material statueHighlight;
    public Material supermarketNormal;
    public Material supermarketHighlight;
    public Material alternateNormal;
    public Material alternateHighlight;

    void Start () {
        LoadLevel(startScene);
        TextureController.statueNormal = statueNormal;
        TextureController.statueHighlight = statueHighlight;
        TextureController.supermarketNormal = supermarketNormal;
        TextureController.supermarketHighlight = supermarketHighlight;
        TextureController.alternateNormal = alternateNormal;
        TextureController.alternateHighlight = alternateHighlight;
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
