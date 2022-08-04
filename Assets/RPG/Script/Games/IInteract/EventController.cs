using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventController : MonoBehaviour, IStoryInteract
{
    EventDialogManager dialogs;


    public void Start()
    {
        dialogs = GetComponentInParent<EventDialogManager>();
    }
    public void StoryInteract(Dialog dialog)
    {
        StartCoroutine(DialogManager.Instance.ShowDialog(dialogs.Dialogs[dialogs.GetStoryChapter()], Story));
    }

    //�L�����f�[�^�������Ƃ��͍ŏ��ɕK��Init�ŏ��������Ă���d�l�i�f�[�^�������Ȃ��ăG���[�ɂȂ�j
    void Story()
    {
        switch (dialogs.GetStoryChapter())
        {
            case 0:
                dialogs.Battlers[dialogs.GetBattlerIndex()].Init();
                dialogs.Player.InParty(dialogs.Battlers[dialogs.GetBattlerIndex()]);
                dialogs.SetBattlerIndex();
                break;
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