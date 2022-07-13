using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BattlerBase :ScriptableObject
{
    //Battler‚ÌŠî‘bƒf[ƒ^
    [SerializeField] new string name;
    [SerializeField] int maxHP;
    [SerializeField] int maxMP;
    [SerializeField] int at;
    [SerializeField] int inte;
    [SerializeField] int def;
    [SerializeField] int mid;
    [SerializeField] Sprite sprite;
    [SerializeField] List<LearnableMove> learnableMoves;
    [SerializeField] int exp;
    [SerializeField] DropItem1 dropItem1;
    [SerializeField] DropItem2 dropItem2;



    public string Name { get => name; }
    public int MaxHP { get => maxHP;}
    public int At { get => at; }
    public Sprite Sprite { get => sprite;}
    public List<LearnableMove> LearnableMoves { get => learnableMoves;}
    public int Exp { get => exp;}
    public int Def { get => def; }
    public int MaxMP { get => maxMP;}
    public int Inte { get => inte;}
    public int Mid { get => mid;}
    public DropItem1 DropItem1 { get => dropItem1; }
    public DropItem2 DropItem2 { get => dropItem2; }
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
    [SerializeField] ItemBase dropItem;
    [SerializeField] int chance;

    public ItemBase DropItem { get => dropItem;}
    public int Chance { get => chance; }
}
[System.Serializable]
public class DropItem2
{
    [SerializeField] ItemBase dropItem;
    [SerializeField] int chance;

    public ItemBase DropItem { get => dropItem;}
    public int Chance { get => chance; }
}
