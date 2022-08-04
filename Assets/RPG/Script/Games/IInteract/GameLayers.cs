using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    // •Ç”»’è‚ÌLayer
    [SerializeField] LayerMask solidObjectsLayer;
    [SerializeField] LayerMask interactableLayer;
    // ‘‚Þ‚ç”»’è‚ÌLayer
    [SerializeField] LayerMask fieldLayer;

    [SerializeField] LayerMask playerLayer;

    [SerializeField] LayerMask storyLayer;

    // ‚Ç‚±‚©‚ç‚Å‚à—˜—p‰Â”\
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
