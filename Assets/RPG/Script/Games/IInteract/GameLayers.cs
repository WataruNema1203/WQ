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

    [SerializeField] LayerMask warpPointLayer;

    [SerializeField] LayerMask hiddenItemLayer;

    [SerializeField] LayerMask movePoint;

    [SerializeField] LayerMask worldMovePoint;


    [SerializeField] LayerMask mapChangeLayer;

    [SerializeField] LayerMask enemyField_Sea;

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
    public LayerMask WarpPointLayer { get => warpPointLayer; }
    public LayerMask HiddenItemLayer { get => hiddenItemLayer;}
    public LayerMask MovePoint { get => movePoint;}
    public LayerMask WorldMovePoint { get => worldMovePoint; }
    public LayerMask MapChangeLayer { get => mapChangeLayer;}
    public LayerMask EnemyField_Sea { get => enemyField_Sea;}
}
