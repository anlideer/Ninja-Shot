using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Enemy behavior")]
    [SerializeField] private Vector3[] rotationStops = new Vector3[] { };   // will loop through and repeat
    [SerializeField] private float rotatingDuration = 1f;
    [SerializeField] private float stopTime = 0.5f;

    protected virtual void Start()
    {
        StartCoroutine(RotateRoutine());
    }

    protected IEnumerator RotateRoutine()
    {
        int ind = 0;
        while (ind < rotationStops.Length)
        {
            if (ind + 1 < rotationStops.Length)
            {
                yield return StartCoroutine(
                    RotateBetweenTwoAnglesRoutine(rotationStops[ind], rotationStops[ind + 1], rotatingDuration));
                ind++;
            }
            else
            {
                yield return StartCoroutine(
                    RotateBetweenTwoAnglesRoutine(rotationStops[ind], rotationStops[0], rotatingDuration));
                ind = 0;
            }
            yield return new WaitForSeconds(stopTime);
        }
    }

    protected IEnumerator RotateBetweenTwoAnglesRoutine(Vector3 fromRot, Vector3 toRot, float duration)
    {
        Quaternion fromQ = Quaternion.Euler(fromRot);
        Quaternion toQ = Quaternion.Euler(toRot);

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(fromQ, toQ, elapsedTime / duration);
            yield return null;
        }
        transform.rotation = toQ;
    }
}
