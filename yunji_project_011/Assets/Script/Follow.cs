using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset; //보정값

    void Update()
    {
        transform.position = target.position + offset; //카메라가 player움직임을 따라감
    }
}
