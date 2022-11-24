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
    public UnityAction OnUpdate;

    Animator animator;

    public Item Item { get => item; }

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
        yield return StartCoroutine(DialogManager.Instance.FieldTypeDialog($"�󔠂��J����"));
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        OnTreasureBoxItem?.Invoke(item);
        yield return StartCoroutine(DialogManager.Instance.FieldTypeDialog($"{item.Base.GetKanjiName()}���E�����I"));
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        DialogManager.Instance.Close();
        player.StartPlayer();
        dialog.SetActive(false);
        item = null;
        OnUpdate?.Invoke();
    }

    public void LookToward()
    {
        animator.SetBool("IsOpen", true);
    }
}
