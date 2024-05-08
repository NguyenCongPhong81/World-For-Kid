using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    [SerializeField] private int speedRotate = 50;
    
    void Update()
    {
        var rotate = transform.rotation.eulerAngles;
        rotate.y += speedRotate * Time.deltaTime;
        transform.rotation = Quaternion.Euler(rotate);
    }
}
