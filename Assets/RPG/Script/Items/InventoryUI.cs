using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;
using System;
enum ItemStatus
{
    CharaSelect,
    ItemTypeSelect,
    ItemSelect,
    ItemUseCharaSelect,
}

public class InventoryUI : MonoBehaviour
{
    [SerializeField] ItemInventory inventory;
    [SerializeField] GameObject descriptionPanel;
    [SerializeField] GameObject itemCharaPanel;
    [SerializeField] GameObject itemTypePanel;
    [SerializeField] GameObject itemPanel;
    [SerializeField] GameObject itemUseCharaPanel;
    [SerializeField] InventoryStatusUI inventoryStatusUI;
    [SerializeField] Text informationText;
    [SerializeField] Text informationValueText;
    [SerializeField] PlayerController player;
    public UnityAction OnUsedItem;
    public UnityAction OnSelectStart;
    public UnityAction<Item> OnEquip;
    public UnityAction OnUsedItemKeep;
    ItemUI[] itemSlots;
    ItemCharaUI[] itemCharas;
    ItemTypeUI[] itemTypes;
    ItemUseCharaUI[] itemUses;
    int selectedChara;
    int selectedItemType;
    int selectedItem;
    int selectedItemUseChara;
    ItemStatus state;

    public int SelectedItemType { get => selectedItemType; }
    public int SelectedItem { get => selectedItem; }
    public ItemInventory Inventory { get => inventory; }
    internal ItemStatus State { get => state; }
    public int SelectedItemUseChara { get => selectedItemUseChara; }
    public int SelectedChara { get => selectedChara; set => selectedChara = value; }

    public void ItemCharaInit()
    {
        selectedChara = 0;
        itemCharas = GetComponentsInChildren<ItemCharaUI>();

        ItemCharaShow();
    }

    public void ItemTypeInit()
    {
        selectedItemType = 0;
        itemTypes = GetComponentsInChildren<ItemTypeUI>();

        ItemTypeShow();
    }

    public void ItemInit()
    {
        selectedItem = 0;
        itemSlots = GetComponentsInChildren<ItemUI>();

        ItemShow();
    }
    public void ItemUseInit()
    {
        selectedItemUseChara = 0;
        itemUses = GetComponentsInChildren<ItemUseCharaUI>();

        ItemUseCharaShow();
    }

