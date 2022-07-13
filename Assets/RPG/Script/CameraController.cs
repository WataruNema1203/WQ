using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Camera MenuCamera;

    private void Update()
    {
        MenuCamera.transform.position = gameObject.transform.position;
    }

    
}
