using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShopNPCController : MonoBehaviour, IInteract
{
    [SerializeField] Dialog dialog;

    Animator animator;
    public UnityAction OnShopStart;



    void Awake()
    {
        animator = GetComponent<Animator>();
    }


    public void Interact(Transform trf)
    {
        LookToward(trf.position);
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, OnShopStart));
    }


    public void LookToward(Vector3 targetPos)
    {
        float xDiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        float yDiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);

        if (xDiff == 0 || yDiff == 0)
        {
            animator.SetFloat("x", Mathf.Clamp(xDiff, -1f, 1f));
            animator.SetFloat("y", Mathf.Clamp(yDiff, -1f, 1f));
        }
    }
}
