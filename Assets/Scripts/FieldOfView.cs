using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private float viewRadius = 1f;
    [SerializeField][Range(0, 360)] private float viewAngle = 30f;
    [SerializeField] private int rayStepCount = 4;

    private Mesh mesh;
    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        DrawSimpleSector();
    }

    private void Update()
    {
        DrawSimpleSector();
    }

    private void DrawSimpleSector()
    {
        Vector3[] vertices = new Vector3[rayStepCount + 2];
        Vector2[] uvs = new Vector2[rayStepCount + 2];
        int[] indices = new int[rayStepCount * 3];

        float angleIncrease = viewAngle / rayStepCount;
        float angle = viewAngle / 2;
        int triangleCnt = 0;
        vertices[0] = Vector3.zero;
        for (int i = 1; i < rayStepCount; i++)
        {
/*            RaycastHit2D rayHit = Physics2D.Raycast(
                transform.position, 
                GetVectorForAngle(angle + transform.rotation.eulerAngles.z), 
                viewRadius);    // mesh is in local space
            Debug.DrawRay(transform.position, GetVectorForAngle(angle + transform.rotation.eulerAngles.z), Color.magenta, 100);
*/            
            
            RaycastHit2D rayHit = Physics2D.Raycast(
                transform.position, 
                transform.TransformDirection(GetVectorForAngle(angle)), 
                viewRadius);    // mesh is in local space
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
            indices[triangleCnt] = 0;
            indices[triangleCnt + 1] = i - 1;
            indices[triangleCnt + 2] = i;

            angle -= angleIncrease;
            triangleCnt += 3;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.uv = uvs;
    }

    private Vector3 GetVectorForAngle(float angle)
    {
        float angleRad = angle * Mathf.PI / 180f;
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0);
    }
}
