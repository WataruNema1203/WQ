//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.Linq;
//using UnityEngine.UI;

//public class ItemSystem: MonoBehaviour
//{
//    Item item;
//    //�@�������Ă��镐��
//    [SerializeField]
//    private Item equipWeapon = null;
//    //�@�������Ă���Z
//    [SerializeField]
//    private Item equipArmor = null;
//    //�@�A�C�e���ƌ���Dictionary
//    [SerializeField]
//    private ItemDictionary itemDictionary = null;

//    public Item Item { get => item; }

//    public void SetEquipWeapon(Item weaponItem)
//    {
//        this.equipWeapon = weaponItem;
//    }

//    public Item GetEquipWeapon()
//    {
//        return equipWeapon;
//    }

//    public void SetEquipArmor(Item armorItem)
//    {
//        this.equipArmor = armorItem;
//    }

//    public Item GetEquipArmor()
//    {
//        return equipArmor;
//    }

//    public void CreateItemDictionary(ItemDictionary itemDictionary)
//    {
//        this.itemDictionary = itemDictionary;
//    }

//    public void SetItemDictionary(Item item, int num = 0)
//    {
//        itemDictionary.Add(item, num);
//    }

//    //�@�A�C�e�����o�^���ꂽ���Ԃ�ItemDictionary��Ԃ�
//    public ItemDictionary GetItemDictionary()
//    {
//        return itemDictionary;
//    }
//    //�@�������̖��O�Ń\�[�g����ItemDictionary��Ԃ�
//    public IOrderedEnumerable<KeyValuePair<Item, int>> GetSortItemDictionary()
//    {
//        return itemDictionary.OrderBy(item => item.Key.GetHiraganaName());
//    }

//    public int SetItemNum(Item tempItem, int num)
//    {
//        return itemDictionary[tempItem] = num;
//    }
//    //�@�A�C�e���̐���Ԃ�
//    public int GetItemNum(Item item)
//    {
//        return itemDictionary[item];
//    }

//}