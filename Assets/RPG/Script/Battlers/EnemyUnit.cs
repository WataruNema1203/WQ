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
        //Enemy:‰æ‘œ‚Æ–¼‘O‚Ìİ’è
        Image.color = color;
        Image.sprite = battler.Base.Sprite;
        originalpos = gameObject.transform.position;
        selectPos = originalpos;
        
    }

    //‘I‘ğ’†‚È‚çF‚ğ•Ï‚¦‚é
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
