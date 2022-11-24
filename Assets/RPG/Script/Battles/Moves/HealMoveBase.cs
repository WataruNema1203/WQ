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
            Heal(attacker, target, damage);
            return $"{attacker.Name}‚Ì{Name}\n{attacker.Name}‚Í{attacker.MaxHP}‰ñ•œ";
        }
        else
        {
            Heal(attacker, target, damage);
            return $"{attacker.Name}‚Ì{Name}\n{attacker.Name}‚Í{healPoint + 1}‰ñ•œ";

        }
    }

    private void Heal(Battler attacker, Battler target, int damage)
    {
        if (full == true)
        {
            attacker.HP += damage;
            attacker.Heal(attacker.MaxHP);
            int fullHeal = attacker.MaxHP;
            attacker.HP = Mathf.Clamp(attacker.HP + fullHeal, 0, attacker.MaxHP);
        }
        else
        {
            attacker.HP += damage;
            float modifiers = Random.Range(0.85f, 1f);
            float a = (2 * attacker.Level + 10) / 250f;
            float d = (a * HealPoint) + attacker.Intelligence + 2;
            int healPoint = Mathf.FloorToInt(d * modifiers);
            attacker.HP = Mathf.Clamp(attacker.HP + healPoint, 0, attacker.MaxHP);
        }

    }
}
