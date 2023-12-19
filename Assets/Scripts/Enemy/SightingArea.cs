using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightingArea : MonoBehaviour
{
    [SerializeField] private float detectCandleDistance = 6f;
    [SerializeField] private LayerMask detectLayer;

    private FieldOfView fov;
    private float originalRadius;

    public float DetectCandleDistance { get { return detectCandleDistance; } }

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
            float currentRadius = detectCandleDistance;
            if (GetFarthestPoint(rayHit.collider, rayHit.point, out Vector3 farthestPoint))
            {
                currentRadius = Vector3.Distance(transform.position, farthestPoint);
            }

            fov.ViewRadius = Mathf.Max(currentRadius, originalRadius);
        }
        else
        {
            fov.ViewRadius = originalRadius;
        }

        fov.UpdateFOV();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
/*        var playerController = collision.gameObject.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.PlayerDiscovered();
        }*/
    }

    private bool GetFarthestPoint(Collider2D collider, Vector3 hitPoint, out Vector3 farthestPoint)
    {
        farthestPoint = Vector3.zero;
        Vector3 dir = hitPoint - transform.position;
        dir.Normalize();
        var circleCollider = collider.GetComponent<CircleCollider2D>();
        if (circleCollider == null)
            return false;

        Vector3 outsidePointOnRay = hitPoint + dir * circleCollider.radius * 2;
        Ray oppositeRay = new Ray(outsidePointOnRay, -dir);
        if (collider.bounds.IntersectRay(oppositeRay, out float dis))
        {
            farthestPoint = outsidePointOnRay - dir * dis;
            return true;
        }
        else
        {
            return false;
        }
    }
}
