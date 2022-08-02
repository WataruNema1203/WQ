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
        //Player：名前とステータスの設定
        NameText.text = battler.Base.Name;
        levelText.text = $"LV：{battler.Level}";
        hpText.text = $"HP：{battler.HP}/{battler.MaxHP}";
        mpText.text = $"MP：{ battler.MP}/{ battler.MaxMP}";
        if (battler.HP <= 0)
        {
            conditionText.text = "RIP";
        }
        else if (battler.Status == null)
        {
            conditionText.text = "状態：正常";
        }
        else
        {
            conditionText.text = $"状態：{battler.Status.Name}";
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


        levelText.text = $"LV：{Battler.Level}";
        hpText.text = $"HP：{Battler.HP}/{Battler.MaxHP}";
        mpText.text = $"MP：{ Battler.MP}/{ Battler.MaxMP}";
        if (Battler.HP <= 0)
        {
            conditionText.text = "RIP";
        }
        else if (Battler.Status == null)
        {
            conditionText.text = "状態：正常";
        }
        else
        {
            conditionText.text = $"状態：{Battler.Status.Name}";
        }
    }
}
