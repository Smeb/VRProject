using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PersistentSceneManager : MonoBehaviour {
    // Use this for initialization
    int BATCH_SIZE = 100;

	void Start () {
        StartCoroutine(LoadSceneAsync("TestSpace"));
	}

    IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while(!asyncLoad.isDone)
        {
            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

        StartCoroutine(AttachScriptsToObjectInLayer("Grabbable"));
    }

    IEnumerator AttachScriptsToObjectInLayer(string layerName)
    {
        GameObject[] gameObjects = FindObjectsOfType<GameObject>();

        for(int i = 0; i < gameObjects.Length; i++)
        {
            if(gameObjects[i].layer == LayerMask.NameToLayer(layerName))
            {
                gameObjects[i].AddComponent<InteractableObject>();
            }
            if (i % BATCH_SIZE == 0) yield return null;
        }
    }
}
