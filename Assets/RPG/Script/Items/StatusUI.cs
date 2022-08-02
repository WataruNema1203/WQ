using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] GameObject playerStatusUI;
    [SerializeField] GameObject aminaStatusUI;
    [SerializeField] GameObject playGorld;
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text hpText;
    [SerializeField] Text mpText;
    [SerializeField] Text conditionText;
    [SerializeField] Text attackText;
    [SerializeField] Text defenceText;
    [SerializeField] Text intelligenceText;
    [SerializeField] Text mindText;
    [SerializeField] Text expText;
    [SerializeField] Text equipWeaponText;
    [SerializeField] Text equipArmorText;
    [SerializeField] Text equipAccessoryText;
    [SerializeField] Text playTimeText;
    [SerializeField] Text HasGorldText;
    [SerializeField] Text aminaNameText;
    [SerializeField] Text aminaLevelText;
    [SerializeField] Text aminaHpText;
    [SerializeField] Text aminaMpText;
    [SerializeField] Text aminaConditionText;
    [SerializeField] Text aminaAttackText;
    [SerializeField] Text aminaDefenceText;
    [SerializeField] Text amiaIntelligenceText;
    [SerializeField] Text aminaMindText;
    [SerializeField] Text aminaExpText;
    [SerializeField] Text aminaEquipWeaponText;
    [SerializeField] Text aminaEquipArmorText;
    [SerializeField] Text aminaEquipAccessoryText;

    [SerializeField]
    private int minute;
    [SerializeField]
    private float seconds;
    //�@�O��Update�̎��̕b��
    private float oldSeconds;
    //�@�^�C�}�[�\���p�e�L�X�g
    private string timerText;
    Text[] playerTexts;
    Text[] aminaTexts;
    Color dei = Color.red;
    Color alive = Color.white;

    private void Awake()
    {
        playerTexts = playerStatusUI.GetComponentsInChildren<Text>();
        aminaTexts = aminaStatusUI.GetComponentsInChildren<Text>();
    }

    private void Init()
    {
        if (player.Battlers[0].HP <= 0)
        {
            for (int i = 0; i < playerTexts.Length; i++)
            {
                playerTexts[i].color = dei;
            }
        }
        else if ((player.Battlers[0].HP >= 0))
        {
            for (int i = 0; i < playerTexts.Length; i++)
            {
                playerTexts[i].color = alive;
            }
        }
        if (player.Battlers.Count == 2)
        {
            if (player.Battlers[1].HP <= 0)
            {
                for (int i = 0; i < aminaTexts.Length; i++)
                {
                    aminaTexts[i].color = dei;
                }
            }
            else if ((player.Battlers[1].HP >= 0))
            {
                for (int i = 0; i < aminaTexts.Length; i++)
                {
                    aminaTexts[i].color = alive;
                }
            }

        }
        playerStatusUI.transform.Find("NameText").GetComponent<Text>().text = $"���O�F{ player.Battlers[0].Base.Name}";
        //nameText.text = "";
        levelText.text = $"���x���F{player.Battlers[0].Level}";
        hpText.text = $"HP�F{ player.Battlers[0].HP}/{player.Battlers[0].MaxHP}";
        mpText.text = $"MP�F{ player.Battlers[0].MP}/{player.Battlers[0].MaxMP}";
        if (player.Battlers[0].HP <= 0)
        {
            conditionText.text = "��ԁF���S";
        }
        else if (player.Battlers[0].Status == null)
        {
            conditionText.text = "��ԁF����";
        }
        else
        {
            conditionText.text = $"��ԁF{player.Battlers[0].Status.Name}";
        }
        attackText.text = $"�U���́F{player.Battlers[0].Attack + player.Battlers[0].Base.GetEquipWeapon().Base.GetAmount() + player.Battlers[0].Base.GetEquipAccessory().Base.GetAmount()}";
        defenceText.text = $"�h��́F{ player.Battlers[0].Defense + player.Battlers[0].Base.GetEquipArmor().Base.GetAmount()}";
        intelligenceText.text = $"���@�́F{player.Battlers[0].Intelligence + player.Battlers[0].Base.GetEquipWeapon().Base.GetMagicAmount() + player.Battlers[0].Base.GetEquipAccessory().Base.GetMagicAmount()}";
        mindText.text = $"���_�́F{ player.Battlers[0].Mind + player.Battlers[0].Base.GetEquipArmor().Base.GetMagicAmount()}";
        expText.text = $"����Lv�܂ŁF{ ((400 * player.Battlers[0].Level * player.Battlers[0].Level / (204 - player.Battlers[0].Level)) + 5) - player.Battlers[0].HasExp}";
        equipWeaponText.text = $"����F{player.Battlers[0].Base.GetEquipWeapon().Base.GetKanjiName()}";
        equipArmorText.text = $"�h��F{player.Battlers[0].Base.GetEquipArmor().Base.GetKanjiName()}";
        equipAccessoryText.text = $"�����i�F{player.Battlers[0].Base.GetEquipAccessory().Base.GetKanjiName()}";
        HasGorldText.text = $"����G�F{player.Battlers[0].Gold}";

        if (player.Battlers.Count == 2)
        {
            aminaStatusUI.SetActive(true);

            aminaNameText.text = $"���O�F{ player.Battlers[1].Base.Name}";
            aminaLevelText.text = $"���x���F{player.Battlers[1].Level}";
            aminaHpText.text = $"HP�F{ player.Battlers[1].HP}/{player.Battlers[1].MaxHP}";
            aminaMpText.text = $"MP�F{ player.Battlers[1].MP}/{player.Battlers[1].MaxMP}";
            if (player.Battlers[1].HP <= 0)
            {
                aminaConditionText.text = "��ԁF���S";
            }
            else if (player.Battlers[1].Status == null)
            {
                aminaConditionText.text = "��ԁF����";
            }
            else
            {
                aminaConditionText.text = $"��ԁF{player.Battlers[1].Status.Name}";
            }
            aminaAttackText.text = $"�U���́F{player.Battlers[1].Attack + player.Battlers[1].Base.GetEquipWeapon().Base.GetAmount() + player.Battlers[1].Base.GetEquipAccessory().Base.GetAmount()}";
            aminaDefenceText.text = $"�h��́F{ player.Battlers[1].Defense + player.Battlers[1].Base.GetEquipArmor().Base.GetAmount()}";
            amiaIntelligenceText.text = $"���@�́F{player.Battlers[1].Intelligence + player.Battlers[1].Base.GetEquipWeapon().Base.GetMagicAmount() + player.Battlers[1].Base.GetEquipAccessory().Base.GetMagicAmount()}";
            aminaMindText.text = $"���_�́F{ player.Battlers[1].Mind + player.Battlers[1].Base.GetEquipArmor().Base.GetMagicAmount()}";
            aminaExpText.text = $"����Lv�܂ŁF{ ((400 * player.Battlers[1].Level * player.Battlers[1].Level / (204 - player.Battlers[1].Level)) + 5) - player.Battlers[1].HasExp}";
            aminaEquipWeaponText.text = $"����F{player.Battlers[1].Base.GetEquipWeapon().Base.GetKanjiName()}";
            aminaEquipArmorText.text = $"�h��F{player.Battlers[1].Base.GetEquipArmor().Base.GetKanjiName()}";
            aminaEquipAccessoryText.text = $"�����i�F{player.Battlers[1].Base.GetEquipAccessory().Base.GetKanjiName()}";
        }

    }
    private void Update()
    {
        seconds += Time.deltaTime;
        if (seconds >= 60f)
        {
            minute++;
            seconds -= 60;
        }
        //�@�l���ς�����������e�L�X�gUI���X�V
        if ((int)seconds != (int)oldSeconds)
        {
            timerText = $"Play�F{minute.ToString("00")}" + ":" + $"{((int)seconds).ToString("00")}";
        }
        oldSeconds = seconds;

        playTimeText.text = $"{timerText}";

    }

    public void Open()
    {
        playerStatusUI.SetActive(true);
        playGorld.SetActive(true);
        if (player.Battlers.Count == 2)
        {
            aminaStatusUI.SetActive(true);
        }
        Init();
    }
    public void Close()
    {
        playerStatusUI.SetActive(false);
        playGorld.SetActive(false);
        if (player.Battlers.Count == 2)
        {
            aminaStatusUI.SetActive(false);
        }
    }

    public void Set(Battler battler)
    {
        nameText.text = battler.Base.Name;
        UpdateUI(battler);
    }
    public void AminaSet(Battler battler)
    {
        aminaNameText.text = battler.Base.Name;
        AminaUpdateUI(battler);
    }

    public void UpdateUI(Battler battler)
    {
        levelText.text = battler.Level.ToString();
        hpText.text = $"{battler.HP}/{battler.MaxHP}";
        mpText.text = $"{battler.MP}/{battler.MaxMP}";
        if (battler.HP <= 0)
        {
            conditionText.text = "��ԁF���S";
        }
        else if (battler.Status == null)
        {
            conditionText.text = "��ԁF����";
        }
        else
        {
            conditionText.text = $"��ԁF{battler.Status.Name}";
        }
    }

    public void AminaUpdateUI(Battler battler)
    {
        aminaLevelText.text = battler.Level.ToString();
        aminaHpText.text = $"{battler.HP}/{battler.MaxHP}";
        aminaMpText.text = $"{battler.MP}/{battler.MaxMP}";
        if (battler.HP <= 0)
        {
            aminaConditionText.text = "��ԁF���S";
        }
        else if (battler.Status == null)
        {
            aminaConditionText.text = "��ԁF����";
        }
        else
        {
            aminaConditionText.text = $"��ԁF{battler.Status.Name}";
        }

    }


}
