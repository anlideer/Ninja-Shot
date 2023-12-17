using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Candle : MonoBehaviour
{
    [SerializeField] private Color lightingColor;
    [SerializeField] private Color deadColor;

    private bool isLighting = true;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        isLighting = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            if (isLighting)
            {
                PutOut();
            }
        }
    }

    public void LightUp()
    {
        isLighting = true;
        spriteRenderer.color = lightingColor;
        SetChildrenVisibility(true);
    }

    private void PutOut()
    {
        isLighting = false;
        spriteRenderer.color = deadColor;
        SetChildrenVisibility(false);
        TryWarnEnemy();
    }

    private void TryWarnEnemy()
    {
        var enemies = FindObjectsByType(typeof(EnemyMovable), FindObjectsSortMode.None);
        List<EnemyMovable> availableList = new List<EnemyMovable>();
        foreach(var enemy in enemies)
        {
            EnemyMovable enemyMovable = enemy.GetComponent<EnemyMovable>();
            if (enemyMovable != null && enemyMovable.CanNoticeCandle(transform.position))
            {
                availableList.Add(enemyMovable);
            }
        }
        
        if (availableList.Count > 0)
        {
            int ind = 0;
            float minDis = Vector3.Distance(transform.position, availableList[0].transform.position);
            for (int i = 1; i < availableList.Count; i++)
            {
                float dis = Vector3.Distance(transform.position, availableList[i].transform.position);
                if (dis < minDis)
                {
                    ind = i;
                    minDis = dis;
                }
            }

            // notify this enemy to light me up
            availableList[ind].GoLightCandle(this);
        }
    }

    private void SetChildrenVisibility(bool visible)
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(visible);
        }
    }
}
