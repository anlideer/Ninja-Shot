using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashRefillDisplay : MonoBehaviour
{
    [SerializeField] private float refillRadius = 0.5f;
    [SerializeField] private GameObject firstRefillCirclePrefab;

    private PlayerController playerController;
    private float firstDashCD;
    private float secondDashCD;
    private LineRenderer firstDashCircle;

    private void Start()
    {
        playerController = transform.parent.GetComponent<PlayerController>();
        firstDashCD = playerController.FirstDashCD;
        secondDashCD = playerController.SecondDashCD;
        playerController.DashCompletedEvent.AddListener(OnDashCompleted);
        playerController.DashStartingEvent.AddListener(OnDashStarting);
    }

    private void OnDashCompleted()
    {
        StartCoroutine(DashRefillRoutine());
    }

    private void OnDashStarting()
    {
        StopAllCoroutines();

        if (firstDashCircle != null)
            Destroy(firstDashCircle.gameObject);

    }

    private IEnumerator DashRefillRoutine()
    {
        firstDashCircle = Instantiate(firstRefillCirclePrefab, transform).GetComponent<LineRenderer>();
        yield return StartCoroutine(DrawCircleRoutine(firstDashCircle, firstDashCD, 100, refillRadius));
    }

    private IEnumerator DrawCircleRoutine(LineRenderer circleRenderer, float duration, int targetTotalSteps, float radius)
    {

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            int currentStepTotal = Mathf.FloorToInt(Mathf.Lerp(0, targetTotalSteps, elapsedTime / duration));
            DrawCircle(circleRenderer, currentStepTotal, targetTotalSteps, radius);
            yield return null;
        }

        DrawCircle(circleRenderer, targetTotalSteps, targetTotalSteps, radius);
    }

    private void DrawCircle(LineRenderer circleRenderer, int currentStepsTotal, int totalSteps, float radius)
    {
        circleRenderer.positionCount = currentStepsTotal;

        for (int currentStep = 0; currentStep < currentStepsTotal; currentStep++)
        {
            float circumferenceProgress = (float)currentStep / (totalSteps - 1);

            float currentRadian = -circumferenceProgress * 2 * Mathf.PI;

            float xScaled = Mathf.Cos(currentRadian);
            float yScaled = Mathf.Sin(currentRadian);

            float x = radius * xScaled;
            float y = radius * yScaled;
            float z = 0;

            Vector3 currentPosition = new Vector3(x, y, z) + transform.position;

            circleRenderer.SetPosition(currentStep, currentPosition);
        }
    }
}
