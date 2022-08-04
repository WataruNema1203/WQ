using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;
using System;


enum SkillStatus
{
    CharaSelect,
    SkillSelect,
    SkillUseCharaSelect,
}

public class FieldSkillController : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] GameObject descriptionPanel;
    [SerializeField] GameObject skillCharaPanel;
    [SerializeField] GameObject skillPanel;
    [SerializeField] GameObject skillUseCharaPanel;
    [SerializeField] Text informationText;
    [SerializeField] Text informationValueText;

    SkillUI[] skillSlots;
    SkillCharaUI[] skillCharaSlots;
    SkillUseUI[] skillUseCharaSlots;
    int selectedChara;
    int selectedSkill;
    int selectedSkillUseChara;
    SkillStatus state;

    //覚える技
    public List<Move> Moves { get; set; }
    //現在の技
    public Move CurrentMove { get; set; }

    public int SelectedSkill { get => selectedSkill; }
    internal SkillStatus State { get => state; }
    public int SelectedItemUseChara { get => selectedSkillUseChara; }
    public int SelectedChara { get => selectedChara; set => selectedChara = value; }

    public UnityAction OnSkill;



    public void SkillCharaInit()
    {
        selectedChara = 0;
        skillCharaSlots = GetComponentsInChildren<SkillCharaUI>();

        SkillCharaShow();
    }

    public void SkillInit()
    {
        selectedSkill = 0;
        skillSlots = GetComponentsInChildren<SkillUI>();

        //覚える技から使える技を生成
        Moves = new List<Move>(10);
        foreach (var learnableMove in player.Battlers[selectedChara].Base.LearnableMoves)
        {
            if (learnableMove.Level <= player.Battlers[selectedChara].Level)
            {
                Moves.Add(new Move(learnableMove.MoveBase));
            }
        }

        SkillShow();
    }
    public void SkillUseInit()
    {
        selectedSkillUseChara = 0;
        skillUseCharaSlots = GetComponentsInChildren<SkillUseUI>();

        SkillUseCharaShow();
    }

    void SkillCharaShow()
    {
        for (int i = 0; i < skillCharaSlots.Length; i++)
        {
            if (i == 0)
            {
                skillCharaSlots[i].SetText("主人公");
            }
            else if (i == 1)
            {
                if (player.Battlers.Count == 2)
                {
                    skillCharaSlots[i].SetText("アミナ");
                }
                else
                {
                    skillCharaSlots[i].SetText("-");
                }
            }

        }
    }
    void SkillShow()
    {
        for (int i = 0; i < skillSlots.Length; i++)
        {
            if (Moves.Count <= 10)
            {
                Moves.Add(null);
            }
        }
        for (int i = 0; i < skillSlots.Length; i++)
        {

            if (Moves[i] == null)
            {
                skillSlots[i].SetText($"-");
                skillSlots[i].transform.Find("SkillMPText").GetComponent<Text>().text = " ";

            }
            else if (Moves[i].Base.Category1 == MoveCategory1.Skill || Moves[i].Base.Category1 == MoveCategory1.Stat || Moves[i].Base.Category1 == MoveCategory1.Physical)
            {
                skillSlots[i].SetText($"{Moves[i].Base.Name}");
                skillSlots[i].transform.Find("SkillMPText").GetComponent<Text>().text = "";

            }
            else if (Moves[i].Base == true)
            {
                skillSlots[i].SetText($"{Moves[i].Base.Name}");
                skillSlots[i].transform.Find("SkillMPText").GetComponent<Text>().text = "MP  " + $"{Moves[i].Base.Mp}";

            }
        }
    }
    void SkillUseCharaShow()
    {
        for (int i = 0; i < skillUseCharaSlots.Length; i++)
        {
            if (i == 0)
            {
                skillUseCharaSlots[i].SetText($"主人公 HP{player.Battlers[0].HP}/{player.Battlers[0].MaxHP} MP{player.Battlers[0].MP}/{player.Battlers[0].MaxMP}");
            }
            else if (i == 1)
            {
                if (player.Battlers.Count == 2)
                {
                    skillUseCharaSlots[i].SetText($"アミナ HP{player.Battlers[1].HP}/{player.Battlers[1].MaxHP} MP{player.Battlers[1].MP}/{player.Battlers[1].MaxMP}");
                }
                else
                {
                    skillUseCharaSlots[i].SetText("-");
                }
            }
        }
    }

    public void HandleUpdateSkillSelection()
    {
        switch (state)
        {
            case SkillStatus.CharaSelect:
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (selectedChara == 0)
                    {
                        selectedChara++;
                    }
                    else
                    {
                        selectedChara--;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (selectedChara == 1)
                    {
                        selectedChara--;
                    }
                    else
                    {
                        selectedChara++;
                    }
                }

                selectedChara = Mathf.Clamp(selectedChara, 0, 1);

                for (int i = 0; i < skillCharaSlots.Length; i++)
                {
                    bool selected = SelectedChara == i;
                    skillCharaSlots[i].SetSelectedColor(selected);
                }
                break;
            case SkillStatus.SkillSelect:
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (selectedSkill == 0 || selectedSkill == 1)
                    {
                        selectedSkill += 8;
                    }
                    else
                    {
                        selectedSkill -= 2;

                    }
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (selectedSkill == 8 || selectedSkill == 9)
                    {
                        selectedSkill -= 8;
                    }
                    else
                    {
                        selectedSkill += 2;

                    }
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (selectedSkill == 0)
                    {
                        selectedSkill += 9;
                    }
                    else
                    {
                        selectedSkill--;

                    }
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (selectedSkill == 9)
                    {
                        selectedSkill -= 9;
                    }
                    else
                    {
                        selectedSkill++;

                    }
                }

                selectedSkill = Mathf.Clamp(selectedSkill, 0, skillSlots.Length - 1);

                for (int i = 0; i < skillSlots.Length; i++)
                {
                    bool selected = selectedSkill == i;
                    if (selected)
                    {
                        if (CanSelectedSkill())
                        {
                            descriptionPanel.SetActive(true);
                            if (Moves[i].Base.Category1 == MoveCategory1.Skill || Moves[i].Base.Category1 == MoveCategory1.Stat || Moves[i].Base.Category1 == MoveCategory1.Physical)
                            {
                                informationText.text = "戦闘中専用スキル";
                                informationValueText.text = "使用不可";
                            }
                            else if (Moves[i].Base.Category1 == MoveCategory1.FullHeal)
                            {
                                informationText.text = $"{Moves[SelectedSkill].Base.Information}";
                                informationValueText.text = $"HP全回復";

                            }
                            else
                            {
                                informationText.text = $"{Moves[SelectedSkill].Base.Information}";
                                informationValueText.text = $"{Moves[selectedSkill].Base.Power}";
                            }
                        }
                        else
                        {
                            descriptionPanel.SetActive(false);
                        }
                    }
                    skillSlots[i].SetSelectedColor(selected);
                }
                break;
            case SkillStatus.SkillUseCharaSelect:
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (selectedSkillUseChara == 0)
                    {
                        selectedSkillUseChara++;
                    }
                    else
                    {
                        selectedSkillUseChara--;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (selectedSkillUseChara == 1)
                    {
                        selectedSkillUseChara--;
                    }
                    else
                    {
                        selectedSkillUseChara++;
                    }
                }

                selectedSkillUseChara = Mathf.Clamp(selectedSkillUseChara, 0, 1);

                for (int i = 0; i < skillUseCharaSlots.Length; i++)
                {
                    bool selected = selectedSkillUseChara == i;
                    skillUseCharaSlots[i].SetSelectedColor(selected);
                }

                break;
        }
    }

    public bool CanSelectedSkill()
    {
        if (Moves[selectedSkill] == null)
        {
            return false;
        }

        return true;
    }


    public IEnumerator Skill()
    {
        if (player.Battlers[selectedChara].Moves[selectedSkill] == null || player.Battlers[selectedChara].Moves[selectedSkill].Base.Category1 == MoveCategory1.Skill ||
            player.Battlers[selectedChara].Moves[selectedSkill].Base.Category1 == MoveCategory1.Physical || player.Battlers[selectedChara].Moves[selectedSkill].Base.Category1 == MoveCategory1.Stat)
        {
            yield return StartCoroutine(DialogManager.Instance.TypeDialog("戦闘外で使うスキルじゃないようだ"));
            OnSkill?.Invoke();
        }
        else
        {
            Move move = player.Battlers[selectedChara].Moves[selectedSkill];
            if (move.Base.Mp > player.Battlers[selectedChara].MP)
            {
                yield return StartCoroutine(DialogManager.Instance.TypeDialog($"MPがたりない！"));
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                OnSkill?.Invoke();
            }
            else if (move.Base.Category1 == MoveCategory1.Heal)
            {
                int healPoint = 0;
                if (player.Battlers[selectedSkillUseChara].HP == player.Battlers[selectedSkillUseChara].MaxHP)
                {
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{player.Battlers[selectedSkillUseChara].Base.Name}のHPは減ってない！"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    DialogManager.Instance.Close();
                    descriptionPanel.SetActive(false);
                    OnSkill?.Invoke();
                    yield break;
                }
                else if (player.Battlers[selectedSkillUseChara].HP > 0)
                {
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{player.Battlers[selectedSkillUseChara].Base.Name}のはまだ死んでいない"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    DialogManager.Instance.Close();
                    descriptionPanel.SetActive(false);
                    OnSkill?.Invoke();
                    yield break;
                }
                else
                {
                    player.Battlers[selectedChara].MP -= player.Battlers[selectedChara].Moves[selectedSkill].Base.Mp;
                    SkillShow();
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{move.Base.Name}を使った"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    healPoint += Mathf.Clamp(healPoint + move.Base.Power, 0, player.Battlers[selectedSkillUseChara].MaxHP);
                    player.Battlers[selectedSkillUseChara].HP += healPoint;
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"HPが{healPoint}回復した"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    descriptionPanel.SetActive(false);
                    DialogManager.Instance.Close();
                    SkillUseCharaShow();
                    OnSkill?.Invoke();
                }
            }
            else if (move.Base.Category1 == MoveCategory1.FullHeal)
            {
                if (player.Battlers[selectedSkillUseChara].HP == player.Battlers[selectedSkillUseChara].MaxHP)
                {
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{player.Battlers[selectedSkillUseChara].Base.Name}のHPは減ってない！"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    DialogManager.Instance.Close();
                    descriptionPanel.SetActive(false);
                    OnSkill?.Invoke();
                    yield break;
                }
                else if (player.Battlers[selectedSkillUseChara].HP > 0)
                {
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{player.Battlers[selectedSkillUseChara].Base.Name}はまだ死んでいない"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    DialogManager.Instance.Close();
                    descriptionPanel.SetActive(false);
                    OnSkill?.Invoke();
                    yield break;
                }
                else
                {
                    player.Battlers[selectedChara].MP -= player.Battlers[selectedChara].Moves[selectedSkill].Base.Mp;
                    SkillShow();
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{move.Base.Name}を使った"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    player.Battlers[selectedSkillUseChara].HP = player.Battlers[selectedSkillUseChara].MaxHP;
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"HPが全回復した"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    descriptionPanel.SetActive(false);
                    DialogManager.Instance.Close();
                    SkillUseCharaShow();
                    OnSkill?.Invoke();
                }
            }
            else if (move.Base.Category1 == MoveCategory1.Resuscitation)
            {
                if (player.Battlers[selectedSkillUseChara].HP > 0)
                {
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{player.Battlers[selectedSkillUseChara].Base.Name}はまだ死んでいない"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    DialogManager.Instance.Close();
                    descriptionPanel.SetActive(false);
                    OnSkill?.Invoke();
                    yield break;
                }
                else if (player.Battlers[selectedSkillUseChara].HP <= 0)
                {
                    player.Battlers[selectedChara].MP -= player.Battlers[selectedChara].Moves[selectedSkill].Base.Mp;
                    SkillShow();
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{move.Base.Name}を使った"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    player.Battlers[selectedSkillUseChara].HP = player.Battlers[selectedSkillUseChara].MaxHP;
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{player.Battlers[selectedSkillUseChara].Base.Name}が生き返った"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    descriptionPanel.SetActive(false);
                    DialogManager.Instance.Close();
                    SkillUseCharaShow();
                    OnSkill?.Invoke();
                }
            }
            else if (move.Base.Category1 == MoveCategory1.Field)
            {
                if (player.EncountMod > 5 || player.EncountMod < 5)
                {
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{move.Base.Name}は使わなくてもいいようだ"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    DialogManager.Instance.Close();
                    descriptionPanel.SetActive(false);
                    OnSkill?.Invoke();
                    yield break;
                }
                else
                {
                    player.Battlers[selectedChara].MP -= player.Battlers[selectedChara].Moves[selectedSkill].Base.Mp;
                    SkillShow();
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{move.Base.Name}を使った"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    player.EncountMod = move.Base.Power;
                    player.EncountSkill = move.Base.SkillPower;
                    if (player.EncountMod == 5)
                    {
                        if (player.EncountMod > 5)
                        {
                            yield return StartCoroutine(DialogManager.Instance.TypeDialog($"モンスターの遭遇率が上がった！"));
                            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                        }
                        else if (player.EncountMod < 5)
                        {
                            yield return StartCoroutine(DialogManager.Instance.TypeDialog($"モンスターの遭遇率が下がった！"));
                            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                        }
                        descriptionPanel.SetActive(false);
                        DialogManager.Instance.Close();
                        SkillUseCharaShow();
                        OnSkill?.Invoke();
                        yield break;
                    }
                    else
                    {
                        if (player.EncountMod > 10)
                        {
                            yield return StartCoroutine(DialogManager.Instance.TypeDialog($"モンスターの遭遇率が上がった！"));
                            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                        }
                        else if (player.EncountMod < 10)
                        {
                            yield return StartCoroutine(DialogManager.Instance.TypeDialog($"モンスターの遭遇率が下がった！"));
                            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                        }
                        descriptionPanel.SetActive(false);
                        DialogManager.Instance.Close();
                        SkillUseCharaShow();
                        OnSkill?.Invoke();
                        yield break;
                    }
                }
            }
        }
    }


    public void SkillCharaSelectOpen()
    {
        state = SkillStatus.CharaSelect;
        skillCharaPanel.SetActive(true);
        SkillCharaInit();
    }

    public void SkillCharaSelectClose()
    {
        skillCharaPanel.SetActive(false);
    }

    public void SkillSelectOpen()
    {
        state = SkillStatus.SkillSelect;
        skillPanel.SetActive(true);
        SkillInit();
    }

    public void SkillSelectClose()
    {
        skillPanel.SetActive(false);
        descriptionPanel.SetActive(false);
        SkillCharaSelectOpen();
    }
    public void UseSkillCharaSelectOpen()
    {
        selectedSkillUseChara = 0;
        state = SkillStatus.SkillUseCharaSelect;
        skillUseCharaPanel.SetActive(true);
        SkillUseInit();
    }

    public void UseSkillCharaSelectClose()
    {
        skillUseCharaPanel.SetActive(false);
        descriptionPanel.SetActive(false);
        SkillSelectOpen();
    }
}