using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUnit : BattleUnit
{
    [SerializeField] Text levelText;
    [SerializeField] Text hpText;
    [SerializeField] Text mpText;

    public override void Setup(Battler battler)
    {
        base.Setup(battler);
        //Player：名前とステータスの設定
        NameText.text = battler.Base.Name;
        levelText.text = $"Level：{battler.Level}";
        hpText.text = $"HP：{battler.HP}/{battler.MaxHP}";
        mpText.text = $"MP：{ battler.MP}/{ battler.MaxMP}";

    }

    public override void UpdateUI()
    {
        levelText.text = $"Level：{Battler.Level}";
        hpText.text = $"HP：{Battler.HP}/{Battler.MaxHP}";
        mpText.text = $"MP：{ Battler.MP}/{ Battler.MaxMP}";
    }
}
