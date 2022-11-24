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
    BuyCharaSelect,
    SellSelect,
    SellCharaSelect,
    SellItemTypeSelect,
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
    [SerializeField] GameObject selectSellCharaPanel;
    [SerializeField] GameObject selectSellItemTypePanel;
    [SerializeField] GameObject selectBuyCharaPanel;
    [SerializeField] Text itemAmountText;
    [SerializeField] Text hasGoldText;
    [SerializeField] Text possessionText;
    List<Item> items = new List<Item>();
    [SerializeField] GameObject shopValuePanel;
    [SerializeField] Text itemSumText;
    [SerializeField] Text itemSumGoldText;

     ShopState state;
    int selectedChara;
    int selectedItemType;
    int selectedItem;
    int selectedBuyChara;
    int sumItem=0;
    int shopNpcIndex;
    ItemUI[] itemSlots;
    SelectableText[] itemCharas;
    SelectableText[] buyCharas;
    SelectableText[] itemTypes;
    SelectableText[] selectableTexts;


    public int SelectedChara { get => selectedChara;}
    public int SelectedItemType { get => selectedItemType;}
    public int SelectedItem { get => selectedItem; }




    internal ShopState State { get => state;}
    public Text HasGoldText { get => hasGoldText; set => hasGoldText = value; }
    public int SumItem { get => sumItem; set => sumItem = value; }
    public int SelectedBuyChara { get => selectedBuyChara;}
    public int ShopNpcIndex { get => shopNpcIndex;}

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
            if (inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items.Count <= i || inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[i] == null || inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[i].item.Base == null)
            {
                itemSlots[i].SetText("-");
                itemSlots[i].transform.Find("GoldText").GetComponent<Text>().text = "";
            }
            else
            {
                itemSlots[i].SetText($"{inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[i].item.Base.GetKanjiName()}");
                itemSlots[i].transform.Find("GoldText").GetComponent<Text>().text = $"{inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[i].item.Base.GetGold()}" + "G";
            }
        }
    }
    void SellCharaShow()
    {
        for (int i = 0; i < itemCharas.Length; i++)
        {
            if (player.Battlers.Count==2)
            {
                itemCharas[i].SetText($"{player.Battlers[i].Base.Name}");
            }
            else
            {
                itemCharas[0].SetText($"{player.Battlers[0].Base.Name}");
                itemCharas[1].SetText("-");
            }
        }
    }
    void BuyCharaShow()
    {
        for (int i = 0; i < buyCharas.Length; i++)
        {
            if (player.Battlers.Count==2)
            {
                buyCharas[i].SetText($"{player.Battlers[i].Base.Name}");
            }
            else
            {
                buyCharas[0].SetText($"{player.Battlers[0].Base.Name}");
                buyCharas[1].SetText("-");
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
                        itemAmountText.text = inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].item.Base.GetInformation();
                        hasGoldText.text = $"所持金：{player.Battlers[0].Gold}G";
                        possessionText.text = $"個数：{inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession}";
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
        if (state == ShopState.SellCharaSelect)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedChara--;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedChara++;
            }

            selectedChara = Mathf.Clamp(selectedChara, 0, itemCharas.Length - 1);

            for (int i = 0; i < itemCharas.Length; i++)
            {
                bool selected = selectedChara == i;
                itemCharas[i].SetSelectedColor(selected);
            }
        }
        if (state == ShopState.SellItemTypeSelect)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedItemType --;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedItemType++;
            }

            selectedItemType = Mathf.Clamp(selectedItemType, 0, itemTypes.Length - 1);

            for (int i = 0; i < itemTypes.Length; i++)
            {
                bool selected = selectedItemType == i;
                itemTypes[i].SetSelectedColor(selected);
            }
        }
        if (state == ShopState.BuyCharaSelect)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                switch (selectedBuyChara)
                {
                    case 0:
                        selectedBuyChara ++;
                        break;
                    case 1:
                        selectedBuyChara--;
                        break;
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                switch (selectedBuyChara)
                {
                    case 0:
                        selectedBuyChara++;
                        break;
                    case 1:
                        selectedBuyChara--;
                        break;
                }
            }

            if (selectedBuyChara < 0)
            {
                selectedBuyChara = 0;
            }
            selectedBuyChara = Mathf.Clamp(selectedBuyChara, 0, buyCharas.Length - 1);

            for (int i = 0; i < buyCharas.Length; i++)
            {
                bool selected = selectedBuyChara == i;
                buyCharas[i].SetSelectedColor(selected);
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
                sumItem = Mathf.Clamp(sumItem, 0, inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession);
                itemSumGoldText.text = $"合計：{inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].item.Base.GetGold() * sumItem}";
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
        if (inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem] == null ||
            inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].item.Base == null|| inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].item.Base.GetKanjiName() == "未所持")
        {
            return false;
        }

        return true;
    }

    public void SelectActionInit()
    {
        state = ShopState.SelectAction;
        selectedItem = 0;
        selectableTexts = selectActionPanel.GetComponentsInChildren<SelectableText>();
    }
    public void BuySelectInit()
    {
        state = ShopState.BuySelect;
        selectedItem = 0;
        itemSlots = ShopPanel.GetComponentsInChildren<ItemUI>();
        possessionText.text = "";
        BuyShow();
    }
    public void BuyCharaInit()
    {
        state = ShopState.BuyCharaSelect;
        selectedBuyChara = 0;
        buyCharas = selectBuyCharaPanel.GetComponentsInChildren<SelectableText>();
        BuyCharaShow();
    }
    public void SellSelectInit()
    {
        state = ShopState.SellSelect;
        selectedItem = 0;
        itemSlots = ShopPanel.GetComponentsInChildren<ItemUI>();
        SellShow();
    }
    public void SellCharaInit()
    {
        state = ShopState.SellCharaSelect;
        selectedChara = 0;
        itemCharas =selectSellCharaPanel.GetComponentsInChildren<SelectableText>();
        SellCharaShow();
    }
    public void SellItemTypeInit()
    {
        state = ShopState.SellItemTypeSelect;
        selectedItemType = 0;
        itemTypes = selectSellItemTypePanel.GetComponentsInChildren<SelectableText>();
    }

    public void BuyInit()
    {
        state = ShopState.Buy;
        sumItem = 0;
        itemSumGoldText.text = "合計：";
        itemSumText.text = "購入個数：0";
    }
    public void SellInit()
    {
        state = ShopState.Sell;
        sumItem = 0;
        itemSumGoldText.text = "合計：";
        itemSumText.text = "売却個数：0";
    }
    public void SelectActionOpen(List<Item> item,int index)
    {
        selectActionPanel.SetActive(true);
        shopNpcIndex = index;
        items = item;
        SelectActionInit();
    }

    public void BuySelectOpen()
    {
        shopValuePanel.SetActive(false);
        selectBuyCharaPanel.SetActive(false);
        ShopPanel.SetActive(true);
        shopStatusPanel.SetActive(true);
        selectActionPanel.SetActive(false);
        BuySelectInit();
    }

    public void BuySelectClose()
    {
        ShopPanel.SetActive(false);
        shopStatusPanel.SetActive(false);
        StartCoroutine(DialogManager.Instance.FieldTypeDialog("まだ何か取引するかい？"));
        SelectActionOpen(this.items,shopNpcIndex);
    }
    public void BuyCharaOpen()
    {
        selectBuyCharaPanel.SetActive(true);
        BuyCharaInit();
    }

    public void SellSelectOpen()
    {
        selectActionPanel.SetActive(false);       
        ShopPanel.SetActive(true);
        shopStatusPanel.SetActive(true);
        SellSelectInit();
    }

    public void SellSelectClose()
    {
        ShopPanel.SetActive(false);
        shopStatusPanel.SetActive(false);
        SelectSellItemTypeOpen();
    }

    public void SelectSellCharaOpen()
    {
        selectSellCharaPanel.SetActive(true);
        SellCharaInit();
    }
    public void SelectSellCharaClose()
    {
        selectSellCharaPanel.SetActive(false);
        SelectActionOpen(this.items,shopNpcIndex);
        StartCoroutine(DialogManager.Instance.FieldTypeDialog("まだ何か取引するかい？"));
    }
    public void SelectSellItemTypeOpen()
    {
        selectActionPanel.SetActive(true);
        selectSellItemTypePanel.SetActive(true);
        SellItemTypeInit();
    }
    public void SelectSellItemTypeClose()
    {
        selectSellItemTypePanel.SetActive(false);
        SelectSellCharaOpen();
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