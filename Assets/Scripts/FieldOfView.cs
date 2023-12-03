using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A fov class to draw a sector-like fov mesh.
/// Make sure the scale of attached transform is always 1, including parents' scales.
/// </summary>
public class FieldOfView : MonoBehaviour
{
    [Header("Basic")]
    [SerializeField]
    [Tooltip("The radius of the view")]
    private float viewRadius = 2f;

    [SerializeField][Range(0, 360)]
    [Tooltip("The angle of the view. Will take the transform right as the center line to display this.")]
    private float viewAngle = 30f;

    [SerializeField][Range(0, 30)]
    [Tooltip("The angle step for one triangle. Decrese the value to get a more good-looking sector/circle, but it affects performance.")]
    private float angleStep = 3f;

    [SerializeField]
    [Tooltip("The layer mask that fov should detect the obstacle using the raycast.")]
    private LayerMask layerMask;

    [Header("Advanced: circle around the object")]
    [SerializeField]
    [Tooltip("Turn this on to also have a circle around the object.")]
    private bool createWithCircleAround = false;

    [SerializeField]
    [Tooltip("The radius of the circle around the object.")]
    private float circleRadius = 1f;

    [SerializeField]
    [Tooltip("Automatically update fov. Turn this off to control the update yourself by calling UpdateFOV.")]
    private bool autoUpdateFov = true;

    private Mesh mesh;
    private PolygonCollider2D fovCollider;

    public float ViewRadius
    {
        get { return viewRadius; }
        set 
        {
            viewRadius = value;
            UpdateFOV();
        }
    }

    public void UpdateFOV()
    {
        if (!createWithCircleAround)
        {
            DrawSimpleFOVSector();
        }
        else
        {
            DrawFOVSectorWithCircle();
        }
    }

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        fovCollider = GetComponent<PolygonCollider2D>();
    }

    private void FixedUpdate()
    {
        if (autoUpdateFov)
        {
            UpdateFOV();
        }
    }

    private void DrawSimpleFOVSector()
    {
        int rayStepCount = Mathf.FloorToInt(viewAngle / angleStep);
        float angleIncrease = viewAngle / rayStepCount;

        Vector3[] vertices = new Vector3[rayStepCount + 2];
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] indices = new int[rayStepCount * 3];

        float angle = viewAngle / 2;
        int triangleCnt = 0;
        vertices[0] = Vector3.zero;
        for (int i = 1; i < rayStepCount + 2; i++)
        {
            RaycastHit2D rayHit = Physics2D.Raycast(
                transform.position,
                transform.TransformDirection(GetVectorForAngle(angle)).normalized, 
                viewRadius, layerMask);    // mesh is in local space

            if (rayHit.collider == null)
            {
                vertices[i] = viewRadius * GetVectorForAngle(angle); 
            }
            else
            {
                Vector3 hitPoint = rayHit.point;
                vertices[i] = transform.InverseTransformPoint(hitPoint);    // mesh is in local space
            }

            if (i > 1)
            {
                indices[triangleCnt] = 0;
                indices[triangleCnt + 1] = i - 1;
                indices[triangleCnt + 2] = i;
                triangleCnt += 3;
            }

            angle -= angleIncrease;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.uv = uvs;

        UpdateCollider(vertices);
    }

    private void DrawFOVSectorWithCircle()
    {
        int rayStepCount = Mathf.FloorToInt(viewAngle / angleStep);
        float angleIncrease = viewAngle / rayStepCount;
        float circleAngle = 360 - viewAngle;
        int circleStepCount = Mathf.FloorToInt(circleAngle / angleStep);
        float circleAngleIncrease = circleAngle / circleStepCount;

        Vector3[] vertices = new Vector3[rayStepCount + circleStepCount + 3];
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] indices = new int[(rayStepCount + circleStepCount) * 3];

        float angle = viewAngle / 2;
        int triangleCnt = 0;
        vertices[0] = Vector3.zero;
        for (int i = 1; i < rayStepCount + 2; i++)
        {
            RaycastHit2D rayHit = Physics2D.Raycast(
                transform.position,
                transform.TransformDirection(GetVectorForAngle(angle)).normalized,
                viewRadius, layerMask);    // mesh is in local space

            if (rayHit.collider == null)
            {
                vertices[i] = viewRadius * GetVectorForAngle(angle);
            }
            else
            {
                Vector3 hitPoint = rayHit.point;
                vertices[i] = transform.InverseTransformPoint(hitPoint);    // mesh is in local space
            }

            if (i > 1)
            {
                indices[triangleCnt] = 0;
                indices[triangleCnt + 1] = i - 1;
                indices[triangleCnt + 2] = i;
                triangleCnt += 3;
            }

            angle -= angleIncrease;
        }

        int startingIndex = rayStepCount + 2;
        angle += angleIncrease;
        for (int j = 0; j < circleStepCount + 1; j++)
        {
            RaycastHit2D rayHit = Physics2D.Raycast(
                transform.position,
                transform.TransformDirection(GetVectorForAngle(angle).normalized),
                circleRadius, layerMask);    // mesh is in local space
            
            if (rayHit.collider == null)
            {
                vertices[j + startingIndex] = circleRadius * GetVectorForAngle(angle);
            }
            else
            {
                Vector3 hitPoint = rayHit.point;
                vertices[j + startingIndex] = transform.InverseTransformPoint(hitPoint);    // mesh is in local space
            }

            if (j >= 1)
            {
                indices[triangleCnt] = 0;
                indices[triangleCnt + 1] = startingIndex + j - 1;
                indices[triangleCnt + 2] = startingIndex + j;
                triangleCnt += 3;
            }

            angle -= circleAngleIncrease;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.uv = uvs;

        UpdateCollider(vertices);
    }

    private void UpdateCollider(Vector3[] localVertices)
    {
        if (fovCollider == null)
            return;

        Vector2[] colliderVertices = new Vector2[localVertices.Length];
        for (int i = 0; i < localVertices.Length; i++)
        {
            colliderVertices[i] = localVertices[i];
        }
        fovCollider.SetPath(0, colliderVertices);
    }

    private Vector3 GetVectorForAngle(float angle)
    {
        float angleRad = angle * Mathf.PI / 180f;
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0);
    }
}
