using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Controller Values")]
    public Transform cam;
    public Transform target;

    private Vector3 _position;
    public float lerpPercentage;

    void Start()
    {
        _position = cam.position;
    }

    void Update()
    {
        if (this.target == null) return;
        Vector3 targetPosition = target.position;
        targetPosition.z = -10;
        _position = Vector3.Lerp(cam.position, targetPosition, lerpPercentage * Time.deltaTime);
    }

    void LateUpdate()
    {
        cam.position = _position;
    }
}
