using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.3F;

    Vector3 velocity = Vector3.zero;
    Vector3 targetOffset;
    Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
        targetOffset = transform.position - target.position;
    }

    void FixedUpdate()
    {
        // Define a target position above and behind the target transform
        Vector3 targetPosition = target.TransformPoint(targetOffset);

        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
