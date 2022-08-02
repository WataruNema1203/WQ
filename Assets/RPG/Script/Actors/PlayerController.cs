using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    bool isMoving;
    bool isSelect;
    Vector2 input;
    float moveSpeed = 5;
    int encountMod = 0;//エンカウント率0-100%
    Animator animator;


    public UnityAction<Battler> OnEncounts;//Encountしたときに実行したい関数を登録できる

    [SerializeField] List<Battler> battlers = new List<Battler>();

    public List<Battler> Battlers { get => battlers; }
    public int EncountMod { get => encountMod; set => encountMod = value; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        Battlers[0].Init();
        isSelect = false;
        Battlers[0].Gold += 2000;

    }

    public void InParty(Battler NewMenber)
    {
        battlers.Add(NewMenber);
    }


    public void HandleUpdate()
    {
        if (!isMoving&&!isSelect)
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
                animator.SetFloat("x", input.x);
                animator.SetFloat("y", input.y);
            }
        }
        animator.SetBool("IsMoving", isMoving);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Interact();
        }

        StoryInteract();
    }

    public void StartPlayer()
    {
        isSelect = false;
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

        transform.position = targetPos;
        CheckForEncounters();
    }

    void CheckForEncounters()
    {
        Collider2D collider2D = Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.Instance.FieldLayer);
        if (collider2D)
        {
            if (Random.Range(0, 100) < encountMod)
            {
                OnEncounts?.Invoke(collider2D.GetComponent<EncountArea>().GetRandomBattler());
                animator.SetBool("IsMoving", false);
            }
        }
        isMoving = false;
    }

    void Interact()
    {
        Vector3 faceDirection = new Vector3(animator.GetFloat("x"), animator.GetFloat("y"));

        Vector3 interactPos = transform.position + faceDirection;

        Collider2D collider2D = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.Instance.InteractableLayer);
        if (collider2D)
        {
            collider2D.GetComponent<IInteract>().Interact(transform);
        }

    }


    void StoryInteract()
    {
        Vector3 faceDirection = new Vector3(animator.GetFloat("x"), animator.GetFloat("y"));

        Vector3 interactPos = transform.position + faceDirection;

        Collider2D collider2D = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.Instance.StoryLayer);
        if (collider2D)
        {
            collider2D.GetComponent<IInteract>().Interact(transform);
        }

    }


    //いまから移動する位置に移動できるか判定する関数
    bool IsWalkable(Vector2 targetPos)
    {
        return Physics2D.OverlapCircle(targetPos, 0.2f, GameLayers.Instance.SolidObjectsLayer | GameLayers.Instance.InteractableLayer) == false;
    }

}

