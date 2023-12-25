using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelTimeRecorder : MonoBehaviour
{
    private void Update()
    {
        GetComponent<TextMeshProUGUI>().text = string.Format("{0:N2}s", Time.timeSinceLevelLoad);
    }
}
