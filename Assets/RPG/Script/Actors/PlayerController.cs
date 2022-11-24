using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

enum PlayerState
{
    FreeRoam,
    WarpPointSet,
    Event,
}

public class PlayerController : MonoBehaviour
{
    PlayerState playerState;
    bool isMoving;
    bool isSelect;
    bool isShip;
    [SerializeField] Vector2 input;
    float moveSpeed = 5;
    public int encountMod = -105;//エンカウント率0-100%
    int originalEncountMod = 5;
    float count;
    float encountSkill;
    Animator charaAnimation;


    public UnityAction<List<Battler>> OnEncounts;//Encountしたときに実行したい関数を登録できる

    [SerializeField] List<Battler> battlers = new List<Battler>();
    [SerializeField] List<Battler> enemyBattlers = new List<Battler>();
    public List<Battler> Battlers { get => battlers; }
    public int EncountMod { get => encountMod; set => encountMod = value; }
    public float EncountSkill { get => encountSkill; set => encountSkill = value; }
    internal PlayerState PlayerState { get => playerState; set => playerState = value; }

    public static PlayerController Instance { get; private set; }
    public bool IsSelect { get => isSelect; }
    public List<Battler> EnemyBattlers { get => enemyBattlers; }
    public int OriginalEncountMod { get => originalEncountMod; }

    private void Awake()
    {
        charaAnimation = GetComponent<Animator>();
    }
    private void Start()
    {
        playerState = PlayerState.FreeRoam;
        Battlers[0].Init();
        isSelect = false;
        if (Instance == null)
        {
            Instance = this;
        }

    }

    public void InParty(Battler NewMenber)
    {
        battlers.Add(NewMenber);
    }


    public void HandleUpdate()
    {
        if (playerState == PlayerState.FreeRoam)
        {
            if (!isMoving && !isSelect)
            {
                input.x = Input.GetAxisRaw("Horizontal");
                input.y = Input.GetAxisRaw("Vertical");


                if (input.x != 0)
                {
                    input.y = 0;
                }

                if (input != Vector2.zero)
                {
                    StartCoroutine(Move(input));
                    charaAnimation.SetFloat("x", input.x);
                    charaAnimation.SetFloat("y", input.y);
                }
            }
            charaAnimation.SetBool("IsMoving", isMoving);
        }

    }
    public void StartPlayer()
    {
        encountMod = originalEncountMod;
        isSelect = false;
        isMoving = false;
        playerState = PlayerState.FreeRoam;
    }
    public void StartSelect()
    {
        isSelect = true;
    }


