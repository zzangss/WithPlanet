using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour


{
    private Transform camTransform;

    void Start()
    {
        camTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + camTransform.forward, camTransform.up);
    }
}