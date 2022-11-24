using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using System;

public class CameraManager : MonoBehaviour
{
    [SerializeField] GameObject targetObj;
    Vector3 defaultPosition;
    float movespeed = 5;
    public static CameraManager Instance { get; private set; }
    [SerializeField]bool isEvent;



    public float Movespeed { get => movespeed; set => movespeed = value; }
    public bool IsEvent { get => isEvent; set => isEvent = value; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        defaultPosition = transform.position;
    }

    public void SetTarget(GameObject cameraTarget)
    {
        targetObj = cameraTarget;
    }

    private void LateUpdate()
    {
        if (!isEvent)
        {
            Vector3 vector3 = targetObj.transform.position;
            vector3.z = defaultPosition.z - 10f;
            transform.position = vector3;
        }
    }
}
