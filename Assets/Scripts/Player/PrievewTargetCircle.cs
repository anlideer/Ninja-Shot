using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PrievewTargetCircle : MonoBehaviour
{
    [SerializeField] private GameObject simpleCircle;
    [SerializeField] private int circleSteps = 100;
    [SerializeField] private float maxRadius = 0.5f;
    [SerializeField] private float minRadius = 0.1f;
    [SerializeField] private float radiusDeductionSpeed = 0.05f;

    private LineRenderer circleRenderer;
    private PlayerController playerController;
    private Vector3 targetPosition;
    private float currentRadius;

    private void Start()
    {
        playerController = transform.parent.GetComponent<PlayerController>();
        targetPosition = playerController.TargetPosition;
    }

    private void FixedUpdate()
    {
        targetPosition = playerController.TargetPosition;

        if (playerController.CanDash)
        {
            if (circleRenderer == null)
            {
                circleRenderer = Instantiate(simpleCircle, targetPosition, Quaternion.identity).
                    GetComponent<LineRenderer>();
                currentRadius = maxRadius;
            }

            UpdateCircle();
        }
        else
        {
            if (circleRenderer != null)
            {
                Destroy(circleRenderer.gameObject);
            }
        }
    }

    private void UpdateCircle()
    {
        currentRadius -= radiusDeductionSpeed * Time.deltaTime;
        if (currentRadius < minRadius)
        {
            currentRadius = maxRadius;
        }

        DrawCircle(targetPosition, circleSteps, currentRadius);
    }

    private void DrawCircle(Vector3 centerPoint, int steps, float radius)
    {
        if (circleRenderer == null) { return; }
        
        circleRenderer.positionCount = steps;

        for (int currentStep = 0; currentStep < steps; currentStep++)
        {
            float circumferenceProgress = (float)currentStep / (steps - 1);

            float currentRadian = circumferenceProgress * 2 * Mathf.PI;

            float xScaled = Mathf.Cos(currentRadian);
            float yScaled = Mathf.Sin(currentRadian);

            float x = radius * xScaled;
            float y = radius * yScaled;
            float z = 0;

            Vector3 currentPosition = new Vector3(x, y, z) + centerPoint;

            circleRenderer.SetPosition(currentStep, currentPosition);
        }
    }
}
