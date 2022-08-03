using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    
    public GameObject MainCamera;

    Vector3 cameraVelocity = Vector3.zero;

    void Start()
    {
        
    }

    void Update()
    {
        PositionCamera();
    }

    void PositionCamera()
    {
        if (MainCamera == null) return;
        Vector3 cameraPosition = transform.TransformPoint(new Vector3(0, 2, -10));
        MainCamera.transform.position = Vector3.SmoothDamp(
            MainCamera.transform.position,
            cameraPosition,
            ref cameraVelocity,
            0.3f
        );
    }
}
