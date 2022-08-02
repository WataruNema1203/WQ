using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventController : MonoBehaviour, IInteract
{
    EventDialogManager dialogs;

    public void Start()
    {
        dialogs = GetComponentInParent<EventDialogManager>();
    }
    public void Interact(Transform trf)
    {
        StartCoroutine(DialogManager.Instance.ShowDialog(dialogs.Dialogs[dialogs.GetStoryChapter()], Story));
    }


    void Story()
    {
        switch (dialogs.GetStoryChapter())
        {
            case 1:
                dialogs.Battlers[dialogs.GetBattlerIndex()].Init();
                GameController.Instance.StartBattle(dialogs.Battlers[dialogs.GetBattlerIndex()]);
                dialogs.SetBattlerIndex();
                break;
            case 3:
                dialogs.Battlers[dialogs.GetBattlerIndex()].Init();
                GameController.Instance.StartBattle(dialogs.Battlers[dialogs.GetBattlerIndex()]);
                dialogs.SetBattlerIndex();
                break;
            default:

                break;
        }
        dialogs.SetStoryChapter();
        Destroy(gameObject);
        dialogs.Player.StartPlayer();
    }
}