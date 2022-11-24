using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyUnit :BattleUnit
{
    Color color = new Color(255,255,255,255);
    Vector3 originalpos;
    Vector3 selectPos;

    public override void Setup(Battler battler)
    {
        SetImage();
        base.Setup(battler);
        //Enemy:�摜�Ɩ��O�̐ݒ�
        Image.color = color;
        Image.sprite = battler.Base.Sprite;
        originalpos = gameObject.transform.position;
        selectPos = originalpos;
        
    }

    //�I�𒆂Ȃ�F��ς���
    public void SetSelected(bool selected)
    {
        if (selected)
        {
            selectPos.y = originalpos.y + 0.05f;
            gameObject.transform.position = selectPos;
        }
        else
        {
            selectPos = originalpos;
            gameObject.transform.position = selectPos;
        }
    }



}
