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
        if (Input.GetMouseButton(1))
        {
            if (Input.GetAxis("Mouse X") < 0)
            {
                transform.eulerAngles += new Vector3(Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"), 0.0f) * Time.deltaTime * 100f;
            }

            else if (Input.GetAxis("Mouse X") > 0)
            {
                transform.eulerAngles += new Vector3(Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"), 0.0f) * Time.deltaTime * 100f;
            }
        }
    }
}