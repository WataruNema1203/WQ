using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MPItemBase : ItemBase
{
    [SerializeField] int point;
    public int Point { get => point; }

    public override void Use(Battler target)
    {
        target.MP = Mathf.Clamp(target.MP + Point, 0, target.MaxMP);
    }
}
