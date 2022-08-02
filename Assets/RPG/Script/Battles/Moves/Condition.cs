using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Condition
{
    public ConditionID Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    //状態異常のメッセージ
    public string StartMessage { get; set; }

    //ターンの一番最初にやりたいこと
    public Action<Battler> OnStart;
    //ターン終了時に実行したいこと
    public Func<Battler, bool> OnBeforeMove;
    public Action<Battler> OnAfterTurn;
}

