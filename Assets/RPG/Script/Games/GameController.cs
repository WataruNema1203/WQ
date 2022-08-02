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
    [SerializeField] StatusUI statusUI;
    [SerializeField] ShopController shop;
    [SerializeField] ShopNPCController shopNPC;
    [SerializeField] ItemController item;
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
        inventoryUI.OnUsedItemKeep += UsedItemKeep;
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
    void UsedItemKeep()
    {
        state = GameState.OpenInventory;
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
            case GameState.OpenMenu:
                HandleUpdateOpenMenu();
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
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (item.State == MenuState.Status)
            {
                statusUI.Close();
                item.Open();
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
                    shop.SellSelectOpen();
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
            else if (shop.State == ShopState.Buy)
            {
                StartCoroutine(shop.Buy());
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
                if (inventory.ItemCharas[0].ItemTypes[1].Items[i].item.Base.GetKanjiName() == "未所持")
                {
                    inventory.ItemCharas[0].ItemTypes[1].Items[i].item = item;
                    inventory.ItemCharas[0].ItemTypes[1].Items[i].possession++;
                    return;
                }
            }
            else if (item.Base.GetItemType() == Type.Valuables)
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
                if (inventory.ItemCharas[1].ItemTypes[1].Items[i].item.Base.GetKanjiName() == "未所持")
                {
                    inventory.ItemCharas[1].ItemTypes[1].Items[i].item = item;
                    inventory.ItemCharas[1].ItemTypes[1].Items[i].possession++;
                    return;
                }
            }
            else if (item.Base.GetItemType() == Type.Valuables)
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
                if (inventory.ItemCharas[0].ItemTypes[1].Items[i].item.Base.GetKanjiName() == "未所持")
                {
                    inventory.ItemCharas[0].ItemTypes[1].Items[i].item = item;
                    inventory.ItemCharas[0].ItemTypes[1].Items[i].possession++;
                    return;
                }
            }
            else if (item.Base.GetItemType() == Type.Valuables)
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
        if (sumItem <= 0)
        {
            shop.BuyInit();
            yield break;
        }
        if (player.Battlers[0].Gold >= item.Base.GetGold() && player.Battlers[0].Gold >= item.Base.GetGold() * sumItem)
        {
            player.Battlers[0].Gold -= item.Base.GetGold() * sumItem;
            for (int i = 0; i < inventory.ItemCharas[0].ItemTypes[0].Items.Count; i++)
            {
                if (item.Base.GetItemType() == Type.Weapon || item.Base.GetItemType() == Type.Armor || item.Base.GetItemType() == Type.Accessory)
                {
                    if (inventory.ItemCharas[0].ItemTypes[1].Items[i].item.Base.GetHiraganaName() == item.Base.GetHiraganaName())
                    {
                        inventory.ItemCharas[0].ItemTypes[1].Items[i].possession += sumItem;
                        shop.HasGoldText.text = $"所持金：{player.Battlers[0].Gold}G";
                        shop.BuyClose();
                        yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{item.Base.GetKanjiName()}を{sumItem}個だね\n他に何か要るもんはあるかい？"));
                        shop.BuySelectOpen();
                        yield break;
                    }
                }
                else if (item.Base.GetItemType() == Type.Valuables)
                {
                    if (inventory.ItemCharas[0].ItemTypes[2].Items[i].item.Base.GetHiraganaName() == item.Base.GetHiraganaName())
                    {
                        inventory.ItemCharas[0].ItemTypes[2].Items[i].possession += sumItem;
                        shop.HasGoldText.text = $"所持金：{player.Battlers[0].Gold}G";
                        shop.BuyClose();
                        yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{item.Base.GetKanjiName()}を{sumItem}個だね\n他に何か要るもんはあるかい？"));
                        shop.BuySelectOpen();
                        yield break;
                    }
                }
                else
                {
                    if (inventory.ItemCharas[0].ItemTypes[0].Items[i].item.Base.GetHiraganaName() == item.Base.GetHiraganaName())
                    {
                        inventory.ItemCharas[0].ItemTypes[0].Items[i].possession += sumItem;
                        shop.HasGoldText.text = $"所持金：{player.Battlers[0].Gold}G";
                        shop.BuyClose();
                        yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{item.Base.GetKanjiName()}を{sumItem}個だね\n他に何か要るもんはあるかい？"));
                        shop.BuySelectOpen();
                        yield break;
                    }
                }
            }
            for (int i = 0; i < inventory.ItemCharas[0].ItemTypes[0].Items.Count; i++)
            {
                if (item.Base.GetItemType() == Type.Weapon || item.Base.GetItemType() == Type.Armor || item.Base.GetItemType() == Type.Accessory)
                {
                    if (inventory.ItemCharas[0].ItemTypes[1].Items[i].item.Base.GetKanjiName() == "未所持")
                    {
                        inventory.ItemCharas[0].ItemTypes[1].Items[i].item = item;
                        inventory.ItemCharas[0].ItemTypes[1].Items[i].possession += sumItem;
                        shop.HasGoldText.text = $"所持金：{player.Battlers[0].Gold}G";
                        shop.BuyClose();
                        yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{item.Base.GetKanjiName()}を{sumItem}個だね\n他に何か要るもんはあるかい？"));
                        shop.BuySelectOpen();
                        yield break;
                    }
                }
                else if (item.Base.GetItemType() == Type.Valuables)
                {
                    if (inventory.ItemCharas[0].ItemTypes[2].Items[i].item.Base.GetKanjiName() == "未所持")
                    {
                        inventory.ItemCharas[0].ItemTypes[2].Items[i].item = item;
                        inventory.ItemCharas[0].ItemTypes[2].Items[i].possession += sumItem;
                        shop.HasGoldText.text = $"所持金：{player.Battlers[0].Gold}G";
                        shop.BuyClose();
                        yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{item.Base.GetKanjiName()}を{sumItem}個だね\n他に何か要るもんはあるかい？"));
                        shop.BuySelectOpen();
                        yield break;
                    }
                }
                else
                {
                    if (inventory.ItemCharas[0].ItemTypes[2].Items[i].item.Base.GetKanjiName() == "未所持")
                    {
                        inventory.ItemCharas[0].ItemTypes[2].Items[i].item = item;
                        inventory.ItemCharas[0].ItemTypes[2].Items[i].possession += sumItem;
                        shop.HasGoldText.text = $"所持金：{player.Battlers[0].Gold}G";
                        shop.BuyClose();
                        yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{item.Base.GetKanjiName()}を{sumItem}個だね\n他に何か要るもんはあるかい？"));
                        shop.BuySelectOpen();
                        yield break;
                    }
                }
            }
        }
        else
        {
            shop.BuyClose();
            yield return StartCoroutine(DialogManager.Instance.TypeDialog($"うん？ゴールドが足りないようだよ"));
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
                    shop.HasGoldText.text = $"所持金：{player.Battlers[0].Gold}G";
                    if (inventory.ItemCharas[shop.SelectedChara].ItemTypes[shop.SelectedItemType].Items[i].possession == 0)
                    {
                        inventory.ItemCharas[shop.SelectedChara].ItemTypes[shop.SelectedItemType].Items[i].item = inventory.NoneItem;
                    }
                    shop.SellClose();
                    shop.SellSelectOpen();
                    yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{item.Base.GetKanjiName()}を{sumItem}個だね\n他に何か売るもんはあるかい？"));
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
        else if (item.Base.GetItemType() == Type.Armor)
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
        else if (item.Base.GetItemType() == Type.Accessory)
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