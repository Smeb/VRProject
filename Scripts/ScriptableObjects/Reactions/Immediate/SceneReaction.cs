using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneReaction : Reaction
{
    public string sceneName;

    private SceneController sceneController;
 
    protected override void InitOverride()
    {
        sceneController = FindObjectOfType<SceneController>();
    }

    protected override void ImmediateReaction()
    {
        sceneController.FadeAndLoadScene(this);
    }
}
