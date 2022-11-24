using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionDB
{
    //キー、Value
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.LowContinuation,
            new Condition()
            {
                Id=ConditionID.LowContinuation,
                Name="吐き気",
                StartMessage="は吐きそうになった",
                OnAfterTurn=(Battler target) =>
                {
                    //弱継続ダメージを与える
                    target.HP-=target.MaxHP/16;
                    //弱継続ダメージのログを出す
                    target.StatusChanges.Enqueue($"{target.Base.Name}は吐きそうになって苦しんでいる");
                }
            }
        },
        {
            ConditionID.HighContinuation,
            new Condition()
            {
                Id=ConditionID.HighContinuation,
                Name="嘔吐",
                StartMessage="は吐き始めた",
                OnAfterTurn=(Battler target) =>
                {
                    //やけどダメージを与える
                    target.HP-=target.MaxHP/8;
                    //やけどダメージのログを出す
                    target.StatusChanges.Enqueue($"{target.Base.Name}は吐きまくって苦しんでいる");
                }
            }
        },
        {
            ConditionID.Barn,
            new Condition()
            {
                Id=ConditionID.HighContinuation,
                Name="ちどりあし",
                StartMessage="はフラフラで歩き始めた",
                OnAfterTurn=(Battler target) =>
                {
                    //やけどダメージを与える
                    target.HP-=target.MaxHP/16;
                    //やけどダメージのログを出す
                    target.StatusChanges.Enqueue($"{target.Base.Name}はフラフラすぎて転びまくっている！");
                }
            }
        },
        {
            ConditionID.Paralisis,
            new Condition()
            {
                Id=ConditionID.Paralisis,
                Name="アル中ｶﾗｶﾗ",
                StartMessage="はアル中でからだがふるえ始めた",
                OnBeforeMove=(Battler target) =>
                {
                    //true:技が出せる　false:まひで動けない

                    //・一定確率で
                    //・技が出せずに自分のターンが終わる
                    
                    //1,2,3,4,の中で１が出たら（25％）
                    if (Random.Range(1,5)==1)
                    {
                        //技が出せない
                        target.StatusChanges.Enqueue($"{target.Base.name}はからだがふるえてて動けない！");
                        return false;
                    }
                    return true;
                }
            }
        },
        {
            ConditionID.Freeze,
            new Condition()
            {
                Id=ConditionID.Freeze,
                Name="潰れてる",
                StartMessage="は潰れて眠り始めた",

                OnBeforeMove=(Battler target) =>
                {
                    //true:凍ったまま　false:こおり状態から回復して動けるようになる

                    //・一定確率で
                    //・技が出せずに自分のターンが終わる
                    
                    //0,1,2,3,4の中で0が出たら（20％）
                    if (Random.Range(0,5)==1)
                    {
                        target.CureStatus();
                        target.StatusChanges.Enqueue($"{target.Base.name}は意識を取り戻した！");
                        return true;
                    }
                    target.StatusChanges.Enqueue($"{target.Base.name}は潰れて動けない！");
                    return false;
                }
            }
        }, {
            ConditionID.Binding,
            new Condition()
            {
                Id=ConditionID.Binding,
                Name="グイグイ",
                StartMessage="はグイグイされはじめた",
                OnStart=(Battler target) =>
                {
                    //技を受けた時に、何ターン眠るか決める
                    target.StatusTime=Random.Range(1,5);//1,2,3ターンのどれか

                },

                OnBeforeMove=(Battler target) =>
                {
                    //true:ねむったまま　false:ねむり状態から回復してターンを終了する

                    //・モンスターが技を使うとき
                    //・ねむりのターンカウントを減らす
                    //・ねむりのターンカウントが０になったら行動可能
                    //・０じゃなかったら行動不可能
                    if (target.StatusTime <= 0)
                    {
                        target.CureStatus();
                        target.StatusChanges.Enqueue($"{target.Base.name}はすきをみて周りから逃げた！");
                        return true;
                    }
                    else
                    {
                    target.StatusChanges.Enqueue($"{target.Base.name}はグイグイされてる！");
                    target.StatusTime--;
                    return false;
                    }
                }
            }
        }, {
            ConditionID.Confusion,
            new Condition()
            {
                Id=ConditionID.Confusion,
                Name="酔っ払い",
                StartMessage="は酔っぱらった",
                 OnStart=(Battler target) =>
                {
                    //技を受けた時に、何ターン混乱するか決める
                    target.VolatileStatusTime=Random.Range(1,5);//1,2,3ターンのどれか

                },

                OnBeforeMove=(Battler target) =>
                {
                    //true:こんらんしたままで自傷ダメージを受ける　false:こんらん状態から回復して動けるようになる

                    //・一定確率で
                    //・技が出せずに自分にダメージ
                    
                    //0,1,2,3,4の中で0が出たら（20％）
                     if (target.VolatileStatusTime <= 0)
                    {
                        target.CureVolatileStatus();
                        target.StatusChanges.Enqueue($"{target.Base.name}は酔いがさめた！");
                        return true;
                    }

                    target.VolatileStatusTime--;

                    if (Random.Range(1,3)==1)
                    {
                        return true;
                    }

                    target.StatusChanges.Enqueue($"{target.Base.name}は酔っぱらっている！");
                    target.HP=target.MaxHP/8;
                    target.StatusChanges.Enqueue($"{target.Base.name}は酔い覚ましで自分の顔を叩きはじめた！");

                    return false;

                }
            }
        },

    };
}

public enum ConditionID
{
    None,             //なし
    LowContinuation,  //弱継続ダメ(毒)
    HighContinuation, //強継続ダメ（猛毒）
    Barn,             //弱継続ダメージ＋攻撃力低下（火傷）
    Binding,          //束縛＋確率で回復（回復後ターン終了）(ねむり)
    Freeze,           //束縛＋確率で回復（回復後攻撃可能）(こおり)
    Paralisis,        //束縛＋たまに行動(麻痺)
    Confusion,        //確率で自身にダメージor相手に攻撃
}