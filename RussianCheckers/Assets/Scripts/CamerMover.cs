using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerMover : MonoBehaviour
{
    public float rotationSpeed;
    void Update()
    {
        float horizontalInput = -Input.GetAxis("Horizontal");
        float verticallInput = Input.GetAxis("Vertical");

        transform.eulerAngles = transform.eulerAngles + new Vector3(verticallInput, horizontalInput, 0);
    }
}