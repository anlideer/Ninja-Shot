using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> 
{
    public UnityEvent<float, float> LevelSuccess;   // passTime, recordTime
    public UnityEvent LevelFail;

    public bool GamePaused { get; private set; }
    public bool GameEnded { get; set; }

    public void OnPlayerSuccess()
    {
        float levelPassTime = Time.timeSinceLevelLoad;
        string sceneName = SceneManager.GetActiveScene().name;
        float recordTime = PlayerPrefs.GetFloat(sceneName, -1f); 
        if (recordTime == -1f || levelPassTime < recordTime)
        {
            recordTime = levelPassTime;
            PlayerPrefs.SetFloat(SceneManager.GetActiveScene().name, levelPassTime);
        }

        LevelSuccess?.Invoke(levelPassTime, recordTime);
        GameEnded = true;
    }

    public void OnPlayerFail()
    {
        LevelFail?.Invoke();
        GameEnded = true;
    }

    public void PauseGame()
    {
        GamePaused = true;
        Time.timeScale = 0f;
    }

    public void UnpauseGame()
    {
        GamePaused = false;
        Time.timeScale = 1f;
    }
}
