using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class Battler
{
    //�f�[�^�̏�����������ĂȂ���_base��level���g���Ă��Ȃ�
    //�C���X�y�N�^�[����f�[�^��ݒ�ł���悤�ɂ���
    [SerializeField] BattlerBase _base;
    [SerializeField] int level;
    //�x�[�X�ƂȂ�f�[�^
    public BattlerBase Base { get => _base; }
    public int Level { get => level; }
    public int HasExp { get; set; }
    public int Gold { get; set; }
    public int HP { get; set; }
    public int MP { get; set; }
    public int Value { get; set; }


    //�o����Z
    public List<Move> Moves { get; set; }
    //���݂̋Z
    public Move CurrentMove { get; set; }
    
    //�����X�e�[�^�X�ƒǉ��X�e�[�^�X
    public Dictionary<Stat, int> Stats { get; set; }
    public Dictionary<Stat, int> StatBoosts { get; set; }

    //���O�����߂Ă����ϐ������F�o�����ꂪ�ȒP�ȃ��X�g
    public Queue<string> StatusChanges { get; private set; }
    //���U���g�e�L�X�g�𑗂�ϐ�
    private string resultText;

    public string ResultText { get => resultText; }
    //��Ԉُ�
    public Condition Status { get; private set; }
    public int StatusTime { get; set; }

    //�X�e�[�^�X�ω����N�������Ƃ��Ɏ��s���������Ƃ�o�^���Ă����֐������
    public Action OnStatusChanged;

    //�o�g���I�����ɉ񕜂���FStatus�Ɠ����悤�ɐݒ肷��
    public Condition VolatileStatus { get; private set; }
    public int VolatileStatusTime { get; set; }



    readonly Dictionary<Stat, string> statDic = new Dictionary<Stat, string>()
    {
        {Stat.Attack, "�U����"},
        {Stat.Defense, "�h���"},
        {Stat.Intelligence ,"�m��"},
        {Stat.Mind ,"���_��"},

    };



    //������
    public void Init()
    {
        StatusChanges = new Queue<string>();
        //�o����Z����g����Z�𐶐�
        Moves = new List<Move>();
        foreach (var learnableMove in Base.LearnableMoves)
        {
            if (learnableMove.Level <= level)
            {
                Moves.Add(new Move(learnableMove.MoveBase));
            }
        }

        ResetStatBoost();
        CalculateStats();
        HP = MaxHP;
        MP = MaxMP;
        Status = null;
        VolatileStatus = null;
    }
    void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack ,0},
            {Stat.Defense ,0},
            {Stat.Intelligence ,0},
            {Stat.Mind ,0},
        };
    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>
        {
            //�X�e�[�^�X�ω����ĂȂ������l�����Ă���
            { Stat.Attack, Mathf.FloorToInt(Base.At * Level)+5},
            { Stat.Defense, Mathf.FloorToInt(Base.Def * Level) + 5 },
            {Stat.Intelligence, Mathf.FloorToInt(Base.Inte * Level) + 5 },
            {Stat.Mind, Mathf.FloorToInt(Base.Mid * Level) + 5 }
        };
        MaxHP = Mathf.FloorToInt(Base.MaxHP * Level) + 10 + Level;
        MaxMP = Mathf.FloorToInt(Base.MaxMP * Level)+ 10 + Level;
    }

    int GetStat(Stat stat)
    {
        int statValue = Stats[stat];
        //boost�̕����v�Z����
        int boost = StatBoosts[stat];
        float[] boostValues = new float[] { 1, 2, 4, 8, 16 };

        if (boost >= 0)
        {
            //�����Ȃ�
            statValue = Mathf.FloorToInt(statValue * boostValues[boost]);
        }
        else
        {
            //��̉��Ȃ�
            statValue = Mathf.FloorToInt(statValue / boostValues[-boost]);
        }


        return statValue;
    }

    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        //TODO�@�X�e�[�^�X�ω��𔽉f:StatBoosts��ύX����
        foreach (StatBoost statBoost in statBoosts)
        {
            //�ǂ̃X�e�[�g��
            Stat stat = statBoost.stat;
            //���i�K
            int boost = statBoost.boost;
            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);
            if (boost > 0)
            {
                StatusChanges.Enqueue($"{Base.Name}��{statDic[stat]}��{boost}�i�K�オ����");
            }
            else
            {
                StatusChanges.Enqueue($"{Base.Name}��{statDic[stat]}��{-boost}�i�K��������");
            }
        }
    }

    //level�ɉ������X�e�[�^�X��Ԃ����́F�v���p�e�B�i�{�����������邱�Ƃ��ł���j
    public int Attack
    {
        get { return GetStat(Stat.Attack); }
    }
    public int Defense
    {
        get { return GetStat(Stat.Defense); }
    }
    public int Intelligence
    {
        get { return GetStat(Stat.Intelligence); }
    }
    public int Mind
    {
        get { return GetStat(Stat.Mind); }
    }

    public int MaxHP { get; private set; }
    public int MaxMP { get; private set; }

    public DamageDetailes TakeDamage(Move move, Battler attacker, Battler target)
    {
        float attack = attacker.Attack;
        float defense = Defense;

        if (move.Base.Category1 == MoveCategory1.Skill)
        {
            attack = attacker.Intelligence;
            defense = Mind;
        }
        //�N���e�B�J��
        float critical = 1f;
        //6.25%�ŃN���e�B�J��
        if (UnityEngine.Random.value * 100 <= 6.25f)
        {
            critical = 2f;
        }
        DamageDetailes damageDetailes = new DamageDetailes
        {
            Fainted = false,
            Critical = critical,
        };

        int damage = 0;
        int modfil = UnityEngine.Random.Range(0, 3);
        int mod = UnityEngine.Random.Range(0, 2);

        //�U���́�2�|����́�4���_���[�W��b�l
        //�_���[�W��b�l ��16�{1�i�[���؎́j
        float a = (((attack + (float)attacker.Base.GetEquipWeapon().Base.GetAmount()) / 2) - ((defense + (float)target.Base.GetEquipArmor().Base.GetAmount()) / 4)) * ((float)move.Base.Power / 100) * critical;
        float b = a / 16 + 1;
        if (move.Base.Category1 == MoveCategory1.Skill)
        {
            a = (((attack + (float)attacker.Base.GetEquipWeapon().Base.GetMagicAmount()) / 2) - ((defense + (float)target.Base.GetEquipArmor().Base.GetMagicAmount()) / 4)) * ((float)move.Base.Power / 100) * critical;
            b = a / 16 + 1;
        }

        if (modfil == 0)
        {
            damage = Mathf.FloorToInt(a - b);
        }
        else if (modfil == 1)
        {
            damage = Mathf.FloorToInt(a);
        }
        else if (modfil == 2)
        {
            damage = Mathf.FloorToInt(a + b);
        }
        if (damage <= 0)
        {
            if (mod == 0)
            {
                resultText = $"{attacker.Base.Name}�͍U�����O�����I";

            }
            else if (mod == 1)
            {
                damage = 1;

                HP = Mathf.Clamp(HP - damage, 0, MaxHP);
                resultText = move.Base.RunMoveResult(attacker, target, damage);
            }
        }
        else
        {
            HP = Mathf.Clamp(HP - damage, 0, MaxHP);
            resultText = move.Base.RunMoveResult(attacker, target, damage);
        }
        return damageDetailes;
    }

    public DamageDetailes NormalAttackTakeDamage(Battler attacker, Battler target)
    {
        float attack = attacker.Attack;
        float defense = Defense;

        //�N���e�B�J��
        float critical = 1f;
        //6.25%�ŃN���e�B�J��
        if (UnityEngine.Random.value * 100 <= 6.25f)
        {
            critical = 2f;
        }
        DamageDetailes damageDetailes = new DamageDetailes
        {
            Fainted = false,
            Critical = critical,
        };

        int damage = 0;
        int modfil = UnityEngine.Random.Range(0, 2);
        int mod = UnityEngine.Random.Range(0, 1);

        //�U���́�2�|����́�4���_���[�W��b�l
        //�_���[�W��b�l ��16�{1�i�[���؎́j
        float a = (((attack + (float)attacker.Base.GetEquipWeapon().Base.GetAmount()) / 2) - ((defense + (float)target.Base.GetEquipArmor().Base.GetAmount()) / 4))  * critical;
        float b = a / 16 + 1;
        if (modfil == 0)
        {
            damage = Mathf.FloorToInt(a - b);
        }
        else if (modfil == 1)
        {
            damage = Mathf.FloorToInt(a);
        }
        else if (modfil == 2)
        {
            damage = Mathf.FloorToInt(a + b);
        }
        if (damage <= 0)
        {
            if (mod == 0)
            {
                resultText = $"{attacker.Base.Name}�͍U�����O�����I";

            }
            else if (mod == 1)
            {
                damage = 1;

                HP = Mathf.Clamp(HP - damage, 0, MaxHP);
                resultText = $"{attacker.Base.Name}�̒ʏ�U��\n{target.Base.Name}��{damage}�̃_���[�W";
            }
        }
        else
        {
            HP = Mathf.Clamp(HP - damage, 0, MaxHP);
            resultText = $"{attacker.Base.Name}�̒ʏ�U��\n{target.Base.Name}��{damage}�̃_���[�W";
        }
        return damageDetailes;
    }

    public void Heal(int healPoint)
    {
        HP = Mathf.Clamp(HP + healPoint, 0, MaxHP);
    }

    public Move GetRandomMove()
    {
        int r = UnityEngine.Random.Range(0, Moves.Count);
        return Moves[r];
        //�������G���[�ɂȂ������̓G���Z���o���ĂȂ��i���x���������������ŖY��Ă�j
    }

    public bool IsLevelUp(BattleUnit playerUnit)
    {
        if (HasExp >= (400*Level* Level / (204- Level))+5)
        {
            int fMaxHp = MaxHP;
            int fMaxMp = MaxMP;
            HasExp -= (400 * Level * Level / (204 - Level)) + 5;
            level++;
            CalculateStats();
            int bMaxHp = MaxHP;
            int bMaxMp = MaxMP;
            HP += bMaxHp - fMaxHp;
            MP += bMaxMp - fMaxMp;
            playerUnit.UpdateUI();
            return true;
        }

        return false;
    }

    //�V�����Z���o����̂��ǂ���
    public Move LearnedMove()
    {
        foreach (var learnableMove in Base.LearnableMoves)
        {

            //�܂��o���Ă��Ȃ����̂Ŋo����Z������Γo�^����
            if (learnableMove.Level <= level && !Moves.Exists(move => move.Base == learnableMove.MoveBase))
            {
                Move move = new Move(learnableMove.MoveBase);
                Moves.Add(move);
                return (move);
            }
        }

        return null;
    }

    //��Ԉُ���󂯂��Ƃ��ɌĂяo���֐�
    public void SetStatus(ConditionID conditionID)
    {
        if (Status != null)
        {
            //��Ԉُ�������Ă�Ȃ�d�ˊ|���͂��Ȃ��H
            return;
        }
        Status = ConditionDB.Conditions[conditionID];//�ǂ̏�Ԉُ�ɂȂ�̂��m��
        Status?.OnStart?.Invoke(this);
        //���O�ɒǉ�
        StatusChanges.Enqueue($"{Base.Name}{Status.StartMessage}");

        //�X�e�[�^�X�ω����N���Ă�
        OnStatusChanged?.Invoke();

    }

    //��Ԉُ���󂯂��Ƃ��ɌĂяo���֐�
    public void SetVolatileStatus(ConditionID conditionID)
    {
        if (Status != null)
        {
            //��Ԉُ�������Ă�Ȃ�d�ˊ|���͂��Ȃ��H
            return;
        }
        VolatileStatus = ConditionDB.Conditions[conditionID];//�ǂ̏�Ԉُ�ɂȂ�̂��m��
        VolatileStatus?.OnStart?.Invoke(this);
        //���O�ɒǉ�
        StatusChanges.Enqueue($"{Base.Name}{VolatileStatus.StartMessage}");
        //�X�e�[�^�X�ω����N���Ă�
        //OnStatusChanged?.Invoke();//UI�̕ύX�͂���Ȃ�

    }
    //��Ԉُ킩���
    public void CureStatus()
    {
        Status = null;//���ׂĂ̏�Ԉُ킪���Z�b�g�����
        OnStatusChanged?.Invoke();
    }

    //�^�[���I�����ɂ�肽�����ƁF��Ԉُ�
    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }

    //��Ԉُ킩���
    public void CureVolatileStatus()
    {
        VolatileStatus = null;//���ׂĂ̏�Ԉُ킪���Z�b�g�����
        //OnStatusChanged?.Invoke();//UI�̕ύX�͂���Ȃ�
    }

    //�^�[���J�n���ɂ�肽�����ƁF��Ԉُ�
    public bool OnBeforeMove()
    {
        bool canRunMove = true;
        if (Status?.OnBeforeMove != null)
        {
            if (Status.OnBeforeMove(this) == false)
            {
                canRunMove = false;
            }
        }
        //�����n�̏�Ԉُ�ɂ���
        if (VolatileStatus?.OnBeforeMove != null)
        {
            if (VolatileStatus.OnBeforeMove(this) == false)
            {
                canRunMove = false;
            }

        }
        return canRunMove;
    }

    //�Q�[���I�����Ɏ��s����������
    public void OnBattleOver()
    {
        ResetStatBoost();
        VolatileStatus = null;
    }

}

public class DamageDetailes
{
    public bool Fainted { get; set; }//�퓬�s�\���ǂ���
    public float Critical { get; set; }//�}���ɓ����������ǂ���
}

