using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Header("Basic")]
    [SerializeField] private float viewRadius = 1f;
    [SerializeField][Range(0, 360)] private float viewAngle = 30f;
    [SerializeField] private int rayStepCount = 4;
    [SerializeField] private LayerMask layerMask;

    private Mesh mesh;
    private PolygonCollider2D fovCollider;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        fovCollider = GetComponent<PolygonCollider2D>();
    }

    private void Update()
    {
        DrawFOVSector();
    }

    private void DrawFOVSector()
    {
        Vector3[] vertices = new Vector3[rayStepCount + 2];
        Vector2[] uvs = new Vector2[rayStepCount + 2];
        int[] indices = new int[rayStepCount * 3];

        float angleIncrease = viewAngle / rayStepCount;
        float angle = viewAngle / 2;
        int triangleCnt = 0;
        vertices[0] = Vector3.zero;
        for (int i = 1; i < rayStepCount + 2; i++)
        { 
            RaycastHit2D rayHit = Physics2D.Raycast(
                transform.position, 
                transform.TransformDirection(GetVectorForAngle(angle)), 
                viewRadius, layerMask);    // mesh is in local space
            //Debug.DrawRay(transform.position, transform.TransformDirection(GetVectorForAngle(angle)), Color.magenta, 100);

            if (rayHit.collider == null)
            {
                vertices[i] = viewRadius * GetVectorForAngle(angle); 
            }
            else
            {
                Vector3 hitPoint = rayHit.point;
                vertices[i] = transform.InverseTransformPoint(hitPoint);    // mesh is in local space
                //Debug.DrawLine(transform.position, hitPoint, Color.red, 100);
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

    private void UpdateCollider(Vector3[] localVertices)
    {
        if (fovCollider == null)
            return;

        // TODO: have an option to keep the original defined collider or overwrite it
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
