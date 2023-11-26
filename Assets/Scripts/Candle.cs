using System.Collections;
using System.Collections.Generic;
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

    private void LightUp()
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
    }

    private void SetChildrenVisibility(bool visible)
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(visible);
        }
    }
}
