using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerMover : MonoBehaviour
{
    private void Start()
    {
        transform.position = new Vector3(3.5f, 0, 3.5f);
    }
    void Update()
    {
        transform.eulerAngles += new Vector3(Input.GetAxis("Vertical"), -Input.GetAxis("Horizontal"), 0);
    }
}