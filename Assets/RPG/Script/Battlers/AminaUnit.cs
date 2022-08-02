using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AminaUnit : BattleUnit
{
    [SerializeField] Text levelText;
    [SerializeField] Text hpText;
    [SerializeField] Text mpText;
    [SerializeField] Text conditionText;
    Text[] playerTexts;


    Color dai = Color.red;
    Color alive = Color.white;
    public override void Setup(Battler battler)
    {
        playerTexts = GetComponentsInChildren<Text>();

        if (battler.HP <= 0)
        {
            for (int i = 0; i < playerTexts.Length; i++)
            {
                playerTexts[i].color = dai;
            }
        }
        else if ((battler.HP >= 0))
        {
            for (int i = 0; i < playerTexts.Length; i++)
            {
                playerTexts[i].color = alive;
            }
        }


        base.Setup(battler);
        //Player�F���O�ƃX�e�[�^�X�̐ݒ�
        NameText.text = battler.Base.Name;
        levelText.text = $"LV�F{battler.Level}";
        hpText.text = $"HP�F{battler.HP}/{battler.MaxHP}";
        mpText.text = $"MP�F{ battler.MP}/{ battler.MaxMP}";
        if (battler.HP <= 0)
        {
            conditionText.text = "RIP";
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

    public override void UpdateUI()
    {
        if (Battler.HP <= 0)
        {
            for (int i = 0; i < playerTexts.Length; i++)
            {
                playerTexts[i].color = dai;
            }
        }
        else if ((Battler.HP >= 0))
        {
            for (int i = 0; i < playerTexts.Length; i++)
            {
                playerTexts[i].color = alive;
            }
        }


        levelText.text = $"LV�F{Battler.Level}";
        hpText.text = $"HP�F{Battler.HP}/{Battler.MaxHP}";
        mpText.text = $"MP�F{ Battler.MP}/{ Battler.MaxMP}";
        if (Battler.HP <= 0)
        {
            conditionText.text = "RIP";
        }
        else if (Battler.Status == null)
        {
            conditionText.text = "��ԁF����";
        }
        else
        {
            conditionText.text = $"��ԁF{Battler.Status.Name}";
        }
    }
}
