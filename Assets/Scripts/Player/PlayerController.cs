using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [Header("Dash Ability")]
    [SerializeField] private float dashCD = 1f;
    [SerializeField] private float dashDistance = 4f;
    [SerializeField] private bool enableSecondDash = true;
    [SerializeField] private float secondDashCD = 1f;
    [SerializeField] private float secondDashTotalDistance = 8f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private TrailRenderer trailRenderer;

    public UnityEvent DashCompletedEvent;
    public UnityEvent DashStartingEvent;

    public bool CanDash { get; private set; }

    public Vector3 TargetPosition { get; private set; }

    public bool IsDashing
    {
        get { return isDashing; }
        private set
        {
            isDashing = value;
            trailRenderer.emitting = value;
        }
    }

    public float FirstDashCD { get { return dashCD; } }

    public float SecondDashCD { get { return secondDashCD; } }


    private const float rotationAngleOffset = 90f;
    private PlayerControls playerControls;
    private bool isDashing;


    #region Unity lifecycle

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void Start()
    {
        playerControls.Movement.Dash.performed += _ => Dash();
        CanDash = true;
        IsDashing = false;
    }

    private void Update()
    {
        UpdateTargetPosition();
    }

    private void FixedUpdate()
    {
        UpdateTargetPosition();
        if (!IsDashing)
        {
            LookAtTarget();
        }
    }

    private void OnEnable()
    {
        playerControls?.Enable();
    }

    private void OnDisable()
    {
        playerControls?.Disable();
    }

    #endregion

    #region Look at mouse
    private void LookAtTarget()
    {
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(playerControls.TargetPosition.Pos.ReadValue<Vector2>());
        targetPosition.z = 0;
        Vector3 direction = targetPosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - rotationAngleOffset;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void UpdateTargetPosition()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(playerControls.TargetPosition.Pos.ReadValue<Vector2>());
        mousePos.z = 0;
        float realDis = Vector3.Distance(transform.position, mousePos);
        if (realDis <= dashDistance)
        {
            TargetPosition = mousePos;
        }
        else
        {
            Vector3 direction = mousePos - transform.position;
            direction.z = 0;
            direction.Normalize();
            var targetPos = direction * dashDistance + transform.position;
            targetPos.z = 0;
            TargetPosition = targetPos;
        }
    }
    #endregion

    #region Dash
    private void Dash()
    {
        if (!CanDash)
            return;

        CanDash = false;
        IsDashing = true;
        DashStartingEvent?.Invoke();

        StopAllCoroutines();

        StartCoroutine(DashRoutine(TargetPosition));
    }

    private IEnumerator DashRoutine(Vector3 targetPos)
    {
        Vector3 direction = targetPos - transform.position;
        direction.z = 0;

        while (Vector3.Distance(transform.position, targetPos) > 1)
        {
            transform.position = transform.position + direction * Time.deltaTime * dashSpeed;
            yield return null;
        }

        transform.position = targetPos;
        yield return new WaitForSeconds(0.2f);
        IsDashing = false;

        StartCoroutine(DashCDRoutine());
        
        DashCompletedEvent?.Invoke();
    }

    private IEnumerator DashCDRoutine()
    {
        yield return new WaitForSeconds(dashCD);
        CanDash = true;
    }
    #endregion

}
