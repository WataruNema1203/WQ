using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDialogManager : MonoBehaviour
{
    [SerializeField] List<Dialog> dialogs;
    [SerializeField] List<Battler> battlers;
    [SerializeField] PlayerController player;
    [SerializeField] int storyChapter = 0;
    [SerializeField] int battlerIndex = 0;

    public List<Dialog> Dialogs { get => dialogs; }
    public List<Battler> Battlers { get => battlers; }
    public PlayerController Player { get => player; }

    public void SetStoryChapter()
    {
        storyChapter++;
    }
    public int GetStoryChapter()
    {
        return storyChapter;
    }

    public void SetBattlerIndex()
    {
        battlerIndex++;
    }

    public int GetBattlerIndex()
    {
        return battlerIndex;
    }

}
