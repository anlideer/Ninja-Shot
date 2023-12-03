using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Enemy behavior")]
    [SerializeField] private Vector3[] rotationStops = new Vector3[] { };   // will loop through and repeat
    [SerializeField] private float rotatingDuration = 1f;
    [SerializeField] private float stopTime = 0.5f;

    private void Start()
    {
        StartCoroutine(RotateRoutine());
    }

    private IEnumerator RotateRoutine()
    {
        int ind = 0;
        while (ind < rotationStops.Length)
        {
            if (ind + 1 < rotationStops.Length)
            {
                yield return StartCoroutine(
                    RotateBetweenTwoAnglesRoutine(rotationStops[ind], rotationStops[ind + 1]));
                ind++;
            }
            else
            {
                yield return StartCoroutine(
                    RotateBetweenTwoAnglesRoutine(rotationStops[ind], rotationStops[0]));
                ind = 0;
            }
            yield return new WaitForSeconds(stopTime);
        }
    }

    private IEnumerator RotateBetweenTwoAnglesRoutine(Vector3 fromRot, Vector3 toRot)
    {
        Quaternion fromQ = Quaternion.Euler(fromRot);
        Quaternion toQ = Quaternion.Euler(toRot);

        float elapsedTime = 0f;
        while (elapsedTime < rotatingDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(fromQ, toQ, elapsedTime / rotatingDuration);
            yield return null;
        }
        transform.rotation = toQ;
    }
}