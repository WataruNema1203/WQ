using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUnit : BattleUnit
{
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] TextMeshProUGUI mpText;
    TextMeshProUGUI[] texts;


    Color dai =Color.red;
    Color none = new Color(0, 0, 0, 255);
    Color lowContinuation = new Color(200, 0, 255, 255);
    Color highContinuation = new Color(135, 0, 110, 255);
    Color barn = new Color(65, 0, 185, 255);
    Color binding = new Color(255, 0, 0, 255);
    Color freeze = new Color(27, 44, 120, 255);
    Color paralisis = new Color(255, 255, 0, 255);
    Color confusion = new Color(255, 0, 255, 255);


    public override void Setup(Battler battler)
    {
        texts = GetComponentsInChildren<TextMeshProUGUI>();

        if (battler.HP <= 0)
        {
            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].color = dai;
            }
        }

        base.Setup(battler);
        //Player�F���O�ƃX�e�[�^�X�̐ݒ�
        NameText.text = battler.Base.Name;
        levelText.text = $"LV�F{battler.Level}";
        hpText.text = $"HP�F{battler.HP}/{battler.MaxHP}";
        mpText.text = $"MP�F{ battler.MP}/{ battler.MaxMP}";
    }

    public override void UpdateUI()
    {
        if (Battler.HP <= 0)
        {
            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].color = dai;
            }
        }

        levelText.text = $"LV�F{Battler.Level}";
        hpText.text = $"HP�F{Battler.HP}/{Battler.MaxHP}";
        mpText.text = $"MP�F{ Battler.MP}/{ Battler.MaxMP}";
    }
}
