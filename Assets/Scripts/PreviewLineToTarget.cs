using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewLineToTarget : MonoBehaviour
{
    [SerializeField] private GameObject simpleLine;
    [SerializeField] private float growingSpeed = 1f;
    [SerializeField] private float singleLineLength = 1f;
    [SerializeField] private float lineInterval = 0.5f;
    [SerializeField] private float dontDrawDistance = 1f;

    private LineRenderer currentLineRenderer;
    private float lineLength = 0f;
    private int currentLineIndex = 0;
    private List<LineRenderer> lineCollection = new List<LineRenderer>();
    private PlayerControls playerControls;
    private PlayerController playerController;
    private bool lineStatusCleared;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void Start()
    {
        playerController = transform.parent.GetComponent<PlayerController>();
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
        if (playerController.CanDash)
        {
            UpdateLines();
        }
        else
        {
            ResetLineStatus();
        }
    }

    private void UpdateLines()
    {
        lineStatusCleared = false;
        Vector3 targetPoint = Camera.main.ScreenToWorldPoint(playerControls.TargetPosition.Pos.ReadValue<Vector2>());
        targetPoint.z = 0;
        float distance = Vector3.Distance(transform.position, targetPoint);

        // if distance is too small, don't draw
        if (distance < dontDrawDistance)
        {
            ResetLineStatus();
            return;
        }

        lineLength += Time.fixedDeltaTime * growingSpeed;
        if (lineLength >= distance)
        {
            ResetLineStatus();
        }

        if (currentLineRenderer == null)
        {
            AddOneLine();
        }

        UpdatePreviousLines(targetPoint);
        UpdateCurrentLine(targetPoint);
    }

    private void UpdatePreviousLines(Vector3 targetPoint)
    {
        Vector3 lineDirection = (targetPoint - transform.position).normalized;

        for (int i = 0; i < lineCollection.Count; i++)
        {
            if (i != currentLineIndex)
            {
                var line = lineCollection[i];
                var startPoint = transform.position + lineDirection * i * (singleLineLength + lineInterval);
                SetLinePosition(line, singleLineLength, startPoint, lineDirection);
            }
        }
    }

    private void UpdateCurrentLine(Vector3 targetPoint)
    {
        float neededLength = lineLength - currentLineIndex * (singleLineLength + lineInterval);
        Vector3 lineDirection = (targetPoint - transform.position).normalized;

        // we need a new line
        if (neededLength > singleLineLength + lineInterval)
        {
            SetLinePosition(currentLineRenderer, 
                singleLineLength, 
                transform.position + lineDirection * currentLineIndex * (singleLineLength + lineInterval), 
                lineDirection);
            AddOneLine();
            currentLineIndex++;
            SetLinePosition(currentLineRenderer, 
                neededLength - (singleLineLength + lineInterval), 
                transform.position + lineDirection * currentLineIndex * (singleLineLength + lineInterval), 
                lineDirection);
        }
        // we are in interval
        else if (neededLength > singleLineLength)
        {
            SetLinePosition(currentLineRenderer, singleLineLength, 
                transform.position + lineDirection * currentLineIndex * (singleLineLength + lineInterval), lineDirection);
        }
        // we are drawing line
        else
        {
            SetLinePosition(currentLineRenderer, neededLength, 
                transform.position + lineDirection * currentLineIndex * (singleLineLength + lineInterval), lineDirection);
        }
    }

    private void SetLinePosition(LineRenderer lineRenderer, float neededLength, Vector3 startPoint, Vector3 lineDirection)
    {
        if (neededLength < 0)   // just to be safe... there was a bug but now should be ok
            return;

        lineDirection.Normalize();
        neededLength = neededLength > singleLineLength ? singleLineLength : neededLength;
        Vector3 endPoint = startPoint + lineDirection * neededLength;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
    }

    private void AddOneLine()
    {
        GameObject newLineObj = Instantiate(simpleLine, transform);
        currentLineRenderer = newLineObj.GetComponent<LineRenderer>();
        currentLineRenderer.useWorldSpace = true;
        lineCollection.Add(currentLineRenderer);
    }

    private void ResetLineStatus()
    {
        if (lineStatusCleared)
            return;

        lineStatusCleared = true;
        lineLength = 0f;
        currentLineIndex = 0;
        currentLineRenderer = null;

        foreach (var line in lineCollection)
        {
            Destroy(line.gameObject);
        }
        lineCollection.Clear();
    }
}
