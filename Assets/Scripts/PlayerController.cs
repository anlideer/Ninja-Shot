using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{

    private const float rotationAngleOffset = 90f;

    private void Update()
    {
        LookAtMouse();
    }

    private void LookAtMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - rotationAngleOffset;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
