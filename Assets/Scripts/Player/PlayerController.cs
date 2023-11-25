using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashCD = 1f;
    [SerializeField] private TrailRenderer trailRenderer;

    public bool CanDash { get; private set; }
    public bool IsDashing
    {
        get { return isDashing; }
        private set
        {
            isDashing = value;
            trailRenderer.emitting = value;
        }
    }

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

    private void LookAtTarget()
    {
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(playerControls.TargetPosition.Pos.ReadValue<Vector2>());
        targetPosition.z = 0;
        Vector3 direction = targetPosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - rotationAngleOffset;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void Dash()
    {
        if (!CanDash)
            return;

        CanDash = false;
        IsDashing = true;
        // TODO: targetPosition should not be mouse pos, should in a range
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(playerControls.TargetPosition.Pos.ReadValue<Vector2>());
        targetPosition.z = 0;
        StartCoroutine(DashRoutine(targetPosition));
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
        StartCoroutine(DashCDRoutine());

        yield return new WaitForSeconds(0.2f);
        IsDashing = false;
    }

    private IEnumerator DashCDRoutine()
    {
        yield return new WaitForSeconds(dashCD);
        CanDash = true;
    }
}
