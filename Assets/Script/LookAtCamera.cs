using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera mainCamera;
    public bool isActive = false;

    void Start()
    {
        mainCamera = Camera.main;
        isActive = false;
    }

    void LateUpdate()
    {
        if (mainCamera != null && isActive)
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                             mainCamera.transform.rotation * Vector3.up);
        }
    }

}
