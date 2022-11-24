using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using TMPro;


enum SkillStatus
{
    Busy,
    CharaSelect,
    SkillSelect,
    SkillUseCharaSelect,
    WarpPointSelect,
}

public class FieldSkillController : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] GameObject descriptionPanel;
    [SerializeField] GameObject skillCharaPanel;
    [SerializeField] GameObject skillPanel;
    [SerializeField] GameObject skillUseCharaPanel;
    [SerializeField] GameObject warpPointPanel;
    [SerializeField] WarpPointManager warpPoint;
    [SerializeField] WarpSkillController warp;
    [SerializeField] TextMeshProUGUI informationText;
    [SerializeField] TextMeshProUGUI informationValueText;

    SkillUI[] skillSlots;
    SkillCharaUI[] skillCharaSlots;
    SkillUseUI[] skillUseCharaSlots;
    WarpPointUI[] warpPoints;
    int selectedChara;
    int selectedSkill;
    int selectedSkillUseChara;
    int selectedWarpPoint;
    SkillStatus state;

    public List<WarpPointBase> Warps { get; set; }

    //覚える技
    public List<Move> FieldMoves { get; set; }
    //現在の技
    public Move CurrentMove { get; set; }

    public int SelectedSkill { get => selectedSkill; }
    internal SkillStatus State { get => state; }
    public int SelectedItemUseChara { get => selectedSkillUseChara; }
    public int SelectedChara { get => selectedChara; }
    public int SelectedWarpPoint { get => selectedWarpPoint; }
    public GameObject WarpPointPanel { get => warpPointPanel; set => warpPointPanel = value; }

    public UnityAction OnSkill;



    public void SkillCharaInit()
    {
        skillCharaSlots = GetComponentsInChildren<SkillCharaUI>();

        SkillCharaShow();
    }

    public void WarpPointInit()
    {
        warpPoints = GetComponentsInChildren<WarpPointUI>();
        Warps = new List<WarpPointBase>(10);
        for (int i = 0; i < warpPoint.WarpPoints.Count; i++)
        {
            Warps.Add(warpPoint.WarpPoints[i]);
        }

        WarpPointShow();
    }

    public void SkillInit()
    {
        skillSlots = GetComponentsInChildren<SkillUI>();

        //覚える技から使える技を生成
        FieldMoves = new List<Move>(10);
        foreach (var learnableMove in player.Battlers[selectedChara].Base.LearnableMoves)
        {
            if (learnableMove.Level <= player.Battlers[selectedChara].Level)
            {
                if (learnableMove.MoveBase.Category1 == MoveCategory1.FieldSkill)
                {
                    FieldMoves.Add(new Move(learnableMove.MoveBase));
                }
            }
        }

        SkillShow();
    }
    public void SkillUseInit()
    {
        skillUseCharaSlots = GetComponentsInChildren<SkillUseUI>();

        SkillUseCharaShow();
    }

    void SkillCharaShow()
    {
        for (int i = 0; i < skillCharaSlots.Length; i++)
        {
            if (i == 0)
            {
                skillCharaSlots[i].SetText($"{player.Battlers[i].Base.Name}");
            }
            else if (i == 1)
            {
                if (player.Battlers.Count == 2)
                {
                    skillCharaSlots[i].SetText($"{player.Battlers[i].Base.Name}");
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
            if (FieldMoves.Count <= 10)
            {
                FieldMoves.Add(null);
            }
        }
        for (int i = 0; i < skillSlots.Length; i++)
        {

            if (FieldMoves[i] == null)
            {
                skillSlots[i].SetText($"-");
                skillSlots[i].transform.Find("SkillMPText").GetComponent<TextMeshProUGUI>().text = " ";

            }
            else if (FieldMoves[i].Base.Category1 != MoveCategory1.FieldSkill)
            {
                skillSlots[i].SetText($"{FieldMoves[i].Base.Name}");
                skillSlots[i].transform.Find("SkillMPText").GetComponent<TextMeshProUGUI>().text = "";

            }
            else if (FieldMoves[i].Base == true)
            {
                skillSlots[i].SetText($"{FieldMoves[i].Base.Name}");
                skillSlots[i].transform.Find("SkillMPText").GetComponent<TextMeshProUGUI>().text = "MP  " + $"{FieldMoves[i].Base.Mp}";

            }
        }
    }
    void WarpPointShow()
    {
        for (int i = 0; i < warpPoints.Length; i++)
        {
            if (Warps.Count <= 10)
            {
                Warps.Add(null);
            }
        }
        for (int i = 0; i < warpPoints.Length; i++)
        {
            if (Warps[i] == null)
            {
                warpPoints[i].SetText($"-");
            }
            else if (Warps[i])
            {
                warpPoints[i].SetText($"{Warps[i].PointName}");
            }
            else if (Warps[i].WarpPoint == true)
            {
                warpPoints[i].SetText($"{Warps[i].PointName}");
            }
        }
    }
    void SkillUseCharaShow()
    {
        for (int i = 0; i < skillUseCharaSlots.Length; i++)
        {
            if (i == 0)
            {
                skillUseCharaSlots[i].SetText($"{player.Battlers[i].Base.Name} HP{player.Battlers[0].HP}/{player.Battlers[0].MaxHP} MP{player.Battlers[0].MP}/{player.Battlers[0].MaxMP}");
            }
            else if (i == 1)
            {
                if (player.Battlers.Count == 2)
                {
                    skillUseCharaSlots[i].SetText($"{ player.Battlers[i].Base.Name} HP{player.Battlers[1].HP}/{player.Battlers[1].MaxHP} MP{player.Battlers[1].MP}/{player.Battlers[1].MaxMP}");
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
                            if (FieldMoves[i].Base.Category1 == MoveCategory1.Skill || FieldMoves[i].Base.Category1 == MoveCategory1.Stat || FieldMoves[i].Base.Category1 == MoveCategory1.Physical)
                            {
                                informationText.text = "戦闘中専用スキル";
                                informationValueText.text = "使用不可";
                            }
                            else if (FieldMoves[i].Base.Category1 == MoveCategory1.FullHeal)
                            {
                                informationText.text = $"{FieldMoves[SelectedSkill].Base.Information}";
                                informationValueText.text = $"HP全回復";

                            }
                            else
                            {
                                informationText.text = $"{FieldMoves[SelectedSkill].Base.Information}";
                                informationValueText.text = $"{FieldMoves[selectedSkill].Base.Power}";
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
            case SkillStatus.WarpPointSelect:
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (selectedWarpPoint == 0 || selectedWarpPoint == 1)
                    {
                        selectedWarpPoint += 8;
                    }
                    else
                    {
                        selectedWarpPoint -= 2;

                    }
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (selectedWarpPoint == 8 || selectedWarpPoint == 9)
                    {
                        selectedWarpPoint -= 8;
                    }
                    else
                    {
                        selectedWarpPoint += 2;

                    }
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (selectedWarpPoint == 0)
                    {
                        selectedWarpPoint += 9;
                    }
                    else
                    {
                        selectedWarpPoint--;

                    }
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (selectedWarpPoint == 9)
                    {
                        selectedWarpPoint -= 9;
                    }
                    else
                    {
                        selectedWarpPoint++;

                    }
                }

                selectedWarpPoint = Mathf.Clamp(selectedWarpPoint, 0, warpPoints.Length - 1);

                for (int i = 0; i < warpPoints.Length; i++)
                {
                    bool selected = selectedWarpPoint == i;
                    warpPoints[i].SetSelectedColor(selected);
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
        if (FieldMoves[selectedSkill] == null)
        {
            return false;
        }

        return true;
    }


    public IEnumerator Skill()
    {
        state = SkillStatus.Busy;
        Move fieldMoves = player.Battlers[selectedChara].FieldMoves[selectedSkill];

        if (fieldMoves.Base.Category1 != MoveCategory1.FieldSkill)
        {
            yield return StartCoroutine(DialogManager.Instance.TypeDialog("戦闘外で使うスキルじゃないようだ"));
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
            DialogManager.Instance.Close();
            OnSkill?.Invoke();
        }
        else if (fieldMoves.Base.Category2 == MoveCategory2.Warp&&GameController.Instance.IsOutSide==false)
        {
            yield return StartCoroutine(DialogManager.Instance.TypeDialog("ここではワープができない！"));
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
            DialogManager.Instance.Close();
            OnSkill?.Invoke();
        }
        else
        {
            if (fieldMoves.Base.Mp > player.Battlers[selectedChara].MP)
            {
                yield return StartCoroutine(DialogManager.Instance.TypeDialog($"MPがたりない！"));
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                DialogManager.Instance.Close();
                OnSkill?.Invoke();
            }
            else if (fieldMoves.Base.Category1 == MoveCategory1.Heal)
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
                else
                {
                    player.Battlers[selectedChara].MP -= fieldMoves.Base.Mp;
                    SkillShow();
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{fieldMoves.Base.Name}を使った"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    healPoint += Mathf.Clamp(healPoint + fieldMoves.Base.Power, 0, player.Battlers[selectedSkillUseChara].MaxHP);
                    player.Battlers[selectedSkillUseChara].HP += healPoint;
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"HPが{healPoint}回復した"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    descriptionPanel.SetActive(false);
                    DialogManager.Instance.Close();
                    SkillUseCharaShow();
                    OnSkill?.Invoke();
                }
            }
            else if (fieldMoves.Base.Category1 == MoveCategory1.FullHeal)
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
                    player.Battlers[selectedChara].MP -= fieldMoves.Base.Mp;
                    SkillShow();
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{fieldMoves.Base.Name}を使った"));
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
            else if (fieldMoves.Base.Category1 == MoveCategory1.Resuscitation)
            {
                if (!player.Battlers[selectedSkillUseChara].isDai)
                {
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{player.Battlers[selectedSkillUseChara].Base.Name}はまだ死んでいない"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    DialogManager.Instance.Close();
                    descriptionPanel.SetActive(false);
                    OnSkill?.Invoke();
                    yield break;
                }
                else if (player.Battlers[selectedSkillUseChara].isDai)
                {
                    player.Battlers[selectedChara].MP -= fieldMoves.Base.Mp;
                    SkillShow();
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{fieldMoves.Base.Name}を使った"));
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
            else if (fieldMoves.Base.Category2 == MoveCategory2.EncountSkill)
            {
                if (player.EncountMod > 5 || player.EncountMod < 5)
                {
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{fieldMoves.Base.Name}は使わなくてもいいようだ"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    DialogManager.Instance.Close();
                    descriptionPanel.SetActive(false);
                    OnSkill?.Invoke();
                    yield break;
                }
                else
                {
                    player.Battlers[selectedChara].MP -= fieldMoves.Base.Mp;
                    SkillShow();
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{fieldMoves.Base.Name}を使った"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    player.EncountMod = fieldMoves.Base.Power;
                    player.EncountSkill = fieldMoves.Base.SkillPower;
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
                        OnSkill?.Invoke();
                        yield break;
                    }
                }
            }
            else if (fieldMoves.Base.Category2 == MoveCategory2.Warp)
            {
                player.Battlers[selectedChara].MP -= fieldMoves.Base.Mp;
                SkillShow();
                warpPointPanel.SetActive(false);
                yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{fieldMoves.Base.Name}を使った"));
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                yield return StartCoroutine(FadeController.Instance.WarpFadeOut());
                Vector3 vector = warpPoint.WarpPoints[selectedWarpPoint].WarpPoint.position;
                player.transform.position = vector;
                CameraManager.Instance.transform.position = vector;
                yield return StartCoroutine(warp.WarpStart(warpPoint.WarpPoints[selectedWarpPoint]));
                yield return StartCoroutine(FadeController.Instance.WarpFadeIn());
                descriptionPanel.SetActive(false);
                DialogManager.Instance.Close();
                GameController.Instance.StartSelect();
                yield break;
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
        skillUseCharaPanel.SetActive(false);
        skillCharaPanel.SetActive(false);
        skillPanel.SetActive(true);
        SkillInit();
    }

    public void SkillSelectClose()
    {
        skillPanel.SetActive(false);
        descriptionPanel.SetActive(false);
        SkillCharaSelectOpen();
    }
    public void WarpPointSelectOpen()
    {
        state = SkillStatus.WarpPointSelect;
        warpPointPanel.SetActive(true);
        skillPanel.SetActive(false);
        descriptionPanel.SetActive(false);
        WarpPointInit();
    }

    public void WarpPointSelectClose()
    {
        warpPointPanel.SetActive(false);
        SkillSelectOpen();
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