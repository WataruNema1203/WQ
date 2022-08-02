using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class AttackMoveBase : MoveBase
{

    public override string RunMoveResult(Battler attacker, Battler target,int damage)
    {

        return $"{attacker.Base.Name}‚Ì{Name}\n{target.Base.Name}‚Í{damage}‚Ìƒ_ƒ[ƒW";
    }

}
