using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//�Z�̊�b�f�[�^
[CreateAssetMenu]
public class MoveBase : ScriptableObject
{
    [SerializeField] new string name;
    [TextArea]
    [SerializeField] string information;
    [SerializeField] int power;
    [SerializeField] int skillPower;
    [SerializeField] int mp;

    //�J�e�S���[�i�����E����E�X�e�[�^�X�ω��j
    [SerializeField] MoveCategory1 category1;
    [SerializeField] MoveCategory2 category2;
    //�^�[�Q�b�g
    [SerializeField] MoveTarget target;
    //�X�e�[�^�X�ω��̃��X�g:�ǂ̃X�e�[�^�X���ǂ̒��x�ω�������̂��H�̃��X�g
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


//�ǂ̃X�e�[�^�X���ǂ̒��x�ω������邩
[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}

//���̃��X�g
[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] ConditionID status;//�˂ނ�A�ǂ��A�܂ЁA������A�₯��
    [SerializeField] ConditionID volatileStatus;//������

    public List<StatBoost> Boosts { get => boosts; }
    public ConditionID Status { get => status; }
    public ConditionID VolatileStatus { get => volatileStatus; }
}
//�ǉ����ʂ̎���
[System.Serializable]
public class SecondaryEffects : MoveEffects
{
    [SerializeField] int chance;//�ǉ����ʖ�����
    [SerializeField] MoveTarget target;//�ǉ����ʖ�����

    public int Chance { get => chance; }
    public MoveTarget Target { get => target; }
}