    IEnumerator Move(Vector2 moveVec)
    {
        isMoving = true;
        Vector3 targetPos = transform.position;

        targetPos += (Vector3)moveVec;

        if (!IsWalkable(targetPos))
        {
            isMoving = false;
            yield break;
        }

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
                );
            yield return null;
        }

        EncountSkillCount();
        OnEventCheck();
    }

    public void EncountSkillCount()
    {
        if (encountSkill > 0)
        {
            count += 1;
            if (encountSkill <= count)
            {
                encountMod = 5;
                encountSkill = 0;
                count = 0;
                return;
            }
        }
    }


    void CheckForEncounters()
    {
        Collider2D collider2D = Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.Instance.FieldLayer);
        if (collider2D)
        {
            if (Random.Range(0, 100) < encountMod)
            {
                for (int i = 0; i < (Random.Range(1, 6)); i++)
                {
                    enemyBattlers.Add(collider2D.GetComponent<EncountArea>().GetRandomBattler());
                }
                OnEncounts?.Invoke(enemyBattlers);
            }
        }
        isMoving = false;
    }

    public void Interact()
    {
        Vector3 faceDirection = new Vector3(charaAnimation.GetFloat("x"), charaAnimation.GetFloat("y"));

        Vector3 interactPos = transform.position + faceDirection;

        Collider2D collider2D = Physics2D.OverlapCircle(interactPos, 0.1f, GameLayers.Instance.InteractableLayer);
        if (collider2D)
        {
            originalEncountMod = encountMod;
            isSelect = true;
            collider2D.GetComponent<IInteract>().Interact(transform);
        }
    }


    public void OnEventCheck()
    {
        Vector3 faceDirection = new Vector3(charaAnimation.GetFloat("x"), charaAnimation.GetFloat("y"));

        Vector3 interactPos = transform.position + faceDirection;

        Collider2D colliderStory = Physics2D.OverlapCircle(interactPos, 0.1f, GameLayers.Instance.StoryLayer);
        Collider2D colliderMove = Physics2D.OverlapCircle(interactPos, 0.1f, GameLayers.Instance.MovePoint);
        Collider2D colliderWorldMove = Physics2D.OverlapCircle(interactPos, 0.1f, GameLayers.Instance.WorldMovePoint);
        Collider2D colliderMap = Physics2D.OverlapCircle(interactPos, 0.1f, GameLayers.Instance.MapChangeLayer);
        if (colliderStory)
        {
            originalEncountMod = encountMod;
            isSelect = true;
            isMoving = true;
            colliderStory.GetComponent<EventController>().Call();
        }
        else if (colliderMove)
        {
            originalEncountMod = encountMod;
            isSelect = true;
            StartCoroutine(colliderMove.GetComponent<MovePointBase>().MoveMap());
            isMoving = false;

        }
        else if (colliderWorldMove)
        {
            originalEncountMod = encountMod;
            isSelect = true;
            StartCoroutine(colliderWorldMove.GetComponent<WorldMapMovePoint>().MoveMap());
            isMoving = false;

        }
        else if (colliderMap)
        {
            colliderMap.GetComponent<MapController>().MapChanges();

        }
        else
        {
            CheckForEncounters();
        }
    }
    public void WarpFlag()
    {
        if (playerState == PlayerState.FreeRoam)
        {
            Vector3 faceDirection = new Vector3(charaAnimation.GetFloat("x"), charaAnimation.GetFloat("y"));

            Vector3 interactPos = transform.position + faceDirection;

            Collider2D collider2D = Physics2D.OverlapCircle(interactPos, 0.5f, GameLayers.Instance.WarpPointLayer);
            if (collider2D)
            {
                playerState = PlayerState.WarpPointSet;
                originalEncountMod = encountMod;
                isSelect = true;
                StartCoroutine(collider2D.GetComponent<WarpPointBase>().SetPoint());
            }
        }
    }
    public void HiddenItem()
    {
        if (playerState == PlayerState.FreeRoam)
        {
            Vector3 faceDirection = new Vector3(charaAnimation.GetFloat("x"), charaAnimation.GetFloat("y"));

            Vector3 interactPos = transform.position + faceDirection;

            Collider2D collider2D = Physics2D.OverlapCircle(interactPos, 0.1f, GameLayers.Instance.HiddenItemLayer);
            if (collider2D)
            {
                originalEncountMod = encountMod;
                isSelect = true;
                StartCoroutine(collider2D.GetComponent<HiddenItemBase>().GiveItem());
            }
        }
    }


    //いまから移動する位置に移動できるか判定する関数
    bool IsWalkable(Vector2 targetPos)
    {
        if (isShip)
        {
        return Physics2D.OverlapCircle(targetPos, 0.2f, GameLayers.Instance.SolidObjectsLayer | GameLayers.Instance.WarpPointLayer | GameLayers.Instance.InteractableLayer ) == false;
        }
        else
        {
            return Physics2D.OverlapCircle(targetPos, 0.2f, GameLayers.Instance.SolidObjectsLayer | GameLayers.Instance.WarpPointLayer | GameLayers.Instance.InteractableLayer | GameLayers.Instance.EnemyField_Sea) == false;
        }
    }

    public void SetPlayerPosition(Vector3 vector3)
    {
        this.gameObject.transform.position = vector3;
    }

}

