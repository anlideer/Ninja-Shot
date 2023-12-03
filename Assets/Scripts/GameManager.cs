using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    public UnityEvent PlayerDiscovered;

    private void Start()
    {
        PlayerDiscovered.AddListener(OnPlayerDiscovered);
    }

    private void OnPlayerDiscovered()
    {
        Debug.Log("oho");
    }
}
