using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUnit :BattleUnit
{

    public override void Setup(Battler battler)
    {
        base.Setup(battler);
        //Enemy:�摜�Ɩ��O�̐ݒ�
        Image.sprite = battler.Base.Sprite;
        NameText.text = battler.Base.Name;
        
    }

    


}