    void ItemCharaShow()
    {
        for (int i = 0; i < itemCharas.Length; i++)
        {
            if (i == 0)
            {
                itemCharas[i].SetText("��l��");
            }
            else if (i == 1)
            {
                if (player.Battlers.Count == 2)
                {
                    itemCharas[i].SetText("�A�~�i");
                }
                else
                {
                    itemCharas[i].SetText("-");
                }
            }

        }
    }
    void ItemTypeShow()
    {
        for (int i = 0; i < itemTypes.Length; i++)
        {
            if (i == 0)
            {
                itemTypes[i].SetText("����A�C�e��");
            }
            else if (i == 1)
            {
                itemTypes[i].SetText("����");
            }
            else if (i == 2)
            {
                itemTypes[i].SetText("��؂Ȃ���");
            }
        }
    }
    void ItemShow()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[i].item.Base.GetKanjiName() == "������")
            {

                itemSlots[i].SetText("-");
                itemSlots[i].transform.Find("PosessionText").GetComponent<Text>().text = "";
            }
            else if (inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[i].item.Base.GetKanjiName() != "������")
            {
                itemSlots[i].SetText($"{inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[i].item.Base.GetKanjiName()}");
                itemSlots[i].transform.Find("PosessionText").GetComponent<Text>().text = "�~ " + $"{inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[i].possession}";
            }

        }
    }
    void ItemUseCharaShow()
    {
        for (int i = 0; i < itemUses.Length; i++)
        {
            if (i == 0)
            {
                itemUses[i].SetText("��l��");
            }
            else if (i == 1)
            {
                if (player.Battlers.Count == 2)
                {
                    itemUses[i].SetText("�A�~�i");
                }
                else
                {
                    itemUses[i].SetText("-");
                }
            }

        }
    }

    public void HandleUpdateItemSelection()
    {
        if (state == ItemStatus.CharaSelect)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (selectedChara == 0)
                {
                    selectedChara++;
                }
                else
                {
                    selectedChara--;
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (selectedChara == 1)
                {
                    selectedChara--;
                }
                else
                {
                    selectedChara++;
                }
            }

            selectedChara = Mathf.Clamp(selectedChara, 0, 1);

            for (int i = 0; i < itemCharas.Length; i++)
            {
                bool selected = SelectedChara == i;
                itemCharas[i].SetSelectedColor(selected);
            }
        }
        else if (state == ItemStatus.ItemTypeSelect)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedItemType--;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedItemType++;
            }
            selectedItemType = Mathf.Clamp(selectedItemType, 0, 2);
            for (int i = 0; i < itemTypes.Length; i++)
            {
                bool selected = selectedItemType == i;
                itemTypes[i].SetSelectedColor(selected);
            }

        }
        else if (state == ItemStatus.ItemSelect)
        {

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (selectedItem == 0 || selectedItem == 1)
                {
                    selectedItem += 8;
                }
                else
                {
                    selectedItem -= 2;

                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (selectedItem == 8 || selectedItem == 9)
                {
                    selectedItem -= 8;
                }
                else
                {
                    selectedItem += 2;

                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (selectedItem == 0)
                {
                    selectedItem += 9;
                }
                else
                {
                    selectedItem--;

                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (selectedItem == 9)
                {
                    selectedItem -= 9;
                }
                else
                {
                    selectedItem++;

                }
            }

            selectedItem = Mathf.Clamp(selectedItem, 0, itemSlots.Length - 1);

            for (int i = 0; i < itemSlots.Length; i++)
            {
                bool selected = selectedItem == i;
                if (selected)
                {
                    if (CanSelectedItem())
                    {
                        descriptionPanel.SetActive(true);
                        if (selectedItemType == 1)
                        {
                            if (inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[i].item.Base.GetItemType() == Type.Weapon)
                            {
                                informationText.text = inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[i].item.Base.GetInformation();
                                informationValueText.text = "�U���́F" + $"{inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[i].item.Base.GetAmount()}";
                            }
                            else
                            {
                                informationText.text = inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[i].item.Base.GetInformation();
                                informationValueText.text = "�h��́F" + $"{inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[i].item.Base.GetAmount()}";
                            }
                        }
                        else
                        {
                            informationText.text = inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[i].item.Base.GetInformation();
                            informationValueText.text = "�񕜗ʁF" + $"{inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[i].item.Base.GetAmount()}";
                        }
                    }
                    else
                    {
                        descriptionPanel.SetActive(false);
                    }
                }
                itemSlots[i].SetSelectedColor(selected);
            }
        }
        else if (state == ItemStatus.ItemUseCharaSelect)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (selectedItemUseChara == 0)
                {
                    selectedItemUseChara++;
                }
                else
                {
                    selectedItemUseChara--;
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (selectedItemUseChara == 1)
                {
                    selectedItemUseChara--;
                }
                else
                {
                    selectedItemUseChara++;
                }
            }

            selectedItemUseChara = Mathf.Clamp(selectedItemUseChara, 0, 1);

            for (int i = 0; i < itemUses.Length; i++)
            {
                bool selected = selectedItemUseChara == i;
                itemUses[i].SetSelectedColor(selected);
            }
        }

    }

    public bool CanSelectedItem()
    {
        if (inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].item.Base.GetKanjiName() == "������")
        {
            return false;
        }

        return true;
    }


    public IEnumerator Use()
    {
        if (inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].item.Base.GetKanjiName() == "������")
        {
            OnUsedItem?.Invoke();
        }
        else
        {
            Item useItem = inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].item;
            if (useItem.Base.itemType == Type.LowContinuation)
            {
                if (player.Battlers[selectedItemUseChara].Status == null)
                {
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetKanjiName()}�͎g��Ȃ��Ă����I"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    descriptionPanel.SetActive(false);
                    OnUsedItem?.Invoke();
                    yield break;
                }
                else if (player.Battlers[selectedItemUseChara].Status.Id == ConditionID.LowContinuation)
                {
                    inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession--;
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetKanjiName()}���g����"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    inventory.Use(useItem, selectedItemUseChara);
                    inventoryStatusUI.PlayerUpdateUI(player.Battlers[selectedItemUseChara]);
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"�f���C�������܂���"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    if (inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession <= 0)
                    {
                        inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].item = inventory.NoneItem;
                        descriptionPanel.SetActive(false);
                        OnUsedItem?.Invoke();
                    }
                    else if (inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession > 0)
                    {
                        descriptionPanel.SetActive(false);
                        OnUsedItem?.Invoke();
                    }
                    DialogManager.Instance.Close();
                }
            }
            else if (useItem.Base.itemType == Type.LowContinuation)
            {
                if (player.Battlers[selectedItemUseChara].Status == null)
                {
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetKanjiName()}�͎g��Ȃ��Ă����I"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    descriptionPanel.SetActive(false);
                    OnUsedItem?.Invoke();
                    yield break;
                }

                else if (player.Battlers[selectedItemUseChara].Status.Id == ConditionID.HighContinuation)
                {
                    inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession--;
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetKanjiName()}���g����"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    inventory.Use(useItem, selectedItemUseChara);
                    inventoryStatusUI.PlayerUpdateUI(player.Battlers[selectedItemUseChara]);
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"�q�f�������܂���"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    if (inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession <= 0)
                    {
                        inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].item = inventory.NoneItem;
                        descriptionPanel.SetActive(false);
                        OnUsedItem?.Invoke();
                    }
                    else if (inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession > 0)
                    {
                        descriptionPanel.SetActive(false);
                        OnUsedItem?.Invoke();
                    }
                    DialogManager.Instance.Close();
                }
            }
            else if (useItem.Base.itemType == Type.Barn)
            {
                if (player.Battlers[selectedItemUseChara].Status == null)
                {
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetKanjiName()}�͎g��Ȃ��Ă����I"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    descriptionPanel.SetActive(false);
                    OnUsedItem?.Invoke();
                    yield break;
                }
                else if (player.Battlers[selectedItemUseChara].Status.Id == ConditionID.LowContinuation)
                {
                    inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession--;
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetKanjiName()}���g����"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    inventory.Use(useItem, selectedItemUseChara);
                    inventoryStatusUI.PlayerUpdateUI(player.Battlers[selectedItemUseChara]);
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"��ÂɂȂ��Ă���"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    if (inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession <= 0)
                    {
                        inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].item = inventory.NoneItem;
                        descriptionPanel.SetActive(false);
                        OnUsedItem?.Invoke();
                    }
                    else if (inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession > 0)
                    {
                        descriptionPanel.SetActive(false);
                        OnUsedItem?.Invoke();
                    }
                    DialogManager.Instance.Close();
                }
            }
            else if (useItem.Base.itemType == Type.Binding)
            {
                if (player.Battlers[selectedItemUseChara].Status == null)
                {
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetKanjiName()}�͎g��Ȃ��Ă����I"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    descriptionPanel.SetActive(false);
                    OnUsedItem?.Invoke();
                    yield break;
                }
                else if (player.Battlers[selectedItemUseChara].Status.Id == ConditionID.LowContinuation)
                {
                    inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession--;
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetKanjiName()}���g����"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    inventory.Use(useItem, selectedItemUseChara);
                    inventoryStatusUI.PlayerUpdateUI(player.Battlers[selectedItemUseChara]);
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"���������Ď��肩�瓦����"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    if (inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession <= 0)
                    {
                        inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].item = inventory.NoneItem;
                        descriptionPanel.SetActive(false);
                        OnUsedItem?.Invoke();
                    }
                    else if (inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession > 0)
                    {
                        descriptionPanel.SetActive(false);
                        OnUsedItem?.Invoke();
                    }
                    DialogManager.Instance.Close();
                }
            }
            else if (useItem.Base.itemType == Type.Freeze)
            {
                if (player.Battlers[selectedItemUseChara].Status == null)
                {
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetKanjiName()}�͎g��Ȃ��Ă����I"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    descriptionPanel.SetActive(false);
                    OnUsedItem?.Invoke();
                    yield break;
                }
                else if (player.Battlers[selectedItemUseChara].Status.Id == ConditionID.LowContinuation)
                {
                    inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession--;
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetKanjiName()}���g����"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    inventory.Use(useItem, selectedItemUseChara);
                    inventoryStatusUI.PlayerUpdateUI(player.Battlers[selectedItemUseChara]);
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"�ӎ������߂���"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    if (inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession <= 0)
                    {
                        inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].item = inventory.NoneItem;
                        descriptionPanel.SetActive(false);
                        OnUsedItem?.Invoke();
                    }
                    else if (inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession > 0)
                    {
                        descriptionPanel.SetActive(false);
                        OnUsedItem?.Invoke();
                    }
                    DialogManager.Instance.Close();
                }
            }
            else if (useItem.Base.itemType == Type.Paralisis)
            {
                if (player.Battlers[selectedItemUseChara].Status == null)
                {
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetKanjiName()}�͎g��Ȃ��Ă����I"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    descriptionPanel.SetActive(false);
                    OnUsedItem?.Invoke();
                    yield break;
                }
                else if (player.Battlers[selectedItemUseChara].Status.Id == ConditionID.LowContinuation)
                {
                    inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession--;
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetKanjiName()}���g����"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    inventory.Use(useItem, selectedItemUseChara);
                    inventoryStatusUI.PlayerUpdateUI(player.Battlers[selectedItemUseChara]);
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"���炾�̂ӂ邦�������܂���"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    if (inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession <= 0)
                    {
                        inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].item = inventory.NoneItem;
                        descriptionPanel.SetActive(false);
                        OnUsedItem?.Invoke();
                    }
                    else if (inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession > 0)
                    {
                        descriptionPanel.SetActive(false);
                        OnUsedItem?.Invoke();
                    }
                    DialogManager.Instance.Close();
                }
            }
            else if (useItem.Base.GetItemType() == Type.HPFullRecovery)
            {
                if (player.Battlers[selectedItemUseChara].MP == player.Battlers[selectedItemUseChara].MaxMP)
                {
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetKanjiName()}�͎g��Ȃ��Ă����I"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    DialogManager.Instance.Close();
                    descriptionPanel.SetActive(false);
                    OnUsedItem?.Invoke();
                    yield break;
                }
                else
                {
                    inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession--;
                    ItemShow();
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetKanjiName()}���g����"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    inventory.Use(useItem, selectedItemUseChara);
                    inventoryStatusUI.PlayerUpdateUI(player.Battlers[selectedItemUseChara]);
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"MP���S�񕜂���"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    if (inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession <= 0)
                    {
                        inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].item = inventory.NoneItem;
                        descriptionPanel.SetActive(false);
                        OnUsedItem?.Invoke();
                    }
                    else if (inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession > 0)
                    {
                        descriptionPanel.SetActive(false);
                        OnUsedItem?.Invoke();
                    }
                    DialogManager.Instance.Close();
                }
            }
            else if (useItem.Base.GetItemType() == Type.MPFullRecovery)
            {
                if (player.Battlers[selectedItemUseChara].HP == player.Battlers[selectedItemUseChara].MaxHP)
                {
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetKanjiName()}�͎g��Ȃ��Ă����I"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    DialogManager.Instance.Close();
                    descriptionPanel.SetActive(false);
                    OnUsedItem?.Invoke();
                    yield break;
                }
                else
                {
                    inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession--;
                    ItemShow();
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetKanjiName()}���g����"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    inventory.Use(useItem, selectedItemUseChara);
                    inventoryStatusUI.PlayerUpdateUI(player.Battlers[selectedItemUseChara]);
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"HP���S�񕜂���"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    if (inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession <= 0)
                    {
                        inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].item = inventory.NoneItem;
                        descriptionPanel.SetActive(false);
                        OnUsedItem?.Invoke();
                    }
                    else if (inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession > 0)
                    {
                        descriptionPanel.SetActive(false);
                        OnUsedItem?.Invoke();
                    }
                    DialogManager.Instance.Close();
                }
            }
            else if (useItem.Base.GetItemType() == Type.Weapon || useItem.Base.GetItemType() == Type.Armor || useItem.Base.GetItemType() == Type.Accessory)
            {
                inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession--;
                yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetKanjiName()}�𑕔�����"));
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                OnEquip?.Invoke(useItem);
                inventoryStatusUI.PlayerUpdateUI(player.Battlers[selectedItemUseChara]);
                yield return null;
                if (inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession <= 0)
                {
                    inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].item = inventory.NoneItem;
                    descriptionPanel.SetActive(false);
                    OnUsedItem?.Invoke();
                }
                else if (inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession > 0)
                {
                    descriptionPanel.SetActive(false);
                    OnUsedItem?.Invoke();
                }
                DialogManager.Instance.Close();
            }
            else if (useItem.Base.GetItemType() == Type.HPRecovery)
            {
                if (player.Battlers[selectedItemUseChara].HP == player.Battlers[selectedItemUseChara].MaxHP)
                {
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetKanjiName()}�͎g��Ȃ��Ă����I"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    DialogManager.Instance.Close();
                    descriptionPanel.SetActive(false);
                    OnUsedItem?.Invoke();
                    yield break;
                }
                else
                {
                    inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession--;
                    ItemShow();
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetKanjiName()}���g����"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    inventory.Use(useItem, selectedItemUseChara);
                    inventoryStatusUI.PlayerUpdateUI(player.Battlers[selectedItemUseChara]);
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetAmount()}�|�C���g�񕜂���"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    if (inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession <= 0)
                    {
                        inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem] = null;
                    }
                    else if (inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession > 0)
                    {
                        descriptionPanel.SetActive(false);
                        OnUsedItemKeep?.Invoke();
                    }
                    DialogManager.Instance.Close();
                }
            }
            else if (useItem.Base.GetItemType() == Type.MPRecovery)
            {
                if (player.Battlers[selectedItemUseChara].MP == player.Battlers[selectedItemUseChara].MaxMP)
                {
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetKanjiName()}�͎g��Ȃ��Ă����I"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    DialogManager.Instance.Close();
                    descriptionPanel.SetActive(false);
                    OnUsedItem?.Invoke();
                    yield break;
                }
                else
                {
                    inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession--;
                    ItemShow();
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetKanjiName()}���g����"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    inventory.Use(useItem, selectedItemUseChara);
                    inventoryStatusUI.PlayerUpdateUI(player.Battlers[selectedItemUseChara]);
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetAmount()}�|�C���g�񕜂���"));
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                    if (inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession <= 0)
                    {
                        inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem] = null;
                    }
                    else if (inventory.ItemCharas[selectedChara].ItemTypes[selectedItemType].Items[selectedItem].possession > 0)
                    {
                        descriptionPanel.SetActive(false);
                        OnUsedItemKeep?.Invoke();
                    }
                    DialogManager.Instance.Close();
                }
            }
        }
    }

    public IEnumerator UseInBattle(int selectChara,int selectItem,int selectUseChara)
    {
        if (inventory.ItemCharas[selectChara].ItemTypes[selectedItemType].Items[selectItem].item.Base.GetKanjiName() == "������")
        {
            OnUsedItem?.Invoke();
        }
        else if (selectedItemType == 1 || selectedItemType == 2)
        {
            OnUsedItem?.Invoke();

        }
        else
        {
            Item useItem = inventory.ItemCharas[selectChara].ItemTypes[selectedItemType].Items[selectItem].item;
            if (useItem.Base.GetItemType() == Type.LowContinuation)
            {
                inventory.ItemCharas[selectChara].ItemTypes[selectedItemType].Items[selectItem].possession--;
                ItemShow();
                yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetKanjiName()}���g����"));
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                inventory.Use(useItem, selectUseChara);
                inventoryStatusUI.PlayerUpdateUI(player.Battlers[selectUseChara]);
                yield return StartCoroutine(DialogManager.Instance.TypeDialog($"�f���C�������܂���"));
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                if (inventory.ItemCharas[selectChara].ItemTypes[selectedItemType].Items[selectItem].possession <= 0)
                {
                    inventory.ItemCharas[selectChara].ItemTypes[selectedItemType].Items[selectItem].item = inventory.NoneItem;
                }

            }
            else if (useItem.Base.GetItemType() == Type.HighContinuation)
            {
                inventory.ItemCharas[selectChara].ItemTypes[selectedItemType].Items[selectItem].possession--;
                ItemShow();
                yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetKanjiName()}���g����"));
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                inventory.Use(useItem, selectUseChara);
                inventoryStatusUI.PlayerUpdateUI(player.Battlers[selectUseChara]);
                yield return StartCoroutine(DialogManager.Instance.TypeDialog($"�q�f�������܂���"));
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                if (inventory.ItemCharas[selectChara].ItemTypes[selectedItemType].Items[selectItem].possession <= 0)
                {
                    inventory.ItemCharas[selectChara].ItemTypes[selectedItemType].Items[selectItem].item = inventory.NoneItem;
                }
            }
            else if (useItem.Base.GetItemType() == Type.Barn)
            {
                inventory.ItemCharas[selectChara].ItemTypes[selectedItemType].Items[selectItem].possession--;
                ItemShow();
                yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetKanjiName()}���g����"));
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                inventory.Use(useItem, selectUseChara);
                inventoryStatusUI.PlayerUpdateUI(player.Battlers[selectUseChara]);
                yield return StartCoroutine(DialogManager.Instance.TypeDialog($"��ÂɂȂ��Ă���"));
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                if (inventory.ItemCharas[selectChara].ItemTypes[selectedItemType].Items[selectItem].possession <= 0)
                {
                    inventory.ItemCharas[selectChara].ItemTypes[selectedItemType].Items[selectItem].item = inventory.NoneItem;
                }
            }
            else if (useItem.Base.GetItemType() == Type.Binding)
            {
                inventory.ItemCharas[selectChara].ItemTypes[selectedItemType].Items[selectItem].possession--;
                ItemShow();
                yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetKanjiName()}���g����"));
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                inventory.Use(useItem, selectUseChara);
                inventoryStatusUI.PlayerUpdateUI(player.Battlers[selectUseChara]);
                yield return StartCoroutine(DialogManager.Instance.TypeDialog($"�������݂Ď��肩�瓦����"));
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                if (inventory.ItemCharas[selectChara].ItemTypes[selectedItemType].Items[selectItem].possession <= 0)
                {
                    inventory.ItemCharas[selectChara].ItemTypes[selectedItemType].Items[selectItem].item = inventory.NoneItem;
                }
            }
            else if (useItem.Base.GetItemType() == Type.Freeze)
            {
                inventory.ItemCharas[selectChara].ItemTypes[selectedItemType].Items[selectItem].possession--;
                ItemShow();
                yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetKanjiName()}���g����"));
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                inventory.Use(useItem, selectUseChara);
                inventoryStatusUI.PlayerUpdateUI(player.Battlers[selectUseChara]);
                yield return StartCoroutine(DialogManager.Instance.TypeDialog($"�ӎ������߂���"));
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                if (inventory.ItemCharas[selectChara].ItemTypes[selectedItemType].Items[selectItem].possession <= 0)
                {
                    inventory.ItemCharas[selectChara].ItemTypes[selectedItemType].Items[selectItem].item = inventory.NoneItem;
                }
            }
            else if (useItem.Base.GetItemType() == Type.Paralisis)
            {
                inventory.ItemCharas[selectChara].ItemTypes[selectedItemType].Items[selectItem].possession--;
                ItemShow();
                yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetKanjiName()}���g����"));
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                inventory.Use(useItem, selectUseChara);
                inventoryStatusUI.PlayerUpdateUI(player.Battlers[selectUseChara]);
                yield return StartCoroutine(DialogManager.Instance.TypeDialog($"���炾�̂ӂ邦�������܂���"));
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                if (inventory.ItemCharas[selectChara].ItemTypes[selectedItemType].Items[selectItem].possession <= 0)
                {
                    inventory.ItemCharas[selectChara].ItemTypes[selectedItemType].Items[selectItem].item = inventory.NoneItem;
                }
            }
            else if (useItem.Base.GetItemType() == Type.HPFullRecovery || useItem.Base.GetItemType() == Type.MPFullRecovery)
            {
                inventory.ItemCharas[selectChara].ItemTypes[selectedItemType].Items[selectItem].possession--;
                ItemShow();
                yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetKanjiName()}���g����"));
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                inventory.Use(useItem, selectUseChara);
                inventoryStatusUI.PlayerUpdateUI(player.Battlers[selectUseChara]);
                if (useItem.Base.GetItemType() == Type.HPFullRecovery)
                {
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{player.Battlers[selectUseChara].MaxHP}�|�C���g�񕜂���"));
                }
                else if (useItem.Base.GetItemType() == Type.MPFullRecovery)
                {
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{player.Battlers[selectUseChara].MaxMP}�|�C���g�񕜂���"));
                }
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                if (inventory.ItemCharas[selectChara].ItemTypes[selectedItemType].Items[selectItem].possession <= 0)
                {
                    inventory.ItemCharas[selectChara].ItemTypes[selectedItemType].Items[selectItem].item = inventory.NoneItem;
                }
            }
            else
            {
                inventory.ItemCharas[selectChara].ItemTypes[selectedItemType].Items[selectItem].possession--;
                ItemShow();
                yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetKanjiName()}���g����"));
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                inventory.Use(useItem, selectUseChara);
                inventoryStatusUI.PlayerUpdateUI(player.Battlers[selectUseChara]);
                yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{useItem.Base.GetAmount()}�|�C���g�񕜂���"));
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                if (inventory.ItemCharas[selectChara].ItemTypes[selectedItemType].Items[selectItem].possession <= 0)
                {
                    inventory.ItemCharas[selectChara].ItemTypes[selectedItemType].Items[selectItem].item = inventory.NoneItem;
                }
            }
        }
    }



    public void ItemCharaSelectOpen()
    {
        state = ItemStatus.CharaSelect;
        itemCharaPanel.SetActive(true);
        ItemCharaInit();
    }

    public void ItemCharaSelectClose()
    {
        itemCharaPanel.SetActive(false);
    }
    public void ItemTypeSelectOpen()
    {
        state = ItemStatus.ItemTypeSelect;
        itemTypePanel.SetActive(true);
        ItemTypeInit();
    }

    public void ItemTypeSelectClose()
    {
        itemTypePanel.SetActive(false);
    }
    public void ItemSelectOpen()
    {
        state = ItemStatus.ItemSelect;
        itemPanel.SetActive(true);
        inventoryStatusUI.Open();
        ItemInit();
    }

    public void ItemSelectClose()
    {
        itemPanel.SetActive(false);
        descriptionPanel.SetActive(false);
        inventoryStatusUI.Close();
    }
    public void UseItemCharaSelectOpen()
    {
        selectedItemUseChara = 0;
        state = ItemStatus.ItemUseCharaSelect;
        itemUseCharaPanel.SetActive(true);
        ItemUseInit();
    }

    public void UseItemCharaSelectClose()
    {
        itemUseCharaPanel.SetActive(false);
        descriptionPanel.SetActive(false);
    }
}