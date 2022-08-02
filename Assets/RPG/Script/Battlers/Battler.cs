using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class Battler
{
    //データの初期化がされてない＆_baseとlevelが使われていない
    //インスペクターからデータを設定できるようにする
    [SerializeField] BattlerBase _base;
    [SerializeField] int level;
    //ベースとなるデータ
    public BattlerBase Base { get => _base; }
    public int Level { get => level; }
    public int HasExp { get; set; }
    public int Gold { get; set; }
    public int HP { get; set; }
    public int MP { get; set; }
    public int Value { get; set; }


    //覚える技
    public List<Move> Moves { get; set; }
    //現在の技
    public Move CurrentMove { get; set; }
    
    //初期ステータスと追加ステータス
    public Dictionary<Stat, int> Stats { get; set; }
    public Dictionary<Stat, int> StatBoosts { get; set; }

    //ログをためておく変数を作る：出し入れが簡単なリスト
    public Queue<string> StatusChanges { get; private set; }
    //リザルトテキストを送る変数
    private string resultText;

    public string ResultText { get => resultText; }
    //状態異常
    public Condition Status { get; private set; }
    public int StatusTime { get; set; }

    //ステータス変化が起こったときに実行したいことを登録しておく関数を作る
    public Action OnStatusChanged;

    //バトル終了時に回復する：Statusと同じように設定する
    public Condition VolatileStatus { get; private set; }
    public int VolatileStatusTime { get; set; }



    readonly Dictionary<Stat, string> statDic = new Dictionary<Stat, string>()
    {
        {Stat.Attack, "攻撃力"},
        {Stat.Defense, "防御力"},
        {Stat.Intelligence ,"知力"},
        {Stat.Mind ,"精神力"},

    };



    //初期化
    public void Init()
    {
        StatusChanges = new Queue<string>();
        //覚える技から使える技を生成
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
            //ステータス変化してない初期値を入れていく
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
        //boostの分を計算する
        int boost = StatBoosts[stat];
        float[] boostValues = new float[] { 1, 2, 4, 8, 16 };

        if (boost >= 0)
        {
            //強化なら
            statValue = Mathf.FloorToInt(statValue * boostValues[boost]);
        }
        else
        {
            //弱体化なら
            statValue = Mathf.FloorToInt(statValue / boostValues[-boost]);
        }


        return statValue;
    }

    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        //TODO　ステータス変化を反映:StatBoostsを変更する
        foreach (StatBoost statBoost in statBoosts)
        {
            //どのステートを
            Stat stat = statBoost.stat;
            //何段階
            int boost = statBoost.boost;
            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);
            if (boost > 0)
            {
                StatusChanges.Enqueue($"{Base.Name}の{statDic[stat]}が{boost}段階上がった");
            }
            else
            {
                StatusChanges.Enqueue($"{Base.Name}の{statDic[stat]}が{-boost}段階下がった");
            }
        }
    }

    //levelに応じたステータスを返すもの：プロパティ（＋処理を加えることができる）
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
        //クリティカル
        float critical = 1f;
        //6.25%でクリティカル
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

        //攻撃力÷2－守備力÷4＝ダメージ基礎値
        //ダメージ基礎値 ÷16＋1（端数切捨）
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
                resultText = $"{attacker.Base.Name}は攻撃を外した！";

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

        //クリティカル
        float critical = 1f;
        //6.25%でクリティカル
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

        //攻撃力÷2－守備力÷4＝ダメージ基礎値
        //ダメージ基礎値 ÷16＋1（端数切捨）
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
                resultText = $"{attacker.Base.Name}は攻撃を外した！";

            }
            else if (mod == 1)
            {
                damage = 1;

                HP = Mathf.Clamp(HP - damage, 0, MaxHP);
                resultText = $"{attacker.Base.Name}の通常攻撃\n{target.Base.Name}は{damage}のダメージ";
            }
        }
        else
        {
            HP = Mathf.Clamp(HP - damage, 0, MaxHP);
            resultText = $"{attacker.Base.Name}の通常攻撃\n{target.Base.Name}は{damage}のダメージ";
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
        //ここがエラーになったら大体敵が技を覚えてない（レベルを下げたせいで忘れてる）
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

    //新しく技を覚えるのかどうか
    public Move LearnedMove()
    {
        foreach (var learnableMove in Base.LearnableMoves)
        {

            //まだ覚えていないもので覚える技があれば登録する
            if (learnableMove.Level <= level && !Moves.Exists(move => move.Base == learnableMove.MoveBase))
            {
                Move move = new Move(learnableMove.MoveBase);
                Moves.Add(move);
                return (move);
            }
        }

        return null;
    }

    //状態異常を受けたときに呼び出す関数
    public void SetStatus(ConditionID conditionID)
    {
        if (Status != null)
        {
            //状態異常を持ってるなら重ね掛けはしない？
            return;
        }
        Status = ConditionDB.Conditions[conditionID];//どの状態異常になるのか確定
        Status?.OnStart?.Invoke(this);
        //ログに追加
        StatusChanges.Enqueue($"{Base.Name}{Status.StartMessage}");

        //ステータス変化が起きてる
        OnStatusChanged?.Invoke();

    }

    //状態異常を受けたときに呼び出す関数
    public void SetVolatileStatus(ConditionID conditionID)
    {
        if (Status != null)
        {
            //状態異常を持ってるなら重ね掛けはしない？
            return;
        }
        VolatileStatus = ConditionDB.Conditions[conditionID];//どの状態異常になるのか確定
        VolatileStatus?.OnStart?.Invoke(this);
        //ログに追加
        StatusChanges.Enqueue($"{Base.Name}{VolatileStatus.StartMessage}");
        //ステータス変化が起きてる
        //OnStatusChanged?.Invoke();//UIの変更はいらない

    }
    //状態異常から回復
    public void CureStatus()
    {
        Status = null;//すべての状態異常がリセットされる
        OnStatusChanged?.Invoke();
    }

    //ターン終了時にやりたいこと：状態異常
    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }

    //状態異常から回復
    public void CureVolatileStatus()
    {
        VolatileStatus = null;//すべての状態異常がリセットされる
        //OnStatusChanged?.Invoke();//UIの変更はいらない
    }

    //ターン開始時にやりたいこと：状態異常
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
        //混乱系の状態異常について
        if (VolatileStatus?.OnBeforeMove != null)
        {
            if (VolatileStatus.OnBeforeMove(this) == false)
            {
                canRunMove = false;
            }

        }
        return canRunMove;
    }

    //ゲーム終了時に実行したいこと
    public void OnBattleOver()
    {
        ResetStatBoost();
        VolatileStatus = null;
    }

}

public class DamageDetailes
{
    public bool Fainted { get; set; }//戦闘不能かどうか
    public float Critical { get; set; }//急所に当たったかどうか
}

