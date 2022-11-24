using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[Serializable]
public class ItemBase : ScriptableObject
{

    //　アイテムの種類
    [SerializeField]
    public MoveType itemType = MoveType.HPRecovery;
    //　アイテムの漢字名
    [SerializeField]
    private string kanjiName = "";
    //　アイテムの平仮名名
    [SerializeField]
    private string hiraganaName = "";
    //　アイテム情報
    [TextArea]
    [SerializeField]
    private string information = "";
    //　アイテムのパラメータ
    [SerializeField]
    private int amount = 0;
    [SerializeField]
    private int magicAmount = 0;
    [SerializeField]
    private int speedAmount = 0;
    //　アイテムのゴールド
    [SerializeField]
    private int gold = 0;
    //貴重品等のダイアログ表示テキスト
    [SerializeField]
    private Dialog dialog;

    public Dialog Dialog { get => dialog;}

    //　アイテムの種類を返す
    public MoveType GetItemType()
    {
        return itemType;
    }
    //　アイテムの名前を返す
    public string GetKanjiName()
    {
        return kanjiName;
    }
    //　アイテムの平仮名の名前を返す
    public string GetHiraganaName()
    {
        return hiraganaName;
    }
    //　アイテム情報を返す
    public string GetInformation()
    {
        return information;
    }
    //　アイテムの強さを返す
    public int GetAmount()
    {
        return amount;
    }
    public int GetMagicAmount()
    {
        return magicAmount;
    }
    public int GetSpeedAmount()
    {
        return speedAmount;
    }
    //　アイテムの金額を返す
    public int GetGold()
    {
        return gold;
    }
    public void RecoveryHP(Battler battler)
    {
        battler.HP = Mathf.Clamp(battler.HP + amount, 0, battler.MaxHP);
    }
    public void RecoveryMP(Battler battler)
    {
        battler.MP = Mathf.Clamp(battler.MP + amount, 0, battler.MaxMP);
    }
    public void RecoveryFullHP(Battler battler)
    {
        battler.HP = battler.MaxHP;
    }
    public void RecoveryFullMP(Battler battler)
    {
        battler.MP = battler.MaxMP;
    }

    public void RecoveryStatus(Battler battler)
    {
        battler.CureStatus();
    }
}

public enum MoveType
{
    HPRecovery,
    HPFullRecovery,
    MPRecovery,
    MPFullRecovery,
    LowContinuation,  //弱継続ダメ(毒)
    HighContinuation, //強継続ダメ（猛毒）
    Barn,             //弱継続ダメージ＋攻撃力低下（火傷）
    Binding,          //束縛＋確率で回復（回復後ターン終了）(ねむり)
    Freeze,           //束縛＋確率で回復（回復後攻撃可能）(こおり)
    Paralisis,        //束縛＋たまに行動(麻痺)
    Weapon,
    Armor,
    Accessory,
    Valuables         //貴重品
}