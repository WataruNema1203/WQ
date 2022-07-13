using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu]
public class PlayerBattler : BattlerBase
{
    Battler battler;

    [SerializeField]
    private ItemBase equipWeapon = null;
    //　装備している鎧
    [SerializeField]
    private ItemBase equipArmor = null;
    //　アイテムと個数のDictionary
    [SerializeField]
    private ItemDictionary itemDictionary = null;

    public Battler Battler { get => battler; set => battler = value; }

    public void SetEquipWeapon(ItemBase weaponItem)
    {
        this.equipWeapon = weaponItem;
    }

    public ItemBase GetEquipWeapon()
    {
        return equipWeapon;
    }

    public void SetEquipArmor(ItemBase armorItem)
    {
        this.equipArmor = armorItem;
    }

    public ItemBase GetEquipArmor()
    {
        return equipArmor;
    }

    public void CreateItemDictionary(ItemDictionary itemDictionary)
    {
        this.itemDictionary = itemDictionary;
    }

    public void SetItemDictionary(ItemBase item, int num = 0)
    {
        itemDictionary.Add(item, num);
    }

    //　アイテムが登録された順番のItemDictionaryを返す
    public ItemDictionary GetItemDictionary()
    {
        return itemDictionary;
    }
    //　平仮名の名前でソートしたItemDictionaryを返す
    public IOrderedEnumerable<KeyValuePair<ItemBase, int>> GetSortItemDictionary()
    {
        return itemDictionary.OrderBy(ItemBase => ItemBase.Key.GetHiraganaName());
    }
    public int SetItemNum(ItemBase tempItem, int num)
    {
        return itemDictionary[tempItem] = num;
    }
    //　アイテムの数を返す
    public int GetItemNum(ItemBase item)
    {
        return itemDictionary[item];
    }
}
