using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class HealMoveBase : MoveBase
{
    [SerializeField] int healPoint;
    [SerializeField] bool full;


    public int HealPoint { get => healPoint; }

    public override string RunMoveResult(Battler attacker, Battler target, int damage)
    {
        if (full == true)
        {
            attacker.HP += damage;
            attacker.Heal(attacker.MaxHP);
            int fullHeal = attacker.MaxHP;
            attacker.HP = Mathf.Clamp(attacker.HP + fullHeal, 0, attacker.MaxHP);
            return $"{attacker.Base.Name}‚Ì{Name}\n{attacker.Base.Name}‚Í{fullHeal}‰ñ•œ";

        }
        else
        {
            attacker.HP += damage;
            float modifiers = Random.Range(0.85f, 1f);
            float a = (2 * attacker.Level + 10) / 250f;
            float d = (a * HealPoint) + attacker.Intelligence + 2;
            int healPoint = Mathf.FloorToInt(d * modifiers);
            attacker.HP = Mathf.Clamp(attacker.HP + healPoint, 0, attacker.MaxHP);
            return $"{attacker.Base.Name}‚Ì{Name}\n{attacker.Base.Name}‚Í{healPoint+1}‰ñ•œ";

        }
    }
}
