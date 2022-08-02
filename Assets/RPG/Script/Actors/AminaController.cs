using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AminaController: MonoBehaviour, IInteract
{
    [SerializeField] GameObject dialog;
    [SerializeField] PlayerController player;
    [SerializeField] Battler battler;

    public void Interact(Transform trf)
    {
        StartCoroutine(InParty());
    }

    IEnumerator InParty()
    {
        battler.Init();
        yield return DialogManager.Instance.TypeDialog("ぐい？アミナかあいい？しゅき");
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        player.InParty(battler);
        yield return DialogManager.Instance.TypeDialog("アミナが仲間になった！");
        yield return new WaitForSeconds(0.5f);
        player.StartPlayer();
        dialog.SetActive(false);
        Destroy(gameObject);

    }


}
