using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


enum GameState
{
    FreeRoam,
    Battle,
    OpenMenu,
    OpenStatusUI,
    OpenInventory,
    OpenFieldSkill,
    OpenConfig,
    Shop,
    ShowDialog,
    Busy,
    Ending,
}

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] ItemInventory inventory;
    [SerializeField] TreasureBoxController treasureBoxController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] FieldSkillController fieldSkill;
    [SerializeField] StatusUI statusUI;
    [SerializeField] ShopController shop;
    [SerializeField] ShopNPCController shopNPC;
    [SerializeField] ItemController item;
    [SerializeField] ConfigController config;
    [SerializeField] GameObject menuBar;
    [SerializeField] GameState state;
    public static GameController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }



    private void Start()
    {
        player.OnEncounts += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
        battleSystem.OnDropItem += TryAdd;
        item.OnMenuSelect += StartSelect;
        item.MenuSelectEnd += EndSelect;
        item.OnInventorySelect += ItemSelect;
        inventoryUI.OnEquip += Equip;
        inventoryUI.OnUsedItem += UsedItem;
        inventoryUI.OnSelectStart += ItemSelect;
        fieldSkill.OnSkill += FieldSkill;
        shopNPC.OnShopStart += StartShop;
        shop.OnBuyItem += ShopAddStart;
        shop.OnSellItem += ShopSellStart;
        DialogManager.Instance.OnShowDialog += ShowDialog;
        DialogManager.Instance.OnCloseDialog += CloseDialog;

        for (int i = 0; i < treasureBoxController.BoxBase.Count; i++)
        {
            treasureBoxController.BoxBase[i].OnTreasureBoxItem += TreasureAddItem;

        }
    }

    void ItemSelect()
    {
        state = GameState.OpenInventory;
        inventoryUI.ItemCharaSelectOpen();
    }
    void UsedItem()
    {
        state = GameState.OpenInventory;
        inventoryUI.UseItemCharaSelectClose();
        inventoryUI.ItemSelectOpen();
    }


    void ConfigSelect()
    {
        state = GameState.OpenConfig;
        config.ConfigOpen();
    }

    void FieldSkill()
    {
        state = GameState.OpenFieldSkill;
        fieldSkill.UseSkillCharaSelectOpen();
    }


    void ShowDialog()
    {
        state = GameState.ShowDialog;
    }

    void CloseDialog()
    {
        if (state == GameState.ShowDialog && shop.State == ShopState.Buy)
        {
            state = GameState.Shop;
        }
        else if (state == GameState.ShowDialog)
        {
            state = GameState.FreeRoam;
        }
    }

    public int GetBattlerMember()
    {
        return player.Battlers.Count;
    }


    void Update()
    {
        switch (state)
        {
            case GameState.FreeRoam:
                HandleUpdateFreeRoam();
                break;
            case GameState.Battle:
                battleSystem.HandleUpdate();
                break;
            case GameState.OpenInventory:
                HandleUpdateInventory();
                break;
            case GameState.OpenFieldSkill:
                HandleUpdateSkill();
                break;
            case GameState.OpenMenu:
                HandleUpdateOpenMenu();
                break;
            case GameState.OpenConfig:
                HandleConfig();
                break;
            case GameState.Shop:
                HandleUpdateShop();
                break;
            case GameState.ShowDialog:
                DialogManager.Instance.HandleUpdate();
                break;
        }

    }

    void HandleUpdateFreeRoam()
    {
        player.HandleUpdate();
        if (Input.GetKeyDown(KeyCode.X))
        {
            StartSelect();
        }
    }

    void HandleUpdateOpenMenu()
    {
        item.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            switch (item.State)
            {
                case MenuState.SelectStart:
                    if (item.MenuSelectionUI.SelectedIndex == 0)
                    {

                        item.StatusUI.Open();
                        item.State = MenuState.Status;
                    }
                    else if (item.MenuSelectionUI.SelectedIndex == 1)
                    {
                        item.InventorySelection();
                        ItemSelect();
                    }
                    else if (item.MenuSelectionUI.SelectedIndex == 2)
                    {
                        item.SkillSelection();
                        StartFieldSkill();
                    }
                    else if (item.MenuSelectionUI.SelectedIndex == 3)
                    {
                        item.ConfigSelection();
                        ConfigSelect();
                    }
                    else if (item.MenuSelectionUI.SelectedIndex == 4)
                    {

                        item.MenuSelectionUI.Close();
                        item.State = MenuState.End;
                        item.MenuSelectEnd();
                    }
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            switch (item.State)
            {
                case MenuState.Status:
                    statusUI.Close();
                    item.Open();
                    break;
            }
        }
    }

    void HandleUpdateInventory()
    {
        inventoryUI.HandleUpdateItemSelection();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (inventoryUI.State == ItemStatus.CharaSelect)
            {
                if (inventoryUI.SelectedChara < player.Battlers.Count)
                {
                    inventoryUI.ItemTypeSelectOpen();
                }
            }
            else if (inventoryUI.State == ItemStatus.ItemTypeSelect)
            {
                item.Close();
                inventoryUI.ItemCharaSelectClose();
                inventoryUI.ItemTypeSelectClose();
                inventoryUI.ItemSelectOpen();
            }
            else if (inventoryUI.State == ItemStatus.ItemSelect)
            {
                if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[inventoryUI.SelectedItem].item.Base.GetKanjiName() == "������")
                {
                    UsedItem();
                }
                else
                {
                    inventoryUI.UseItemCharaSelectOpen();
                }
            }
            else if (inventoryUI.State == ItemStatus.ItemUseCharaSelect)
            {
                if (inventoryUI.SelectedItemUseChara < player.Battlers.Count)
                {
                    state = GameState.Busy;
                    inventoryUI.UseItemCharaSelectClose();
                    StartCoroutine(inventoryUI.Use());
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (inventoryUI.State == ItemStatus.CharaSelect)
            {
                state = GameState.OpenMenu;
                inventoryUI.ItemCharaSelectClose();
                item.Open();
            }
            else if (inventoryUI.State == ItemStatus.ItemTypeSelect)
            {
                inventoryUI.ItemTypeSelectClose();
                inventoryUI.ItemCharaSelectOpen();
            }
            else if (inventoryUI.State == ItemStatus.ItemSelect)
            {
                inventoryUI.ItemSelectClose();
                item.Open();
                inventoryUI.ItemCharaSelectOpen();
                inventoryUI.ItemTypeSelectOpen();
            }
            else if (inventoryUI.State == ItemStatus.ItemUseCharaSelect)
            {
                inventoryUI.UseItemCharaSelectClose();
                inventoryUI.ItemSelectOpen();
            }
        }
    }
    void HandleUpdateSkill()
    {
        fieldSkill.HandleUpdateSkillSelection();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (fieldSkill.State == SkillStatus.CharaSelect)
            {
                if (fieldSkill.SelectedChara < player.Battlers.Count)
                {
                    item.Close();
                    fieldSkill.SkillSelectOpen();
                }
            }
            else if (fieldSkill.State == SkillStatus.SkillSelect && fieldSkill.CanSelectedSkill())
            {
                if (player.Battlers[fieldSkill.SelectedChara].Moves[fieldSkill.SelectedSkill].Base.Target == MoveTarget.Foe)
                {
                    state = GameState.Busy;
                    StartCoroutine(fieldSkill.Skill());
                }
                fieldSkill.UseSkillCharaSelectOpen();
            }
            else if (fieldSkill.State == SkillStatus.SkillUseCharaSelect)
            {
                state = GameState.Busy;
                StartCoroutine(fieldSkill.Skill());
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (fieldSkill.State == SkillStatus.CharaSelect)
            {
                state = GameState.OpenMenu;
                fieldSkill.SkillCharaSelectClose();
            }
            else if (fieldSkill.State == SkillStatus.SkillSelect)
            {
                fieldSkill.SkillSelectClose();
                item.Open();
                fieldSkill.SkillCharaSelectOpen();
            }
            else if (fieldSkill.State == SkillStatus.SkillUseCharaSelect)
            {
                fieldSkill.UseSkillCharaSelectClose();
                fieldSkill.SkillSelectOpen();
            }
        }
    }

    void HandleConfig()
    {
        config.HandleUpdate();
        if (Input.GetKeyDown(KeyCode.X))
        {
            switch (config.ConfigState)
            {
                case ConfigState.Select:
                    state = GameState.OpenMenu;
                    item.State = MenuState.SelectStart;
                    config.ConfigClose();
                    break;
                default:
                    break;
            }
        }
    }
    void HandleUpdateShop()
    {
        shop.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (shop.State == ShopState.SelectAction)
            {
                if (shop.SelectedItem == 0)
                {
                    shop.BuySelectOpen();
                }
                else if (shop.SelectedItem == 1)
                {
                    shop.SelectSellCharaOpen();
                }
                else if (shop.SelectedItem == 2)
                {
                    EndShop();
                }
            }
            else if (shop.State == ShopState.BuySelect)
            {
                shop.BuyOpen();
            }
            else if (shop.State == ShopState.BuyCharaSelect)
            {
                switch (shop.SelectedBuyChara)
                {
                    case 0:
                        StartCoroutine(shop.Buy());
                        break;
                    case 1:
                        if (player.Battlers.Count == 2)
                        {
                            StartCoroutine(shop.Buy());
                        }
                        break;
                    default:
                        break;
                }

            }
            else if (shop.State == ShopState.Buy)
            {
                shop.BuyCharaOpen();
            }
            else if (shop.State == ShopState.SellCharaSelect)
            {
                switch (shop.SelectedChara)
                {
                    case 0:
                        shop.SelectSellItemTypeOpen();
                        break;
                    case 1:
                        if (player.Battlers.Count == 2)
                        {
                            shop.SelectSellItemTypeOpen();
                        }
                        break;
                    default:
                        break;
                }
            }
            else if (shop.State == ShopState.SellItemTypeSelect)
            {
                shop.SellSelectOpen();
            }
            else if (shop.State == ShopState.SellSelect)
            {
                if (shop.CanSelectedSellItem())
                {
                    shop.SellOpen();
                }
            }
            else if (shop.State == ShopState.Sell)
            {
                StartCoroutine(shop.Sell());
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {

            if (shop.State == ShopState.BuySelect)
            {
                shop.BuySelectClose();
            }
            else if (shop.State == ShopState.Buy)
            {
                shop.BuyClose();
            }
            else if (shop.State == ShopState.BuyCharaSelect)
            {
                shop.BuySelectOpen();
            }
            else if (shop.State == ShopState.SellCharaSelect)
            {
                shop.SelectSellCharaClose();
            }
            else if (shop.State == ShopState.SellItemTypeSelect)
            {
                shop.SelectSellItemTypeClose();
            }
            else if (shop.State == ShopState.SellSelect)
            {
                shop.SellSelectClose();
            }
            else if (shop.State == ShopState.Sell)
            {
                shop.SellClose();
            }
        }
    }


    public void StartBattle(Battler enemy)
    {
        state = GameState.Battle;
        enemy.Init();
        menuBar.gameObject.SetActive(false);
        player.gameObject.SetActive(false);
        battleSystem.gameObject.SetActive(true);
        battleSystem.BattleStart(player, enemy);
    }


    void EndBattle(bool win)
    {
        battleSystem.gameObject.SetActive(false);
        menuBar.gameObject.SetActive(true);
        player.gameObject.SetActive(true);
        if (win)
        {
            player.StartPlayer();
            state = GameState.FreeRoam;
            DialogManager.Instance.Close();

        }
        else
        {
            player.StartPlayer();
            state = GameState.FreeRoam;
            DialogManager.Instance.Close();
        }
    }

    public void BossBattle(Battler wildBattler)
    {
        state = GameState.Battle;
        menuBar.gameObject.SetActive(false);
        player.gameObject.SetActive(false);
        battleSystem.BattleStart(player, wildBattler, true);
    }


    public void StartSelect()
    {
        state = GameState.OpenMenu;
        player.StartSelect();
        item.Open();
        menuBar.SetActive(false);

    }
    public void EndSelect()
    {
        state = GameState.FreeRoam;
        item.Close();
        menuBar.SetActive(true);
        player.StartPlayer();
    }

    public void StartFieldSkill()
    {
        state = GameState.OpenFieldSkill;
        menuBar.SetActive(false);
        fieldSkill.SkillCharaSelectOpen();
    }

    public void StartShop()
    {
        state = GameState.Shop;
        menuBar.SetActive(false);
        shop.SelectActionOpen();
    }
    public void EndShop()
    {
        state = GameState.FreeRoam;
        menuBar.SetActive(true);
        shop.SelectActionClose();
        DialogManager.Instance.Close();
        player.StartPlayer();
    }

    public void ShopAddStart(Item item, int sumItem)
    {
        StartCoroutine(ShopAddItem(item, sumItem));
    }
    public void ShopSellStart(Item item, int sumItem)
    {
        StartCoroutine(ShopSellItem(item, sumItem));
    }


    public void TryAdd(Item item)
    {
        for (int i = 0; i < inventory.ItemCharas[0].ItemTypes[0].Items.Count; i++)
        {
            if (item.Base.GetItemType() == Type.Weapon || item.Base.GetItemType() == Type.Armor || item.Base.GetItemType() == Type.Accessory)
            {
                if (inventory.ItemCharas[0].ItemTypes[1].Items[i].item.Base.GetHiraganaName() == item.Base.GetHiraganaName())
                {
                    inventory.ItemCharas[0].ItemTypes[1].Items[i].possession++;
                    return;
                }

            }

            if (item.Base.GetItemType() == Type.Valuables)
            {
                if (inventory.ItemCharas[0].ItemTypes[2].Items[i].item.Base.GetHiraganaName() == item.Base.GetHiraganaName())
                {
                    inventory.ItemCharas[0].ItemTypes[2].Items[i].possession++;
                    return;
                }

            }
            else
            {
                if (inventory.ItemCharas[0].ItemTypes[0].Items[i].item.Base.GetHiraganaName() == item.Base.GetHiraganaName())
                {
                    inventory.ItemCharas[0].ItemTypes[0].Items[i].possession++;
                    return;
                }
            }
        }
        for (int i = 0; i < inventory.ItemCharas[0].ItemTypes[0].Items.Count; i++)
        {
            if (item.Base.GetItemType() == Type.Weapon || item.Base.GetItemType() == Type.Armor || item.Base.GetItemType() == Type.Accessory)
            {
                if (inventory.ItemCharas[0].ItemTypes[1].Items[i].item.Base.GetKanjiName() == "������")
                {
                    inventory.ItemCharas[0].ItemTypes[1].Items[i].item = item;
                    inventory.ItemCharas[0].ItemTypes[1].Items[i].possession++;
                    return;
                }
            }
            else if (item.Base.GetItemType() == Type.Valuables)
            {
                if (inventory.ItemCharas[0].ItemTypes[2].Items[i].item.Base.GetKanjiName() == "������")
                {
                    inventory.ItemCharas[0].ItemTypes[2].Items[i].item = item;
                    inventory.ItemCharas[0].ItemTypes[2].Items[i].possession++;
                    return;
                }
            }
            else
            {
                if (inventory.ItemCharas[0].ItemTypes[0].Items[i].item.Base.GetKanjiName() == "������")
                {
                    inventory.ItemCharas[0].ItemTypes[0].Items[i].item = item;
                    inventory.ItemCharas[0].ItemTypes[0].Items[i].possession++;
                    return;
                }
            }

        }
        for (int i = 0; i < inventory.ItemCharas[0].ItemTypes[0].Items.Count; i++)
        {
            if (item.Base.GetItemType() == Type.Weapon || item.Base.GetItemType() == Type.Armor || item.Base.GetItemType() == Type.Accessory)
            {
                if (inventory.ItemCharas[1].ItemTypes[1].Items[i].item.Base.GetHiraganaName() == item.Base.GetHiraganaName())
                {
                    inventory.ItemCharas[1].ItemTypes[1].Items[i].possession++;
                    return;
                }

            }

            if (item.Base.GetItemType() == Type.Valuables)
            {
                if (inventory.ItemCharas[1].ItemTypes[2].Items[i].item.Base.GetHiraganaName() == item.Base.GetHiraganaName())
                {
                    inventory.ItemCharas[1].ItemTypes[2].Items[i].possession++;
                    return;
                }

            }
            else
            {
                if (inventory.ItemCharas[1].ItemTypes[0].Items[i].item.Base.GetHiraganaName() == item.Base.GetHiraganaName())
                {
                    inventory.ItemCharas[1].ItemTypes[0].Items[i].possession++;
                    return;
                }
            }
        }
        for (int i = 0; i < inventory.ItemCharas[0].ItemTypes[0].Items.Count; i++)
        {
            if (item.Base.GetItemType() == Type.Weapon || item.Base.GetItemType() == Type.Armor || item.Base.GetItemType() == Type.Accessory)
            {
                if (inventory.ItemCharas[1].ItemTypes[1].Items[i].item.Base.GetKanjiName() == "������")
                {
                    inventory.ItemCharas[1].ItemTypes[1].Items[i].item = item;
                    inventory.ItemCharas[1].ItemTypes[1].Items[i].possession++;
                    return;
                }
            }
            else if (item.Base.GetItemType() == Type.Valuables)
            {
                if (inventory.ItemCharas[1].ItemTypes[2].Items[i].item.Base.GetKanjiName() == "������")
                {
                    inventory.ItemCharas[1].ItemTypes[2].Items[i].item = item;
                    inventory.ItemCharas[1].ItemTypes[2].Items[i].possession++;
                    return;
                }
            }
            else
            {
                if (inventory.ItemCharas[1].ItemTypes[0].Items[i].item.Base.GetKanjiName() == "������")
                {
                    inventory.ItemCharas[1].ItemTypes[0].Items[i].item = item;
                    inventory.ItemCharas[1].ItemTypes[0].Items[i].possession++;
                    return;
                }
            }
        }

    }
    public void TreasureAddItem(Item item)
    {
        for (int i = 0; i < inventory.ItemCharas[0].ItemTypes[0].Items.Count; i++)
        {
            if (item.Base.GetItemType() == Type.Weapon | item.Base.GetItemType() == Type.Armor || item.Base.GetItemType() == Type.Accessory)
            {
                if (inventory.ItemCharas[0].ItemTypes[1].Items[i].item.Base.GetHiraganaName() == item.Base.GetHiraganaName())
                {
                    inventory.ItemCharas[0].ItemTypes[1].Items[i].possession++;
                    return;
                }

            }
            if (item.Base.GetItemType() == Type.Valuables)
            {
                if (inventory.ItemCharas[0].ItemTypes[2].Items[i].item.Base.GetHiraganaName() == item.Base.GetHiraganaName())
                {
                    inventory.ItemCharas[0].ItemTypes[2].Items[i].possession++;
                    return;
                }

            }
            else
            {
                if (inventory.ItemCharas[0].ItemTypes[0].Items[i].item.Base.GetHiraganaName() == item.Base.GetHiraganaName())
                {
                    inventory.ItemCharas[0].ItemTypes[0].Items[i].possession++;
                    return;
                }
            }
        }
        for (int i = 0; i < inventory.ItemCharas[0].ItemTypes[0].Items.Count; i++)
        {
            if (item.Base.GetItemType() == Type.Weapon || item.Base.GetItemType() == Type.Armor || item.Base.GetItemType() == Type.Accessory)
            {
                if (inventory.ItemCharas[0].ItemTypes[1].Items[i].item.Base.GetKanjiName() == "������")
                {
                    inventory.ItemCharas[0].ItemTypes[1].Items[i].item = item;
                    inventory.ItemCharas[0].ItemTypes[1].Items[i].possession++;
                    return;
                }
            }
            else if (item.Base.GetItemType() == Type.Valuables)
            {
                if (inventory.ItemCharas[0].ItemTypes[2].Items[i].item.Base.GetKanjiName() == "������")
                {
                    inventory.ItemCharas[0].ItemTypes[2].Items[i].item = item;
                    inventory.ItemCharas[0].ItemTypes[2].Items[i].possession++;
                    return;
                }
            }
            else
            {
                if (inventory.ItemCharas[0].ItemTypes[0].Items[i].item.Base.GetKanjiName() == "������")
                {
                    inventory.ItemCharas[0].ItemTypes[0].Items[i].item = item;
                    inventory.ItemCharas[0].ItemTypes[0].Items[i].possession++;
                    return;
                }
            }
        }

    }
    public IEnumerator ShopAddItem(Item item, int sumItem)
    {
        if (sumItem <= 0)
        {
            shop.BuyInit();
            yield break;
        }
        if (player.Battlers[0].Gold >= item.Base.GetGold() && player.Battlers[0].Gold >= item.Base.GetGold() * sumItem)
        {
            player.Battlers[0].Gold -= item.Base.GetGold() * sumItem;
            for (int i = 0; i < inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[0].Items.Count; i++)
            {
                if (item.Base.GetItemType() == Type.Weapon || item.Base.GetItemType() == Type.Armor || item.Base.GetItemType() == Type.Accessory)
                {
                    if (inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[1].Items[i].item.Base.GetHiraganaName() == item.Base.GetHiraganaName())
                    {
                        inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[1].Items[i].possession += sumItem;
                        shop.HasGoldText.text = $"�������F{player.Battlers[0].Gold}G";
                        shop.BuyClose();
                        yield return StartCoroutine(DialogManager.Instance.FieldTypeDialog($"{item.Base.GetKanjiName()}��{sumItem}����\n���ɉ����v�����͂��邩���H"));
                        shop.BuySelectOpen();
                        yield break;
                    }
                }
                else if (item.Base.GetItemType() == Type.Valuables)
                {
                    if (inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[2].Items[i].item.Base.GetHiraganaName() == item.Base.GetHiraganaName())
                    {
                        inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[2].Items[i].possession += sumItem;
                        shop.HasGoldText.text = $"�������F{player.Battlers[0].Gold}G";
                        shop.BuyClose();
                        yield return StartCoroutine(DialogManager.Instance.FieldTypeDialog($"{item.Base.GetKanjiName()}��{sumItem}����\n���ɉ����v�����͂��邩���H"));
                        shop.BuySelectOpen();
                        yield break;
                    }
                }
                else
                {
                    if (inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[0].Items[i].item.Base.GetHiraganaName() == item.Base.GetHiraganaName())
                    {
                        inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[0].Items[i].possession += sumItem;
                        shop.HasGoldText.text = $"�������F{player.Battlers[0].Gold}G";
                        shop.BuyClose();
                        yield return StartCoroutine(DialogManager.Instance.FieldTypeDialog($"{item.Base.GetKanjiName()}��{sumItem}����\n���ɉ����v�����͂��邩���H"));
                        shop.BuySelectOpen();
                        yield break;
                    }
                }
            }
            for (int i = 0; i < inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[0].Items.Count; i++)
            {
                if (item.Base.GetItemType() == Type.Weapon || item.Base.GetItemType() == Type.Armor || item.Base.GetItemType() == Type.Accessory)
                {
                    if (inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[1].Items[i].item.Base.GetKanjiName() == "������")
                    {
                        inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[1].Items[i].item = item;
                        inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[1].Items[i].possession += sumItem;
                        shop.HasGoldText.text = $"�������F{player.Battlers[0].Gold}G";
                        shop.BuyClose();
                        yield return StartCoroutine(DialogManager.Instance.FieldTypeDialog($"{item.Base.GetKanjiName()}��{sumItem}����\n���ɉ����v�����͂��邩���H"));
                        shop.BuySelectOpen();
                        yield break;
                    }
                }
                else if (item.Base.GetItemType() == Type.Valuables)
                {
                    if (inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[2].Items[i].item.Base.GetKanjiName() == "������")
                    {
                        inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[2].Items[i].item = item;
                        inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[2].Items[i].possession += sumItem;
                        shop.HasGoldText.text = $"�������F{player.Battlers[0].Gold}G";
                        shop.BuyClose();
                        yield return StartCoroutine(DialogManager.Instance.FieldTypeDialog($"{item.Base.GetKanjiName()}��{sumItem}����\n���ɉ����v�����͂��邩���H"));
                        shop.BuySelectOpen();
                        yield break;
                    }
                }
                else
                {
                    if (inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[0].Items[i].item.Base.GetKanjiName() == "������")
                    {
                        inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[0].Items[i].item = item;
                        inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[0].Items[i].possession += sumItem;
                        shop.HasGoldText.text = $"�������F{player.Battlers[0].Gold}G";
                        shop.BuyClose();
                        yield return StartCoroutine(DialogManager.Instance.FieldTypeDialog($"{item.Base.GetKanjiName()}��{sumItem}����\n���ɉ����v�����͂��邩���H"));
                        shop.BuySelectOpen();
                        yield break;
                    }
                }
            }
        }
        else
        {
            shop.BuyClose();
            yield return StartCoroutine(DialogManager.Instance.FieldTypeDialog($"����H�S�[���h������Ȃ��悤����"));
            shop.BuySelectOpen();
            yield break;
        }
    }

    public IEnumerator ShopSellItem(Item item, int sumItem)
    {
        if (sumItem <= 0)
        {
            shop.SellInit();
            yield break; ;
        }
        for (int i = 0; i < inventory.ItemCharas[shop.SelectedChara].ItemTypes[shop.SelectedItemType].Items.Count; i++)
        {

            if (inventory.ItemCharas[shop.SelectedChara].ItemTypes[shop.SelectedItemType].Items[i].item.Base.GetHiraganaName() == item.Base.GetHiraganaName())
            {
                if (inventory.ItemCharas[shop.SelectedChara].ItemTypes[shop.SelectedItemType].Items[i].possession >= sumItem)
                {
                    inventory.ItemCharas[shop.SelectedChara].ItemTypes[shop.SelectedItemType].Items[i].possession -= sumItem;
                    player.Battlers[0].Gold += item.Base.GetGold() * sumItem;
                    shop.HasGoldText.text = $"�������F{player.Battlers[0].Gold}G";
                    if (inventory.ItemCharas[shop.SelectedChara].ItemTypes[shop.SelectedItemType].Items[i].possession == 0)
                    {
                        inventory.ItemCharas[shop.SelectedChara].ItemTypes[shop.SelectedItemType].Items[i].item = inventory.NoneItem;
                    }
                    shop.SellClose();
                    shop.SellSelectOpen();
                    yield return StartCoroutine(DialogManager.Instance.FieldTypeDialog($"{item.Base.GetKanjiName()}��{sumItem}����\n���ɉ����������͂��邩���H"));
                    yield break;
                }
            }
        }
    }
    public void Equip(Item item)
    {
        if (item.Base.GetItemType() == Type.Weapon)
        {
            Item tmp;
            if (player.Battlers[inventoryUI.SelectedItemUseChara].Base.GetEquipWeapon().Base.GetKanjiName() != "������")
            {
                //���ꂩ����鑕���ƌ��X���Ă������������������̏ꍇ
                if (item.Base.GetHiraganaName() == player.Battlers[inventoryUI.SelectedItemUseChara].Base.GetEquipWeapon().Base.GetHiraganaName())
                {
                    inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[inventoryUI.SelectedItem].possession++;
                    return;
                }
                //���ꂩ����鑕���ƌ��X���Ă����������Ⴄ�����̏ꍇ
                else if (item.Base.GetHiraganaName() != player.Battlers[inventoryUI.SelectedItemUseChara].Base.GetEquipWeapon().Base.GetHiraganaName())
                {
                    tmp = player.Battlers[inventoryUI.SelectedItemUseChara].Base.GetEquipWeapon();
                    player.Battlers[inventoryUI.SelectedItemUseChara].Base.SetEquipWeapon(item);
                    //���ꂩ����鑕����-1����Ă��܂������c���Ă��鎞�ɑ��̂Ƃ���Ɍ��X���Ă�������������ꍇ
                    if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[inventoryUI.SelectedItem].possession >= 1)
                    {
                        for (int k = 0; k < inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items.Count; k++)
                        {
                            //���̂Ƃ���Ɍ��X���Ă������������鎞����������Inventory���ɂ���ꍇ
                            if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[k].item.Base.GetHiraganaName() == tmp.Base.GetHiraganaName())
                            {
                                inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[k].possession++;
                                return;
                            }
                        }
                        for (int j = 0; j < inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items.Count; j++)
                        {
                            //���̂Ƃ���Ɍ��X���Ă������������鎞����������Inventory���ɂȂ��ꍇNull�ɓ����
                            if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[j].item.Base.GetKanjiName() == "������")
                            {
                                inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[j].item = tmp;
                                inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[j].possession++;
                                return;
                            }
                        }
                    }
                    //���ꂩ����鑕����-1����ď����Ă����Ɍ��X���Ă�������������ꍇ
                    else if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[inventoryUI.SelectedItem].possession == 0)
                    {
                        for (int k = 0; k < inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items.Count; k++)
                        {
                            if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[k].item.Base.GetHiraganaName() == tmp.Base.GetHiraganaName())
                            {
                                inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[k].possession++;
                                return;
                            }
                        }
                        for (int j = 0; j < inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items.Count; j++)
                        {
                            //���̂Ƃ���Ɍ��X���Ă������������鎞����������Inventory���ɂȂ��ꍇNull�ɓ����
                            if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[j].item.Base.GetKanjiName() == "������")
                            {
                                inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[j].item = tmp;
                                inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[j].possession++;
                                return;
                            }
                        }
                    }
                }
            }
            else
            {
                player.Battlers[inventoryUI.SelectedItemUseChara].Base.SetEquipWeapon(item);
                return;
            }
        }
        else if (item.Base.GetItemType() == Type.Armor)
        {
            Item tmp;
            if (player.Battlers[inventoryUI.SelectedItemUseChara].Base.GetEquipArmor().Base.GetKanjiName() != "������")
            {
                //���ꂩ����鑕���ƌ��X���Ă������������������̏ꍇ
                if (item.Base.GetHiraganaName() == player.Battlers[inventoryUI.SelectedItemUseChara].Base.GetEquipArmor().Base.GetHiraganaName())
                {
                    inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[inventoryUI.SelectedItem].possession++;
                    return;
                }
                //���ꂩ����鑕���ƌ��X���Ă����������Ⴄ�����̏ꍇ
                else if (item.Base.GetHiraganaName() != player.Battlers[inventoryUI.SelectedItemUseChara].Base.GetEquipArmor().Base.GetHiraganaName())
                {
                    tmp = player.Battlers[inventoryUI.SelectedItemUseChara].Base.GetEquipArmor();
                    player.Battlers[inventoryUI.SelectedItemUseChara].Base.SetEquipArmor(item);
                    //���ꂩ����鑕����-1����Ă��܂������c���Ă��鎞�ɑ��̂Ƃ���Ɍ��X���Ă�������������ꍇ
                    if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[inventoryUI.SelectedItem].possession >= 1)
                    {
                        for (int k = 0; k < inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items.Count; k++)
                        {
                            //���̂Ƃ���Ɍ��X���Ă������������鎞����������Inventory���ɂ���ꍇ
                            if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[k].item.Base.GetHiraganaName() == tmp.Base.GetHiraganaName())
                            {
                                inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[k].possession++;
                                return;
                            }
                        }
                        for (int j = 0; j < inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items.Count; j++)
                        {
                            //���̂Ƃ���Ɍ��X���Ă������������鎞����������Inventory���ɂȂ��ꍇNull�ɓ����
                            if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[j].item.Base.GetKanjiName() == "������")
                            {
                                inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[j].item = tmp;
                                inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[j].possession++;
                                return;
                            }
                        }
                    }
                    //���ꂩ����鑕����-1����ď����Ă����Ɍ��X���Ă�������������ꍇ
                    else if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[inventoryUI.SelectedItem].possession == 0)
                    {
                        for (int k = 0; k < inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items.Count; k++)
                        {
                            if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[k].item.Base.GetHiraganaName() == tmp.Base.GetHiraganaName())
                            {
                                inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[k].possession++;
                                return;
                            }
                        }
                        for (int j = 0; j < inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items.Count; j++)
                        {
                            //���̂Ƃ���Ɍ��X���Ă������������鎞����������Inventory���ɂȂ��ꍇNull�ɓ����
                            if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[j].item.Base.GetKanjiName() == "������")
                            {
                                inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[j].item = tmp;
                                inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[j].possession++;
                                return;
                            }
                        }
                    }
                }
            }
            else
            {
                player.Battlers[inventoryUI.SelectedItemUseChara].Base.SetEquipArmor(item);
                return;
            }
        }
        else if (item.Base.GetItemType() == Type.Accessory)
        {
            Item tmp;
            if (player.Battlers[inventoryUI.SelectedItemUseChara].Base.GetEquipAccessory().Base.GetKanjiName() != "������")
            {
                //���ꂩ����鑕���ƌ��X���Ă������������������̏ꍇ
                if (item.Base.GetHiraganaName() == player.Battlers[inventoryUI.SelectedItemUseChara].Base.GetEquipAccessory().Base.GetHiraganaName())
                {
                    inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[inventoryUI.SelectedItem].possession++;
                    return;
                }
                //���ꂩ����鑕���ƌ��X���Ă����������Ⴄ�����̏ꍇ
                else if (item.Base.GetHiraganaName() != player.Battlers[inventoryUI.SelectedItemUseChara].Base.GetEquipAccessory().Base.GetHiraganaName())
                {
                    tmp = player.Battlers[inventoryUI.SelectedItemUseChara].Base.GetEquipAccessory();
                    player.Battlers[inventoryUI.SelectedItemUseChara].Base.SetEquipAccessory(item);
                    //���ꂩ����鑕����-1����Ă��܂������c���Ă��鎞�ɑ��̂Ƃ���Ɍ��X���Ă�������������ꍇ
                    if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[inventoryUI.SelectedItem].possession >= 1)
                    {
                        for (int k = 0; k < inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items.Count; k++)
                        {
                            //���̂Ƃ���Ɍ��X���Ă������������鎞����������Inventory���ɂ���ꍇ
                            if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[k].item.Base.GetHiraganaName() == tmp.Base.GetHiraganaName())
                            {
                                inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[k].possession++;
                                return;
                            }
                        }
                        for (int j = 0; j < inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items.Count; j++)
                        {
                            //���̂Ƃ���Ɍ��X���Ă������������鎞����������Inventory���ɂȂ��ꍇNull�ɓ����
                            if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[j].item.Base.GetKanjiName() == "������")
                            {
                                inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[j].item = tmp;
                                inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[j].possession++;
                                return;
                            }
                        }
                    }
                    //���ꂩ����鑕����-1����ď����Ă����Ɍ��X���Ă�������������ꍇ
                    else if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[inventoryUI.SelectedItem].possession == 0)
                    {
                        for (int k = 0; k < inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items.Count; k++)
                        {
                            if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[k].item.Base.GetHiraganaName() == tmp.Base.GetHiraganaName())
                            {
                                inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[k].possession++;
                                return;
                            }
                        }
                        for (int j = 0; j < inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items.Count; j++)
                        {
                            //���̂Ƃ���Ɍ��X���Ă������������鎞����������Inventory���ɂȂ��ꍇNull�ɓ����
                            if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[j].item.Base.GetKanjiName() == "������")
                            {
                                inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[j].item = tmp;
                                inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[j].possession++;
                                return;
                            }
                        }
                    }
                }
            }
            else
            {
                player.Battlers[inventoryUI.SelectedItemUseChara].Base.SetEquipAccessory(item);
                return;
            }
        }

    }
}