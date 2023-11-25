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
    private PlayerControls playerControls;
    private PlayerController playerController;
    private Vector3 targetPosition;
    private float currentRadius;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void Start()
    {
        playerController = transform.parent.GetComponent<PlayerController>();
        targetPosition = Camera.main.ScreenToWorldPoint(playerControls.TargetPosition.Pos.ReadValue<Vector2>());
        targetPosition.z = 0;
    }

    private void OnEnable()
    {
        playerControls?.Enable();
    }

    private void OnDisable()
    {
        playerControls?.Disable();
    }

    private void FixedUpdate()
    {
        targetPosition = Camera.main.ScreenToWorldPoint(playerControls.TargetPosition.Pos.ReadValue<Vector2>());
        targetPosition.z = 0;

        if (playerController.CanDash)
        {
            if (circleRenderer == null)
            {
                // TODO: target position should be calculated, not directly mouse pos
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

    void DrawCircle(Vector3 centerPoint, int steps, float radius)
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
