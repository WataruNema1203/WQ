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
        //Enemy:画像と名前の設定
        Image.color = color;
        Image.sprite = battler.Base.Sprite;
        originalpos = gameObject.transform.position;
        selectPos = originalpos;
        
    }

    //選択中なら色を変える
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
