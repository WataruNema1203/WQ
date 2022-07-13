using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] List<Items> items = new List<Items>();
    PlayerController player;

    public List<Items> Items { get => items; set => items = value; }

    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    public void Use(Item item)
    {
        if (item.Base is HPItemBase)
        {
            ((HPItemBase)item.Base).Use(player.Battler);
        }
        if (item.Base is MPItemBase)
        {
            ((MPItemBase)item.Base).Use(player.Battler);
        }

    }

    public bool TryAdd(Item item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null || items[i].item.Base == null)
            {
                items[i].item = item;
                return true;
            }
        }

        return false;
    }
}

[System.Serializable]
public class Items
{
    public Item item;
    public int posession;
}