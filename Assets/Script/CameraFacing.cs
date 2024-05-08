using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFacing : MonoBehaviour
{
    private Camera _cam;
    private void Start()
    {
        _cam = Camera.main;
        var direction = _cam.transform.position - transform.position;
        direction.z = 0;
        transform.LookAt(transform.position-direction);
    }
}