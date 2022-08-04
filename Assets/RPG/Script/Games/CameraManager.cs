using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] GameObject clampObj;
    Vector3 defaultPosition;
    float movespeed = 5;
    public static CameraManager Instance { get; private set; }


    Vector3 max;
    Vector3 min;

    public float Movespeed { get => movespeed; set => movespeed = value; }

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
        max = clampObj.GetComponent<Renderer>().bounds.max;
        min = clampObj.GetComponent<Renderer>().bounds.min;
    }

    private void Update()
    {
        max = clampObj.GetComponent<Renderer>().bounds.max;
        min = clampObj.GetComponent<Renderer>().bounds.min;
        Vector3 vector3 = player.transform.position;
        vector3.z = defaultPosition.z;
        vector3.x = Mathf.Clamp(vector3.x, min.x, max.x);
        vector3.y = Mathf.Clamp(vector3.y, min.y, max.y);
        transform.position = Vector3.MoveTowards(transform.position, vector3, Time.deltaTime * movespeed);
    }
}
