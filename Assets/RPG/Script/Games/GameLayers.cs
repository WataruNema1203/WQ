using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    // �ǔ����Layer
    [SerializeField] LayerMask solidObjectsLayer;
    [SerializeField] LayerMask interactableLayer;
    // ���ނ画���Layer
    [SerializeField] LayerMask fieldLayer;

    [SerializeField] LayerMask playerLayer;

    [SerializeField] LayerMask storyLayer;

    // �ǂ�����ł����p�\
    public static GameLayers Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    public LayerMask SolidObjectsLayer { get => solidObjectsLayer; }
    public LayerMask InteractableLayer { get => interactableLayer; }
    public LayerMask FieldLayer { get => fieldLayer; }
    public LayerMask PlayerLayer { get => playerLayer; }
    public LayerMask StoryLayer { get => storyLayer; }
}
