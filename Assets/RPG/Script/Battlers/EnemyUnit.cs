using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUnit :BattleUnit
{

    public override void Setup(Battler battler)
    {
        base.Setup(battler);
        //Enemy:‰æ‘œ‚Æ–¼‘O‚Ìİ’è
        Image.sprite = battler.Base.Sprite;
        NameText.text = battler.Base.Name;
        
    }

    


}
