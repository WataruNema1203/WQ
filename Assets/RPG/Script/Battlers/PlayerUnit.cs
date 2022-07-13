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
        //Player�F���O�ƃX�e�[�^�X�̐ݒ�
        NameText.text = battler.Base.Name;
        levelText.text = $"Level�F{battler.Level}";
        hpText.text = $"HP�F{battler.HP}/{battler.MaxHP}";
        mpText.text = $"MP�F{ battler.MP}/{ battler.MaxMP}";

    }

    public override void UpdateUI()
    {
        levelText.text = $"Level�F{Battler.Level}";
        hpText.text = $"HP�F{Battler.HP}/{Battler.MaxHP}";
        mpText.text = $"MP�F{ Battler.MP}/{ Battler.MaxMP}";
    }
}
