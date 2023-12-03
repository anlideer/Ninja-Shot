using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightingArea : MonoBehaviour
{
    [SerializeField] private float detectCandleDistance = 6f;
    [SerializeField] private LayerMask detectLayer;

    private FieldOfView fov;
    private float originalRadius;

    private void Start()
    {
        fov = GetComponent<FieldOfView>();
        originalRadius = fov.ViewRadius;
    }

    private void FixedUpdate()
    {
        RaycastHit2D rayHit = Physics2D.Raycast(
                transform.position,
                transform.right,
                detectCandleDistance,
                detectLayer);
        
        if (rayHit.collider != null && rayHit.collider.GetComponent<LightingArea>() != null)
        {
            fov.ViewRadius = detectCandleDistance;
        }
        else
        {
            fov.ViewRadius = originalRadius;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var playerController = collision.gameObject.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.PlayerDiscovered();
        }
    }
}
