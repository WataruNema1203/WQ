using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    [SerializeField] GameObject itemPrefabs;
    [SerializeField] PlayerController player;
    [SerializeField] GameObject descriptionPanel;
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] Text descriptionText;
    public UnityAction OnUsedItem;
    ItemUI[] itemSlots;
    ItemBase @base;
    
    

    int selectedIndex;

    public int SelectedIndex { get => selectedIndex; }

    public void Init()
    {
        selectedIndex = 0;
       itemSlots = GetComponentsInChildren<ItemUI>();
        Show();
    }

    void Show()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (inventory.Items.Count <= i || inventory.Items[i] == null || inventory.Items[i].item.Base == null)
            {
                itemSlots[i].SetText("-");
                itemSlots[i].transform.Find("PosessionText").GetComponent<Text>().text = "";
            }
            else
            {
                    itemSlots[i].SetText($"{inventory.Items[i].item.Base.GetKanjiName()}");
                    itemSlots[i].transform.Find("PosessionText").GetComponent<Text>().text = "× " + $"{inventory.Items[i].posession}";
            }
        }
    }
    //void Show()
    //{
    //    foreach (var item in player.GetItemDictionary().Keys)
    //    {
    //        itemPrefabs = Instantiate(itemPrefabs, inventoryPanel.transform);
    //        itemPrefabs.GetComponent<Text>().text = item.GetKanjiName();
    //        itemPrefabs.transform.Find("PosessionText").GetComponent<Text>().text = "× " + $"{player.GetItemNum(item).ToString()}";
    //         Debug.Log("test");

    //    }
    //}

    public void HandleUpdateItemSelection()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex-=2;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex+=2;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedIndex--;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectedIndex++;
        }

        selectedIndex = Mathf.Clamp(selectedIndex, 0, itemSlots.Length-1);

        for (int i = 0; i < itemSlots.Length; i++)
        {
            bool selected = selectedIndex == i;
            if (selected)
            {
                if (CanSelectedItem())
                {
                    descriptionPanel.SetActive(true);
                    descriptionText.text = inventory.Items[i].item.Base.GetInformation();
                }
                else
                {
                    descriptionPanel.SetActive(false);
                }
            }
            itemSlots[i].SetSelectedColor(selected);
        }
    }

    public bool CanSelectedItem()
    {
        if (inventory.Items.Count <= selectedIndex || itemSlots[selectedIndex] == null || inventory.Items[selectedIndex].item.Base == null)
        {
            return false;
        }

        return true;
    }

    public IEnumerator Use()
    {
        if (inventory.Items[selectedIndex] == null || inventory.Items[selectedIndex].item.Base == null)
        {
            OnUsedItem?.Invoke();
        }
        else
        {
            Item useItem = inventory.Items[selectedIndex].item;
            inventory.Items[selectedIndex].posession--;
            Show();
            yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetKanjiName()}を使った"));
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
            inventory.Use(useItem);
            if (inventory.Items[selectedIndex].item.Base.GetItemType() == Type.PoisonRecovery || inventory.Items[selectedIndex].item.Base.GetItemType() == Type.NumbnessRecovery)
            {

            }
            else
            {
                yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetAmount()}ポイント回復した"));
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
            }
            if (inventory.Items[selectedIndex].posession <= 0)
            {
                inventory.Items[selectedIndex] = null;
            }
            DialogManager.Instance.Close();
            OnUsedItem?.Invoke();
        }
    }

    public IEnumerator UseInBattle()
    {
        if (inventory.Items[selectedIndex] == null || inventory.Items[selectedIndex].item.Base == null)
        {
        }
        else
        {
            Item useItem = inventory.Items[selectedIndex].item;
            inventory.Items[selectedIndex].posession --;
            Show();
            yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetKanjiName()}を使った"));
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
            inventory.Use(useItem);
            yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetAmount()}ポイント回復した"));
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
            if (inventory.Items[selectedIndex].posession <= 0)
            {
                inventory.Items[selectedIndex] = null;
            }
        }
    }



    public void Open()
    {
        inventoryPanel.SetActive(true);
        descriptionPanel.SetActive(true);
        Init();
    }

    public void Close()
    {
        inventoryPanel.SetActive(false);
        descriptionPanel.SetActive(false);
    }
}