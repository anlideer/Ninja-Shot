using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingLine : MonoBehaviour
{
    [SerializeField]
    private float growthSpeed = 1f;

    private LineRenderer lineRenderer;
    private float lineLength = 0f;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 3;
        float width = lineRenderer.startWidth;
        lineRenderer.material.mainTextureScale = new Vector2(1f / width, 1.0f);
    }

    private void Update()
    {
        Vector3 targetPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPoint.z = 0;

        // Update the line length based on the growth speed
        lineLength += Time.deltaTime * growthSpeed;

        // If the line has reached or exceeded the target length, reset it
        if (lineLength >= Vector3.Distance(transform.position, targetPoint))
        {
            lineLength = 0f;
        }

        // Update the line positions
        Vector3 lineDirection = (targetPoint - transform.position).normalized;
        Vector3 lineEndPoint = transform.position + lineDirection * lineLength;

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, (transform.position + lineEndPoint) / 2);
        lineRenderer.SetPosition(2, lineEndPoint);
    }
}
