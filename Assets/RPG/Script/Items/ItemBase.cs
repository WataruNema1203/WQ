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
    [SerializeField]
    private string information = "";
    //�@�A�C�e���̃p�����[�^
    [SerializeField]
    private int amount = 0;

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
    public virtual void Use(Battler battler)
    {

    }
}

public enum Type
    {
        HPRecovery,
        MPRecovery,
        PoisonRecovery,
        NumbnessRecovery,
        Weapon,
        Armor,
        Valuables
    }