using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> 
{
    private GameObject levelPanel;
    private GameObject failurePanel;
    private GameObject successPanel;

    private void Start()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    public void OnPlayerSuccess()
    {

    }

    public void OnPlayerFail()
    {

    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name.StartsWith("Level"))
        {
            levelPanel = GameObject.Find("LevelPanel");
            failurePanel = GameObject.Find("FailurePanel");
            successPanel = GameObject.Find("SuccessPanel");
        }
        else
        {
            levelPanel = failurePanel = successPanel = null;
        }
    }
}
