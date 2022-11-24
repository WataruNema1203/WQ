using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
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
    Event,
    Ending,
}

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] ItemInventory inventory;
    [SerializeField] TreasureBoxManager treasureBoxController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] FieldSkillController fieldSkill;
    [SerializeField] StatusUI statusUI;
    [SerializeField] ShopController shop;
    [SerializeField] GameObject shopNPCCase;
    [SerializeField] ItemController item;
    [SerializeField] ConfigController config;
    [SerializeField] GameObject menuBar;
    [SerializeField] GameState state;
    [SerializeField] List<Character> playerFungus = new List<Character>();
    [SerializeField] Flowchart flowchart;
    public static GameController Instance { get; private set; }
    bool isOutSide;
    string[] names = { "PlayerName", "AminaName" };
    internal GameState State { get => state; set => state = value; }
    public ShopNPCController[] ShopNPCs { get => shopNPCs; }
    public bool IsOutSide { get => isOutSide; set => isOutSide = value; }
    public List<Character> PlayerFungus { get => playerFungus; }
    public string[] Names { get => names; set => names = value; }

    ShopNPCController[] shopNPCs;
    readonly List<Battler> battlers = new List<Battler>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }



    private void Start()
    {
        shopNPCs = shopNPCCase.transform.GetComponentsInChildren<ShopNPCController>();
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

        config.OnConfigClose+= ConfigEnd;

        shop.OnBuyItem += ShopAddStart;
        shop.OnSellItem += ShopSellStart;

        DialogManager.Instance.OnShowDialog += ShowDialog;
        DialogManager.Instance.OnCloseDialog += CloseDialog;

        player.Battlers[0].Base.SetNeme($"{flowchart.GetVariable($"{Names[0]}")}");

        if (player.Battlers.Count != 1)
        {
            player.Battlers[1].Base.SetNeme($"{flowchart.GetVariable($"{Names[1]}")}");
        }

        for (int i = 0; i < shopNPCs.Length; i++)
        {
            shopNPCs[i].OnShopStart += StartShop;

        }
        for (int i = 0; i < treasureBoxController.BoxBase.Count; i++)
        {
            treasureBoxController.BoxBase[i].OnTreasureBoxItem += TreasureAddItem;

        }
    }

    public void StartEvent()
    {
        state = GameState.Event;
    }

    public void EndEvent()
    {
        state = GameState.FreeRoam;
        player.gameObject.SetActive(true);
        player.StartPlayer();
    }

    void Showstatus()
    {
        state = GameState.OpenStatusUI;
        statusUI.Open();
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

    void ConfigEnd()
    {
        state = GameState.OpenMenu;
        item.State = MenuState.SelectStart;
        config.ConfigClose();

    }

    void FieldSkill()
    {
        state = GameState.OpenFieldSkill;
        fieldSkill.SkillSelectOpen();
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
            case GameState.OpenStatusUI:
                HandleUpdateStatus();
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
        if (Input.GetKeyDown(KeyCode.X) && !player.IsSelect)
        {
            StartSelect();
        }
        else if (Input.GetKeyDown(KeyCode.Z) && !player.IsSelect)
        {
            player.Interact();
            player.WarpFlag();
            player.HiddenItem();
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
                        Showstatus();
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
                    statusUI.StatusClose();
                    item.Open();
                    break;
            }
        }
    }

    void HandleConfig()
    {
        config.HandleUpdate();
    }

    void HandleUpdateStatus()
    {
        if (statusUI.StatusState == StatusState.SelectChara)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (statusUI.selectedChara == 0)
                {
                    statusUI.selectedChara++;
                }
                else
                {
                    statusUI.selectedChara--;
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (statusUI.selectedChara == 1)
                {
                    statusUI.selectedChara--;
                }
                else
                {
                    statusUI.selectedChara++;
                }
            }

            statusUI.selectedChara = Mathf.Clamp(statusUI.selectedChara, 0, 1);

            for (int i = 0; i < statusUI.StatusCharas.Length; i++)
            {
                bool selected = statusUI.selectedChara == i;
                statusUI.StatusCharas[i].SetSelectedColor(selected);
            }
        }


        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (statusUI.StatusState == StatusState.SelectChara)
            {
                if (statusUI.selectedChara < player.Battlers.Count)
                {
                    statusUI.StatusOpen();
                }
            }
            else if (statusUI.StatusState == StatusState.ShowStatus)
            {
                statusUI.TextClose();
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (statusUI.StatusState == StatusState.SelectChara)
            {
                state = GameState.OpenMenu;
                statusUI.Close();
                item.Open();
            }
            else if (statusUI.StatusState == StatusState.ShowStatus)
            {
                statusUI.StatusClose();
            }
            else if (statusUI.StatusState == StatusState.ShowCharaImage)
            {
                statusUI.TextOpen();
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
                if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[inventoryUI.SelectedItem].item.Base.GetKanjiName() == "未所持")
                {
                    UsedItem();
                }
                else if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[inventoryUI.SelectedItem].item.Base.GetItemType() == MoveType.Valuables)
                {
                    state = GameState.Busy;
                    StartCoroutine(inventoryUI.Use());

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
                if (player.Battlers[fieldSkill.SelectedChara].FieldMoves[fieldSkill.SelectedSkill].Base.Category2 == MoveCategory2.Warp)
                {
                    fieldSkill.WarpPointSelectOpen();
                }
                else if (player.Battlers[fieldSkill.SelectedChara].FieldMoves[fieldSkill.SelectedSkill].Base.Target == MoveTarget.Foe)
                {
                    state = GameState.Busy;
                    StartCoroutine(fieldSkill.Skill());
                }
                else
                {
                    fieldSkill.UseSkillCharaSelectOpen();
                }
            }
            else if (fieldSkill.State == SkillStatus.WarpPointSelect && fieldSkill.CanSelectedSkill())
            {
                state = GameState.Busy;
                fieldSkill.WarpPointPanel.SetActive(false);
                if (fieldSkill.Warps[fieldSkill.SelectedWarpPoint] == null)
                {
                    state = GameState.OpenFieldSkill;
                }
                else
                {
                    StartCoroutine(fieldSkill.Skill());
                }
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
                item.Open();
            }
            else if (fieldSkill.State == SkillStatus.SkillSelect)
            {
                fieldSkill.SkillSelectClose();
                item.Open();
                fieldSkill.SkillCharaSelectOpen();
            }
            else if (fieldSkill.State == SkillStatus.WarpPointSelect)
            {
                fieldSkill.WarpPointSelectClose();
                fieldSkill.SkillSelectOpen();
            }
            else if (fieldSkill.State == SkillStatus.SkillUseCharaSelect)
            {
                fieldSkill.UseSkillCharaSelectClose();
                fieldSkill.SkillSelectOpen();
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


    public void StartBattle(List<Battler> enemes)
    {
        state = GameState.Battle;
        for (int i = 0; i < enemes.Count; i++)
        {
            enemes[i].Init();
        }
        menuBar.gameObject.SetActive(false);
        player.gameObject.SetActive(false);
        battleSystem.gameObject.SetActive(true);
        battleSystem.BattleStart(player, enemes);
    }


    void EndBattle(bool win)
    {
        battleSystem.gameObject.SetActive(false);
        menuBar.gameObject.SetActive(true);
        player.gameObject.SetActive(true);
        player.EnemyBattlers.Clear();
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
        battlers.Clear();
    }

    public void BossBattle(Battler wildBattler)
    {
        state = GameState.Battle;
        menuBar.gameObject.SetActive(false);
        player.gameObject.SetActive(false);
        battlers.Add(wildBattler);
        battleSystem.BattleStart(player, battlers, true);
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

    public void StartShop(List<Item> items, int index)
    {
        state = GameState.Shop;
        menuBar.SetActive(false);
        shop.SelectActionOpen(items, index);
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
            if (item.Base.GetItemType() == MoveType.Weapon || item.Base.GetItemType() == MoveType.Armor || item.Base.GetItemType() == MoveType.Accessory)
            {
                if (inventory.ItemCharas[0].ItemTypes[1].Items[i].item.Base.GetHiraganaName() == item.Base.GetHiraganaName())
                {
                    inventory.ItemCharas[0].ItemTypes[1].Items[i].possession++;
                    return;
                }

            }

            if (item.Base.GetItemType() == MoveType.Valuables)
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
            if (item.Base.GetItemType() == MoveType.Weapon || item.Base.GetItemType() == MoveType.Armor || item.Base.GetItemType() == MoveType.Accessory)
            {
                if (inventory.ItemCharas[0].ItemTypes[1].Items[i].item.Base.GetKanjiName() == "未所持")
                {
                    inventory.ItemCharas[0].ItemTypes[1].Items[i].item = item;
                    inventory.ItemCharas[0].ItemTypes[1].Items[i].possession++;
                    return;
                }
            }
            else if (item.Base.GetItemType() == MoveType.Valuables)
            {
                if (inventory.ItemCharas[0].ItemTypes[2].Items[i].item.Base.GetKanjiName() == "未所持")
                {
                    inventory.ItemCharas[0].ItemTypes[2].Items[i].item = item;
                    inventory.ItemCharas[0].ItemTypes[2].Items[i].possession++;
                    return;
                }
            }
            else
            {
                if (inventory.ItemCharas[0].ItemTypes[0].Items[i].item.Base.GetKanjiName() == "未所持")
                {
                    inventory.ItemCharas[0].ItemTypes[0].Items[i].item = item;
                    inventory.ItemCharas[0].ItemTypes[0].Items[i].possession++;
                    return;
                }
            }

        }
        for (int i = 0; i < inventory.ItemCharas[0].ItemTypes[0].Items.Count; i++)
        {
            if (item.Base.GetItemType() == MoveType.Weapon || item.Base.GetItemType() == MoveType.Armor || item.Base.GetItemType() == MoveType.Accessory)
            {
                if (inventory.ItemCharas[1].ItemTypes[1].Items[i].item.Base.GetHiraganaName() == item.Base.GetHiraganaName())
                {
                    inventory.ItemCharas[1].ItemTypes[1].Items[i].possession++;
                    return;
                }

            }

            if (item.Base.GetItemType() == MoveType.Valuables)
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
            if (item.Base.GetItemType() == MoveType.Weapon || item.Base.GetItemType() == MoveType.Armor || item.Base.GetItemType() == MoveType.Accessory)
            {
                if (inventory.ItemCharas[1].ItemTypes[1].Items[i].item.Base.GetKanjiName() == "未所持")
                {
                    inventory.ItemCharas[1].ItemTypes[1].Items[i].item = item;
                    inventory.ItemCharas[1].ItemTypes[1].Items[i].possession++;
                    return;
                }
            }
            else if (item.Base.GetItemType() == MoveType.Valuables)
            {
                if (inventory.ItemCharas[1].ItemTypes[2].Items[i].item.Base.GetKanjiName() == "未所持")
                {
                    inventory.ItemCharas[1].ItemTypes[2].Items[i].item = item;
                    inventory.ItemCharas[1].ItemTypes[2].Items[i].possession++;
                    return;
                }
            }
            else
            {
                if (inventory.ItemCharas[1].ItemTypes[0].Items[i].item.Base.GetKanjiName() == "未所持")
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
            if (item.Base.GetItemType() == MoveType.Weapon | item.Base.GetItemType() == MoveType.Armor || item.Base.GetItemType() == MoveType.Accessory)
            {
                if (inventory.ItemCharas[0].ItemTypes[1].Items[i].item.Base.GetHiraganaName() == item.Base.GetHiraganaName())
                {
                    inventory.ItemCharas[0].ItemTypes[1].Items[i].possession++;
                    return;
                }

            }
            if (item.Base.GetItemType() == MoveType.Valuables)
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
            if (item.Base.GetItemType() == MoveType.Weapon || item.Base.GetItemType() == MoveType.Armor || item.Base.GetItemType() == MoveType.Accessory)
            {
                if (inventory.ItemCharas[0].ItemTypes[1].Items[i].item.Base.GetKanjiName() == "未所持")
                {
                    inventory.ItemCharas[0].ItemTypes[1].Items[i].item = item;
                    inventory.ItemCharas[0].ItemTypes[1].Items[i].possession++;
                    return;
                }
            }
            else if (item.Base.GetItemType() == MoveType.Valuables)
            {
                if (inventory.ItemCharas[0].ItemTypes[2].Items[i].item.Base.GetKanjiName() == "未所持")
                {
                    inventory.ItemCharas[0].ItemTypes[2].Items[i].item = item;
                    inventory.ItemCharas[0].ItemTypes[2].Items[i].possession++;
                    return;
                }
            }
            else
            {
                if (inventory.ItemCharas[0].ItemTypes[0].Items[i].item.Base.GetKanjiName() == "未所持")
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
        state = GameState.Busy;
        if (sumItem <= 0)
        {
            shop.BuyInit();
            state = GameState.Shop;
            yield break;
        }
        if (player.Battlers[0].Gold >= item.Base.GetGold() && player.Battlers[0].Gold >= item.Base.GetGold() * sumItem)
        {
            player.Battlers[0].Gold -= item.Base.GetGold() * sumItem;
            for (int i = 0; i < inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[0].Items.Count; i++)
            {
                if (item.Base.GetItemType() == MoveType.Weapon || item.Base.GetItemType() == MoveType.Armor || item.Base.GetItemType() == MoveType.Accessory)
                {
                    if (inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[1].Items[i].item.Base.GetHiraganaName() == item.Base.GetHiraganaName())
                    {
                        inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[1].Items[i].possession += sumItem;
                        shop.HasGoldText.text = $"所持金：{player.Battlers[0].Gold}G";
                        shop.BuyClose();
                        yield return StartCoroutine(DialogManager.Instance.FieldTypeDialog($"{item.Base.GetKanjiName()}を{sumItem}個だね\n他に何か要るもんはあるかい？"));
                        shop.BuySelectOpen();
                        state = GameState.Shop;
                        yield break;
                    }
                }
                else if (item.Base.GetItemType() == MoveType.Valuables)
                {
                    if (inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[2].Items[i].item.Base.GetHiraganaName() == item.Base.GetHiraganaName())
                    {
                        inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[2].Items[i].possession += sumItem;
                        shop.HasGoldText.text = $"所持金：{player.Battlers[0].Gold}G";
                        shop.BuyClose();
                        yield return StartCoroutine(DialogManager.Instance.FieldTypeDialog($"{item.Base.GetKanjiName()}を{sumItem}個だね\n他に何か要るもんはあるかい？"));
                        shop.BuySelectOpen();
                        state = GameState.Shop;
                        yield break;
                    }
                }
                else
                {
                    if (inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[0].Items[i].item.Base.GetHiraganaName() == item.Base.GetHiraganaName())
                    {
                        inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[0].Items[i].possession += sumItem;
                        shop.HasGoldText.text = $"所持金：{player.Battlers[0].Gold}G";
                        shop.BuyClose();
                        yield return StartCoroutine(DialogManager.Instance.FieldTypeDialog($"{item.Base.GetKanjiName()}を{sumItem}個だね\n他に何か要るもんはあるかい？"));
                        shop.BuySelectOpen();
                        state = GameState.Shop;
                        yield break;
                    }
                }
            }
            for (int i = 0; i < inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[0].Items.Count; i++)
            {
                if (item.Base.GetItemType() == MoveType.Weapon || item.Base.GetItemType() == MoveType.Armor || item.Base.GetItemType() == MoveType.Accessory)
                {
                    if (inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[1].Items[i].item.Base.GetKanjiName() == "未所持")
                    {
                        inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[1].Items[i].item = item;
                        inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[1].Items[i].possession += sumItem;
                        shop.HasGoldText.text = $"所持金：{player.Battlers[0].Gold}G";
                        shop.BuyClose();
                        yield return StartCoroutine(DialogManager.Instance.FieldTypeDialog($"{item.Base.GetKanjiName()}を{sumItem}個だね\n他に何か要るもんはあるかい？"));
                        shop.BuySelectOpen();
                        state = GameState.Shop;
                        yield break;
                    }
                }
                else if (item.Base.GetItemType() == MoveType.Valuables)
                {
                    if (inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[2].Items[i].item.Base.GetKanjiName() == "未所持")
                    {
                        inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[2].Items[i].item = item;
                        inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[2].Items[i].possession += sumItem;
                        shop.HasGoldText.text = $"所持金：{player.Battlers[0].Gold}G";
                        shop.BuyClose();
                        yield return StartCoroutine(DialogManager.Instance.FieldTypeDialog($"{item.Base.GetKanjiName()}を{sumItem}個だね\n他に何か要るもんはあるかい？"));
                        shop.BuySelectOpen();
                        state = GameState.Shop;
                        yield break;
                    }
                }
                else
                {
                    if (inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[0].Items[i].item.Base.GetKanjiName() == "未所持")
                    {
                        inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[0].Items[i].item = item;
                        inventory.ItemCharas[shop.SelectedBuyChara].ItemTypes[0].Items[i].possession += sumItem;
                        shop.HasGoldText.text = $"所持金：{player.Battlers[0].Gold}G";
                        shop.BuyClose();
                        yield return StartCoroutine(DialogManager.Instance.FieldTypeDialog($"{item.Base.GetKanjiName()}を{sumItem}個だね\n他に何か要るもんはあるかい？"));
                        shop.BuySelectOpen();
                        state = GameState.Shop;
                        yield break;
                    }
                }
            }
        }
        else
        {
            shop.BuyClose();
            yield return StartCoroutine(DialogManager.Instance.FieldTypeDialog($"うん？ゴールドが足りないようだよ"));
            shop.BuySelectOpen();
            state = GameState.Shop;
            yield break;
        }
    }

    public IEnumerator ShopSellItem(Item item, int sumItem)
    {
        state = GameState.Busy;
        if (sumItem <= 0)
        {
            shop.SellInit();
            state = GameState.Shop;
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
                    shop.HasGoldText.text = $"所持金：{player.Battlers[0].Gold}G";
                    if (inventory.ItemCharas[shop.SelectedChara].ItemTypes[shop.SelectedItemType].Items[i].possession == 0)
                    {
                        inventory.ItemCharas[shop.SelectedChara].ItemTypes[shop.SelectedItemType].Items[i].item = inventory.NoneItem;
                    }
                    shop.SellClose();
                    shop.SellSelectOpen();
                    yield return StartCoroutine(DialogManager.Instance.FieldTypeDialog($"{item.Base.GetKanjiName()}を{sumItem}個だね\n他に何か売るもんはあるかい？"));
                    state = GameState.Shop;
                    yield break;
                }
            }
        }
    }
    public void Equip(Item item)
    {
        if (item.Base.GetItemType() == MoveType.Weapon)
        {
            Item tmp;
            if (player.Battlers[inventoryUI.SelectedItemUseChara].Base.GetEquipWeapon().Base.GetKanjiName() != "未所持")
            {
                //これからつける装備と元々つけていた装備が同じ装備の場合
                if (item.Base.GetHiraganaName() == player.Battlers[inventoryUI.SelectedItemUseChara].Base.GetEquipWeapon().Base.GetHiraganaName())
                {
                    inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[inventoryUI.SelectedItem].possession++;
                    return;
                }
                //これからつける装備と元々つけていた装備が違う装備の場合
                else if (item.Base.GetHiraganaName() != player.Battlers[inventoryUI.SelectedItemUseChara].Base.GetEquipWeapon().Base.GetHiraganaName())
                {
                    tmp = player.Battlers[inventoryUI.SelectedItemUseChara].Base.GetEquipWeapon();
                    player.Battlers[inventoryUI.SelectedItemUseChara].Base.SetEquipWeapon(item);
                    //これからつける装備が-1されてもまだ個数が残っている時に他のところに元々つけていた装備を入れる場合
                    if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[inventoryUI.SelectedItem].possession >= 1)
                    {
                        for (int k = 0; k < inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items.Count; k++)
                        {
                            //他のところに元々つけていた装備を入れる時同じ装備がInventory内にある場合
                            if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[k].item.Base.GetHiraganaName() == tmp.Base.GetHiraganaName())
                            {
                                inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[k].possession++;
                                return;
                            }
                        }
                        for (int j = 0; j < inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items.Count; j++)
                        {
                            //他のところに元々つけていた装備を入れる時同じ装備がInventory内にない場合Nullに入れる
                            if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[j].item.Base.GetKanjiName() == "未所持")
                            {
                                inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[j].item = tmp;
                                inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[j].possession++;
                                return;
                            }
                        }
                    }
                    //これからつける装備が-1されて消えてそこに元々つけていた装備を入れる場合
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
                            //他のところに元々つけていた装備を入れる時同じ装備がInventory内にない場合Nullに入れる
                            if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[j].item.Base.GetKanjiName() == "未所持")
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
        else if (item.Base.GetItemType() == MoveType.Armor)
        {
            Item tmp;
            if (player.Battlers[inventoryUI.SelectedItemUseChara].Base.GetEquipArmor().Base.GetKanjiName() != "未所持")
            {
                //これからつける装備と元々つけていた装備が同じ装備の場合
                if (item.Base.GetHiraganaName() == player.Battlers[inventoryUI.SelectedItemUseChara].Base.GetEquipArmor().Base.GetHiraganaName())
                {
                    inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[inventoryUI.SelectedItem].possession++;
                    return;
                }
                //これからつける装備と元々つけていた装備が違う装備の場合
                else if (item.Base.GetHiraganaName() != player.Battlers[inventoryUI.SelectedItemUseChara].Base.GetEquipArmor().Base.GetHiraganaName())
                {
                    tmp = player.Battlers[inventoryUI.SelectedItemUseChara].Base.GetEquipArmor();
                    player.Battlers[inventoryUI.SelectedItemUseChara].Base.SetEquipArmor(item);
                    //これからつける装備が-1されてもまだ個数が残っている時に他のところに元々つけていた装備を入れる場合
                    if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[inventoryUI.SelectedItem].possession >= 1)
                    {
                        for (int k = 0; k < inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items.Count; k++)
                        {
                            //他のところに元々つけていた装備を入れる時同じ装備がInventory内にある場合
                            if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[k].item.Base.GetHiraganaName() == tmp.Base.GetHiraganaName())
                            {
                                inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[k].possession++;
                                return;
                            }
                        }
                        for (int j = 0; j < inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items.Count; j++)
                        {
                            //他のところに元々つけていた装備を入れる時同じ装備がInventory内にない場合Nullに入れる
                            if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[j].item.Base.GetKanjiName() == "未所持")
                            {
                                inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[j].item = tmp;
                                inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[j].possession++;
                                return;
                            }
                        }
                    }
                    //これからつける装備が-1されて消えてそこに元々つけていた装備を入れる場合
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
                            //他のところに元々つけていた装備を入れる時同じ装備がInventory内にない場合Nullに入れる
                            if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[j].item.Base.GetKanjiName() == "未所持")
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
        else if (item.Base.GetItemType() == MoveType.Accessory)
        {
            Item tmp;
            if (player.Battlers[inventoryUI.SelectedItemUseChara].Base.GetEquipAccessory().Base.GetKanjiName() != "未所持")
            {
                //これからつける装備と元々つけていた装備が同じ装備の場合
                if (item.Base.GetHiraganaName() == player.Battlers[inventoryUI.SelectedItemUseChara].Base.GetEquipAccessory().Base.GetHiraganaName())
                {
                    inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[inventoryUI.SelectedItem].possession++;
                    return;
                }
                //これからつける装備と元々つけていた装備が違う装備の場合
                else if (item.Base.GetHiraganaName() != player.Battlers[inventoryUI.SelectedItemUseChara].Base.GetEquipAccessory().Base.GetHiraganaName())
                {
                    tmp = player.Battlers[inventoryUI.SelectedItemUseChara].Base.GetEquipAccessory();
                    player.Battlers[inventoryUI.SelectedItemUseChara].Base.SetEquipAccessory(item);
                    //これからつける装備が-1されてもまだ個数が残っている時に他のところに元々つけていた装備を入れる場合
                    if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[inventoryUI.SelectedItem].possession >= 1)
                    {
                        for (int k = 0; k < inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items.Count; k++)
                        {
                            //他のところに元々つけていた装備を入れる時同じ装備がInventory内にある場合
                            if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[k].item.Base.GetHiraganaName() == tmp.Base.GetHiraganaName())
                            {
                                inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[k].possession++;
                                return;
                            }
                        }
                        for (int j = 0; j < inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items.Count; j++)
                        {
                            //他のところに元々つけていた装備を入れる時同じ装備がInventory内にない場合Nullに入れる
                            if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[j].item.Base.GetKanjiName() == "未所持")
                            {
                                inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[j].item = tmp;
                                inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[j].possession++;
                                return;
                            }
                        }
                    }
                    //これからつける装備が-1されて消えてそこに元々つけていた装備を入れる場合
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
                            //他のところに元々つけていた装備を入れる時同じ装備がInventory内にない場合Nullに入れる
                            if (inventory.ItemCharas[inventoryUI.SelectedChara].ItemTypes[inventoryUI.SelectedItemType].Items[j].item.Base.GetKanjiName() == "未所持")
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