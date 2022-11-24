using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

enum StatusState
{
    SelectChara,
    ShowStatus,
    ShowCharaImage,
}

public class StatusUI : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] GameObject statusUI;
    [SerializeField] GameObject playGorld;
    [SerializeField] GameObject statusCharaSelect;
    [SerializeField] GameObject statusSkillPanel;
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI statusText;
    [SerializeField] TextMeshProUGUI playTimeText;
    [SerializeField] TextMeshProUGUI hasGoldText;
    string statusLine;
    public int selectedChara;
    StatusState statusState;

    StatusCharaUI[] statusCharas;
    SkillUI[] skillSlots;
    [SerializeField]
    private int minute;
    [SerializeField]
    private float seconds;
    //　前のUpdateの時の秒数
    private float oldSeconds;
    //　タイマー表示用テキスト
    private string timerText;
    Color dei = Color.red;
    Color alive = Color.white;
    Color show = new Color(255, 255, 255, 255);
    Color showOff;
    //覚える技
    public List<Move> Moves { get; set; }
    //現在の技
    public Move CurrentMove { get; set; }


    internal StatusState StatusState { get => statusState;}
    public StatusCharaUI[] StatusCharas { get => statusCharas;}

    public void Init()
    {
        statusCharas = GetComponentsInChildren<StatusCharaUI>();

        StatusCharaShow();
    }

    public void SkillInit()
    {
        skillSlots = GetComponentsInChildren<SkillUI>();

        //覚える技から使える技を生成
        Moves = new List<Move>(21);
        foreach (var learnableMove in player.Battlers[selectedChara].Base.LearnableMoves)
        {
            if (learnableMove.Level <= player.Battlers[selectedChara].Level)
            {
                Moves.Add(new Move(learnableMove.MoveBase));
            }
        }

        SkillShow();
    }


    void StatusCharaShow()
    {
        for (int i = 0; i < statusCharas.Length; i++)
        {
            if (i == 0)
            {
                statusCharas[i].SetText($"{player.Battlers[i].Base.Name}");
            }
            else if (i == 1)
            {
                if (player.Battlers.Count == 2)
                {
                    statusCharas[i].SetText($"{player.Battlers[i].Base.Name}");
                }
                else
                {
                    statusCharas[i].SetText("-");
                }
            }

        }
    }

    void SkillShow()
    {
        for (int i = 0; i < skillSlots.Length; i++)
        {
            if (Moves.Count <= skillSlots.Length)
            {
                Moves.Add(null);
            }
        }
        for (int i = 0; i < skillSlots.Length; i++)
        {

            if (Moves[i] == null)
            {
                skillSlots[i].SetText($" ");

            }
            else if (Moves[i].Base.Category1 == MoveCategory1.Skill || Moves[i].Base.Category1 == MoveCategory1.Stat || Moves[i].Base.Category1 == MoveCategory1.Physical)
            {
                skillSlots[i].SetText($"{Moves[i].Base.Name} ");

            }
            else if (Moves[i].Base == true)
            {
                skillSlots[i].SetText($"{Moves[i].Base.Name} ");

            }
        }
    }



    private void StatusInit()
    {
        if (player.Battlers[selectedChara].isDai)
        {
            statusText.color = dei;
        }
        else if ((!player.Battlers[selectedChara].isDai))
        {
            statusText.color = alive;
        }
        statusLine = "";
        hasGoldText.text = $"所持G：{player.Battlers[0].Gold}";
        image.sprite = player.Battlers[selectedChara].Base.Sprite;
        statusLine = "";
        statusLine = $"名前：{ player.Battlers[selectedChara].Base.Name}\n";
        statusLine += $"レベル：{player.Battlers[selectedChara].Level}\n";
        statusLine += $"HP：{ player.Battlers[selectedChara].HP}/{player.Battlers[selectedChara].MaxHP}\n";
        statusLine += $"MP：{ player.Battlers[selectedChara].MP}/{player.Battlers[selectedChara].MaxMP}\n";
        if (player.Battlers[selectedChara].HP <= 0)
        {
            statusLine += "状態：死亡\n";
        }
        else if (player.Battlers[selectedChara].Status == null)
        {
            statusLine += "状態：正常\n";
        }
        else
        {
            statusLine += $"状態：{player.Battlers[selectedChara].Status.Name}\n";
        }
        statusLine += $"攻撃力：{player.Battlers[selectedChara].Attack + player.Battlers[selectedChara].Base.GetEquipWeapon().Base.GetAmount() + player.Battlers[selectedChara].Base.GetEquipAccessory().Base.GetAmount()}\n";
        statusLine += $"防御力：{ player.Battlers[selectedChara].Defense + player.Battlers[selectedChara].Base.GetEquipArmor().Base.GetAmount()}\n";
        statusLine += $"魔法力：{player.Battlers[selectedChara].Intelligence + player.Battlers[selectedChara].Base.GetEquipWeapon().Base.GetMagicAmount() + player.Battlers[selectedChara].Base.GetEquipAccessory().Base.GetMagicAmount()}\n";
        statusLine += $"精神力：{ player.Battlers[selectedChara].Mind + player.Battlers[selectedChara].Base.GetEquipArmor().Base.GetMagicAmount()}\n";
        statusLine += $"素早さ：{ player.Battlers[selectedChara].Speed + player.Battlers[selectedChara].Base.GetEquipArmor().Base.GetSpeedAmount() + player.Battlers[selectedChara].Base.GetEquipAccessory().Base.GetSpeedAmount()}\n";
        statusLine += $"次のLvまで：{ ((400 * player.Battlers[selectedChara].Level * player.Battlers[selectedChara].Level / (204 - player.Battlers[selectedChara].Level)) + 5) - player.Battlers[selectedChara].HasExp}\n";
        statusLine += $"武器：{player.Battlers[selectedChara].Base.GetEquipWeapon().Base.GetKanjiName()}\n";
        statusLine += $"防具：{player.Battlers[selectedChara].Base.GetEquipArmor().Base.GetKanjiName()}\n";
        statusLine += $"装飾品：{player.Battlers[selectedChara].Base.GetEquipAccessory().Base.GetKanjiName()}";
        statusText.text = statusLine;

    }
    private void Update()
    {
        seconds += Time.deltaTime;
        if (seconds >= 60f)
        {
            minute++;
            seconds -= 60;
        }
        //　値が変わった時だけテキストUIを更新
        if ((int)seconds != (int)oldSeconds)
        {
            timerText = $"Play：{minute.ToString("00")}" + ":" + $"{((int)seconds).ToString("00")}";
        }
        oldSeconds = seconds;

        playTimeText.text = $"{timerText}";

    }

    public void Open()
    {
        statusState = StatusState.SelectChara;
        statusCharaSelect.SetActive(true);
        Init();
    }
    public void Close()
    {
        statusCharaSelect.SetActive(false);
    }
    public void StatusOpen()
    {
        statusState = StatusState.ShowStatus;
        statusSkillPanel.SetActive(true);
        statusUI.SetActive(true);
        playGorld.SetActive(true);
        StatusInit();
        SkillInit();
    }
    public void StatusClose()
    {
        statusUI.SetActive(false);
        playGorld.SetActive(false);
        statusSkillPanel.SetActive(false);
        Open();
    }

    public void TextClose()
    {
        statusState = StatusState.ShowCharaImage;
        statusText.gameObject.SetActive(false);
        showOff = image.color;
        image.color = show;
    }

    public void TextOpen()
    {
        statusState = StatusState.ShowStatus;
        statusText.gameObject.SetActive(true);
        image.color = showOff;
    }
}
