using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LevelSnippet : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private TextMeshProUGUI recordText;
    [SerializeField] private string levelToLoad;
    [SerializeField] private string levelDisplayName;

    private void Start()
    {
        levelNameText.text = levelDisplayName;
    }

    private void OnEnable()
    {
        float recordTime = PlayerPrefs.GetFloat(levelToLoad, -1f);
        if (recordTime > 0f)
        {
            recordText.text = string.Format("{0:N2}s", recordTime);
        }
        else
        {
            recordText.text = "None";
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SceneManager.LoadScene(levelToLoad);
    }
}
