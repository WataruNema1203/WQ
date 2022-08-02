using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class AttackMoveBase : MoveBase
{

    public override string RunMoveResult(Battler attacker, Battler target,int damage)
    {

        return $"{attacker.Base.Name}の{Name}\n{target.Base.Name}は{damage}のダメージ";
    }

}
