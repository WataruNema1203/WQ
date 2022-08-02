using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[Serializable]
public class ItemBase : ScriptableObject
{

    //�@�A�C�e���̎��
    [SerializeField]
    public Type itemType = Type.HPRecovery;
    //�@�A�C�e���̊�����
    [SerializeField]
    private string kanjiName = "";
    //�@�A�C�e���̕�������
    [SerializeField]
    private string hiraganaName = "";
    //�@�A�C�e�����
    [TextArea]
    [SerializeField]
    private string information = "";
    //�@�A�C�e���̃p�����[�^
    [SerializeField]
    private int amount = 0;
    [SerializeField]
    private int magicAmount = 0;
    //�@�A�C�e���̃S�[���h
    [SerializeField]
    private int gold = 0;

    //�@�A�C�e���̎�ނ�Ԃ�
    public Type GetItemType()
    {
        return itemType;
    }
    //�@�A�C�e���̖��O��Ԃ�
    public string GetKanjiName()
    {
        return kanjiName;
    }
    //�@�A�C�e���̕������̖��O��Ԃ�
    public string GetHiraganaName()
    {
        return hiraganaName;
    }
    //�@�A�C�e������Ԃ�
    public string GetInformation()
    {
        return information;
    }
    //�@�A�C�e���̋�����Ԃ�
    public int GetAmount()
    {
        return amount;
    }
    public int GetMagicAmount()
    {
        return magicAmount;
    }
    //�@�A�C�e���̋�����Ԃ�
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

public enum Type
{
    HPRecovery,
    HPFullRecovery,
    MPRecovery,
    MPFullRecovery,
    LowContinuation,  //��p���_��(��)
    HighContinuation, //���p���_���i�ғŁj
    Barn,             //��p���_���[�W�{�U���͒ቺ�i�Ώ��j
    Binding,          //�����{�m���ŉ񕜁i�񕜌�^�[���I���j(�˂ނ�)
    Freeze,           //�����{�m���ŉ񕜁i�񕜌�U���\�j(������)
    Paralisis,        //�����{���܂ɍs��(���)
    Weapon,
    Armor,
    Accessory,
    Valuables         //�M�d�i
}