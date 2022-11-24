using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//技の基礎データ
[CreateAssetMenu]
public class MoveBase : ScriptableObject
{
    [SerializeField] new string name;
    [TextArea]
    [SerializeField] string information;
    [SerializeField] int power;
    [SerializeField] int skillPower;
    [SerializeField] int mp;

    //カテゴリー（物理・特殊・ステータス変化）
    [SerializeField] MoveCategory1 category1;
    [SerializeField] MoveCategory2 category2;
    //ターゲット
    [SerializeField] MoveTarget target;
    //ステータス変化のリスト:どのステータスをどの程度変化させるのか？のリスト
    [SerializeField] MoveEffects effects;
    [SerializeField] List<SecondaryEffects> secondaries;



    public string Name { get => name; }
    public MoveCategory1 Category1 { get => category1; }
    public MoveTarget Target { get => target; }
    public MoveEffects Effects { get => effects;}
    public List<SecondaryEffects> Secondaries { get => secondaries;}
    public int Power { get => power; }
    public int Mp { get => mp; }
    public MoveCategory2 Category2 { get => category2; }
    public string Information { get => information;}
    public int SkillPower { get => skillPower; }

    public virtual string RunMoveResult(Battler attacker, Battler target,int damage)
    {
        return "";
    }
}


public enum MoveCategory1
{
    Physical,
    Skill,
    Heal,
    FullHeal,
    Resuscitation,
    FieldSkill,
    Stat,
    All,
}

public enum MoveCategory2
{
    None,
    Heal,
    FullHeal,
    Resuscitation,
    EncountSkill,
    Warp,
}

public enum MoveTarget
{
    Foe,
    Self,
}


//どのステータスをどの程度変化させるか
[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}

//下のリスト
[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] ConditionID status;//ねむり、どく、まひ、こおり、やけど
    [SerializeField] ConditionID volatileStatus;//こんらん

    public List<StatBoost> Boosts { get => boosts; }
    public ConditionID Status { get => status; }
    public ConditionID VolatileStatus { get => volatileStatus; }
}
//追加効果の実装
[System.Serializable]
public class SecondaryEffects : MoveEffects
{
    [SerializeField] int chance;//追加効果命中率
    [SerializeField] MoveTarget target;//追加効果命中率

    public int Chance { get => chance; }
    public MoveTarget Target { get => target; }
}


