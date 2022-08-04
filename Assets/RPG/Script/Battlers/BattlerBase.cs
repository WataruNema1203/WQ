using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BattlerBase : ScriptableObject
{
    //Battlerの基礎データ
    [SerializeField] new string name;
    [SerializeField] float maxHP;
    [SerializeField] float maxMP;
    [SerializeField] float at;
    [SerializeField] float inte;
    [SerializeField] float def;
    [SerializeField] float mnd;
    [SerializeField] Sprite sprite;
    [SerializeField] List<LearnableMove> learnableMoves;
    [SerializeField] int exp;
    [SerializeField] int gold;
    [SerializeField] DropItem1 dropItem1;
    [SerializeField] DropItem2 dropItem2;


    //装備している武器
    [SerializeField]
    private Item equipWeapon;
    //　装備している鎧
    [SerializeField]
    private Item equipArmor;
    [SerializeField]
    private Item equipAccessory;

    public void SetNeme(string name)
    {
        this.name=name;
    }

    public void SetEquipWeapon(Item weaponItem)
    {
        this.equipWeapon = weaponItem;
    }

    public Item GetEquipWeapon()
    {
        return equipWeapon;
    }

    public void SetEquipArmor(Item armorItem)
    {
        this.equipArmor = armorItem;
    }

    public Item GetEquipArmor()
    {
        return equipArmor;
    }
    public void SetEquipAccessory(Item accessory)
    {
        this.equipAccessory = accessory;
    }

    public Item GetEquipAccessory()
    {
        return equipAccessory;
    }




    public string Name { get => name; }
    public float MaxHP { get => maxHP; }
    public float At { get => at; }
    public Sprite Sprite { get => sprite; }
    public List<LearnableMove> LearnableMoves { get => learnableMoves; }
    public int Exp { get => exp; }
    public float Def { get => def; }
    public float MaxMP { get => maxMP; }
    public float Inte { get => inte; }
    public float Mid { get => mnd; }
    public DropItem1 DropItem1 { get => dropItem1; }
    public DropItem2 DropItem2 { get => dropItem2; }
    public int Gold { get => gold; }

}

public enum Stat
{
    Attack,
    Defense,
    Intelligence,
    Mind,
}

[System.Serializable]
public class DropItem1
{
    [SerializeField] Item dropItem;
    [SerializeField] int chance;

    public Item DropItem { get => dropItem;}
    public int Chance { get => chance; }
}
[System.Serializable]
public class DropItem2
{
    [SerializeField] Item dropItem;
    [SerializeField] int chance;

    public Item DropItem { get => dropItem;}
    public int Chance { get => chance; }
}
