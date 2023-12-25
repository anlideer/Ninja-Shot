using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelResultPanel : MonoBehaviour
{
    [SerializeField] private GameObject levelPanel;
    [SerializeField] private GameObject successPanel;
    [SerializeField] private GameObject failurePanel;
    [SerializeField] private TextMeshProUGUI passTimeText;
    [SerializeField] private TextMeshProUGUI recordTimeText;

    private PlayerControls playerControls;
    private bool isShowingResult;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void Start()
    {
        GameManager.Instance.LevelSuccess.AddListener(OnLevelSuccess);
        GameManager.Instance.LevelFail.AddListener(OnLevelFail);
        playerControls.Menu.Pause.performed += Pause_performed;
        GameManager.Instance.UnpauseGame();
        GameManager.Instance.GameEnded = false;
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GotoLevelPanel()
    {
        levelPanel.SetActive(true);
        successPanel.SetActive(false);
        failurePanel.SetActive(false);
    }

    private void OnLevelSuccess(float passTime, float recordTime)
    {
        if (isShowingResult)
            return;

        isShowingResult = true;
        successPanel.SetActive(true);

        passTimeText.text = string.Format("{0:N2}s", passTime);
        if (passTime == recordTime)
        {
            recordTimeText.text = "New record!";
        }
        else
        {
            recordTimeText.text = string.Format("Record time: {0:N2}s", recordTime);
        }
    }

    private void OnLevelFail()
    {
        if (isShowingResult)
            return;

        isShowingResult = true;
        failurePanel.SetActive(true);
    }

    private void OnEnable()
    {
        playerControls?.Enable();
    }

    private void OnDisable()
    {
        playerControls?.Disable();
    }

    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (isShowingResult)
            return;

        if (GameManager.Instance.GamePaused)
        {
            GameManager.Instance.UnpauseGame();
            levelPanel?.SetActive(false);
        }
        else
        {
            GameManager.Instance.PauseGame();
            levelPanel?.SetActive(true);
        }
    }
}
