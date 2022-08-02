using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventory : MonoBehaviour
{
    [SerializeField]PlayerController player;
    [SerializeField] Item noneItem;
    [SerializeField] List<ItemChara> itemCharas=new List<ItemChara>();

    public Item NoneItem { get => noneItem; set => noneItem = value; }
    public List<ItemChara> ItemCharas { get => itemCharas; set => itemCharas = value; }

    public void Use(Item item,int selectIndex)
    {
        if (item.Base.GetItemType() == Type.HPRecovery)
        {
            item.Base.RecoveryHP(player.Battlers[selectIndex]);
        }
        else if (item.Base.GetItemType() == Type.MPRecovery)
        {
            item.Base.RecoveryMP(player.Battlers[selectIndex]);
        }
        else if (item.Base.GetItemType() == Type.HPFullRecovery)
        {
            item.Base.RecoveryFullHP(player.Battlers[selectIndex]);
        }
        else if (item.Base.GetItemType() == Type.MPFullRecovery)
        {
            item.Base.RecoveryFullMP(player.Battlers[selectIndex]);
        }
        else
        {
            item.Base.RecoveryStatus(player.Battlers[selectIndex]);
        }

    }
}

[System.Serializable]
public class ItemChara
{
    [SerializeField] List<ItemType> itemTypes = new List<ItemType>();

    public List<ItemType> ItemTypes { get => itemTypes; set => itemTypes = value; }
}

[System.Serializable]
public class ItemType
{
    [SerializeField] List<Items> items = new List<Items>();

    public List<Items> Items { get => items; set => items = value; }
}

[System.Serializable]
public class Items
{
    public Item item;
    public int possession;
}