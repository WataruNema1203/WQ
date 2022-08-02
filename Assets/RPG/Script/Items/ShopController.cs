using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

enum ShopState
{
    SelectAction,
    BuySelect,
    SellSelect,
    Buy,
    Sell,
}

public class ShopController : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] ItemInventory inventory;
    [SerializeField] GameObject ShopPanel;
    [SerializeField] GameObject shopStatusPanel;
    [SerializeField] GameObject selectActionPanel;
    [SerializeField] Text itemAmountText;
    [SerializeField] Text hasGoldText;
    [SerializeField] Text possessionText;
    [SerializeField] List<Item> items = new List<Item>();
    [SerializeField] GameObject shopValuePanel;
    [SerializeField] Text itemSumText;
    [SerializeField] Text itemSumGoldText;

     ShopState state;
    int selectedChara;
    int selectedItemType;
    int selectedItem;
    int sumItem=0;
    ItemUI[] itemSlots;
    ItemCharaUI[] itemCharas;
    ItemTypeUI[] itemTypes;
    SelectableText[] selectableTexts;


    public int SelectedChara { get => selectedChara;}
    public int SelectedItemType { get => selectedItemType;}
    public int SelectedItem { get => selectedItem; }




    internal ShopState State { get => state;}
    public Text HasGoldText { get => hasGoldText; set => hasGoldText = value; }
    public int SumItem { get => sumItem; set => sumItem = value; }

    public UnityAction<Item,int> OnBuyItem;
    public UnityAction<Item,int> OnSellItem;

    void BuyShow()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (items.Count <= i || items[i] == null || items[i].Base == null)
            {
                itemSlots[i].SetText("-");
                itemSlots[i].transform.Find("GoldText").GetComponent<Text>().text = "";
            }
            else
            {
                itemSlots[i].SetText($"{items[i].Base.GetKanjiName()}");
                itemSlots[i].transform.Find("GoldText").GetComponent<Text>().text =  $"{items[i].Base.GetGold()}"+"G";
            }
        }
    }

    void SellShow()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (inventory.ItemCharas[0].ItemTypes[0].Items.Count <= i || inventory.ItemCharas[0].ItemTypes[0].Items[i] == null || inventory.ItemCharas[0].ItemTypes[0].Items[i].item.Base == null)
            {
                itemSlots[i].SetText("-");
                itemSlots[i].transform.Find("GoldText").GetComponent<Text>().text = "";
            }
            else
            {
                itemSlots[i].SetText($"{inventory.ItemCharas[0].ItemTypes[0].Items[i].item.Base.GetKanjiName()}");
                itemSlots[i].transform.Find("GoldText").GetComponent<Text>().text = $"{inventory.ItemCharas[0].ItemTypes[0].Items[i].item.Base.GetGold()}" + "G";
            }
        }
    }

    public void HandleUpdate()
    {
        if (state == ShopState.SelectAction)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedItem++;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedItem--;
            }
            selectedItem = Mathf.Clamp(selectedItem, 0, selectableTexts.Length-1);

            for (int i = 0; i < selectableTexts.Length; i++)
            {
                if (selectedItem == i)
                {
                    selectableTexts[i].SetSelectedColor(true);
                }
                else
                {
                    selectableTexts[i].SetSelectedColor(false);
                }
            }
        }
        if (state == ShopState.BuySelect)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedItem -= 2;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedItem += 2;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                selectedItem--;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                selectedItem++;
            }

            selectedItem = Mathf.Clamp(selectedItem, 0, itemSlots.Length - 1);

            for (int i = 0; i < itemSlots.Length; i++)
            {
                bool selected = selectedItem == i;
                if (selected)
                {
                    if (CanSelectedBuyItem())
                    {
                        itemAmountText.text = items[i].Base.GetInformation();
                        hasGoldText.text = $"所持金：{player.Battlers[0].Gold}G";
                    }
                    else
                    {
                        hasGoldText.text = $"所持金：{player.Battlers[0].Gold}G";
                        itemAmountText.text = "";
                    }
                }
                itemSlots[i].SetSelectedColor(selected);
            }
        }
        if (state == ShopState.SellSelect)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedItem -= 2;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedItem += 2;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                selectedItem--;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                selectedItem++;
            }

            selectedItem = Mathf.Clamp(selectedItem, 0, itemSlots.Length - 1);

            for (int i = 0; i < itemSlots.Length; i++)
            {
                bool selected = selectedItem == i;
                if (selected)
                {
                    if (CanSelectedSellItem())
                    {
                        itemAmountText.text = inventory.ItemCharas[0].ItemTypes[0].Items[i].item.Base.GetInformation();
                        hasGoldText.text = $"所持金：{player.Battlers[0].Gold}G";
                        possessionText.text = $"個数：{inventory.ItemCharas[0].ItemTypes[0].Items[selectedItem].possession}";
                    }
                    else
                    {
                        hasGoldText.text = $"所持金：{player.Battlers[0].Gold}G";
                        itemAmountText.text = "";
                        possessionText.text = $"個数：None";
                    }
                }
                itemSlots[i].SetSelectedColor(selected);
            }
        }
        if (state == ShopState.Buy)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                sumItem++;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                sumItem--;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                sumItem -= 10;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                sumItem += 10;
            }

            if (sumItem<0)
            {
                sumItem = 0;
            }
            itemSumText.text = $"購入個数：{sumItem}";
            itemSumGoldText.text = $"合計：{items[selectedItem].Base.GetGold() * sumItem}";

        }
        if (state == ShopState.Sell)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                sumItem++;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                sumItem--;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                sumItem -= 10;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                sumItem += 10;
            }

            if (CanSelectedSellItem()==false)
            {
                itemSumGoldText.text = "合計：";
                itemSumText.text = "売却個数：";
                if (sumItem<0)
                {
                    sumItem = 0;
                }
            }
            else
            {
                sumItem = Mathf.Clamp(sumItem, 0, inventory.ItemCharas[0].ItemTypes[0].Items[selectedItem].possession);
                itemSumGoldText.text = $"合計：{inventory.ItemCharas[0].ItemTypes[0].Items[selectedItem].item.Base.GetGold() * sumItem}";
                itemSumText.text = $"売却個数：{sumItem}";
            }

        }
    }

    public bool CanSelectedBuyItem()
    {
        if (items.Count <= selectedItem || items[selectedItem] == null || items[selectedItem].Base == null)
        {
            return false;
        }

        return true;
    }
    public bool CanSelectedSellItem()
    {
        if (inventory.ItemCharas[0].ItemTypes[0].Items.Count <= selectedItem || inventory.ItemCharas[0].ItemTypes[0].Items[selectedItem] == null ||
            inventory.ItemCharas[0].ItemTypes[0].Items[selectedItem].item.Base == null|| inventory.ItemCharas[0].ItemTypes[0].Items[selectedItem].item.Base.GetKanjiName() == "未所持")
        {
            return false;
        }

        return true;
    }

    public void SelectActionInit()
    {
        state = ShopState.SelectAction;
        selectedItem = 0;
        selectableTexts = GetComponentsInChildren<SelectableText>();
    }
    public void BuySelectInit()
    {
        state = ShopState.BuySelect;
        selectedItem = 0;
        itemSlots = GetComponentsInChildren<ItemUI>();
        possessionText.text = "";
        BuyShow();
    }
    public void SellSelectInit()
    {
        state = ShopState.SellSelect;
        selectedItem = 0;
        itemSlots = GetComponentsInChildren<ItemUI>();
        SellShow();
    }

    public void BuyInit()
    {
        state = ShopState.Buy;
        Debug.Log("買取初期化");
        sumItem = 0;
        itemSumGoldText.text = "合計：";
        itemSumText.text = "購入個数：0";
    }
    public void SellInit()
    {
        state = ShopState.Sell;
        Debug.Log("売却初期化");
        sumItem = 0;
        itemSumGoldText.text = "合計：";
        itemSumText.text = "売却個数：0";
    }
    public void SelectActionOpen()
    {
        selectActionPanel.SetActive(true);
        SelectActionInit();
    }

    public void BuySelectOpen()
    {
        ShopPanel.SetActive(true);
        shopStatusPanel.SetActive(true);
        selectActionPanel.SetActive(false);
        BuySelectInit();
    }

    public void BuySelectClose()
    {
        ShopPanel.SetActive(false);
        shopStatusPanel.SetActive(false);
        StartCoroutine(DialogManager.Instance.TypeDialog("まだ何か取引するかい？"));
        SelectActionOpen();
    }
    public void SellSelectOpen()
    {
        ShopPanel.SetActive(true);
        shopStatusPanel.SetActive(true);
        selectActionPanel.SetActive(false);
        SellSelectInit();
    }

    public void SellSelectClose()
    {
        ShopPanel.SetActive(false);
        StartCoroutine(DialogManager.Instance.TypeDialog("まだ何か取引するかい？"));
        shopStatusPanel.SetActive(false);
        SelectActionOpen();
    }

    public void BuyOpen()
    {
        shopValuePanel.SetActive(true);
        BuyInit();
    }

    public IEnumerator Buy()
    {
        OnBuyItem(items[selectedItem], sumItem);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        BuySelectOpen();
    }

    public void BuyClose()
    {
        state = ShopState.BuySelect;
        shopValuePanel.SetActive(false);
    }
    public void SelectActionClose()
    {
        selectActionPanel.SetActive(false);
    }
    public void SellOpen()
    {
        shopValuePanel.SetActive(true);
        SellInit();
    }

    public IEnumerator Sell()
    {
        OnSellItem(inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].item,sumItem);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        SellSelectOpen();
    }

    public void SellClose()
    {
        state = ShopState.SellSelect;
        shopValuePanel.SetActive(false);
    }
}