using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TreasureBoxBase: MonoBehaviour, IInteract
{
    [SerializeField] PlayerController player;

    [SerializeField] Item item;
    [SerializeField] GameObject dialog;

    public UnityAction<Item> OnTreasureBoxItem;

    Animator animator;


    void Init()
    {
        animator = GetComponent<Animator>();
    }


    public void Interact(Transform trf)
    {
        Init();
        LookToward();
        StartCoroutine(GiveItem());
    }

    IEnumerator GiveItem()
    {
        yield return DialogManager.Instance.TypeDialog($"宝箱を開けた");
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        Debug.Log("アイテム取得処理開始");
        OnTreasureBoxItem?.Invoke(item);
        Debug.Log("アイテム取得処理終了");
        yield return DialogManager.Instance.TypeDialog($"{item.Base.GetKanjiName()}を拾った！");
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        player.StartPlayer();
        dialog.SetActive(false);
    }

    public void LookToward()
    {
        animator.SetBool("IsOpen", true);
    }
}
