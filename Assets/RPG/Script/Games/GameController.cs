using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


enum GameState
{
    FreeRoam,
    Battle,
    OpenMenu,
    OpenInventory,
    ShowDialog,
    Busy,
    Ending,
}

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] StatusUI statusUI;
    [SerializeField] ItemController item;
    [SerializeField] GameObject menuBar;
    [SerializeField] GameState state;
    bool IsBossBattle;



    private void Start()
    {
        player.OnEncounts += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
        item.OnMenuSelect += StartSelect;
        item.MenuSelectEnd += EndSelect;
        item.OnInventorySelect += UsedItem;
        inventoryUI.OnUsedItem += UsedItem;
        DialogManager.Instance.OnShowDialog += ShowDialog;
        DialogManager.Instance.OnCloseDialog += CloseDialog;
    }

    void UsedItem()
    {
        state = GameState.OpenInventory;
        inventoryUI.Open();
        Debug.Log("インベントリー操作開始");
        item.Close();
        statusUI.Close();

    }
    void ShowDialog()
    {
        state = GameState.ShowDialog;
    }

    void CloseDialog()
    {
        if (state == GameState.ShowDialog)
        {
            state = GameState.FreeRoam;
        }
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
                item.HandleUpdate();
                break;
            case GameState.ShowDialog:
                DialogManager.Instance.HandleUpdate();
                break;
        }

    }

    void HandleUpdateFreeRoam()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            statusUI.Open();
            inventoryUI.gameObject.SetActive(true);
            inventoryUI.Open();

        }
        else
        {
            player.HandleUpdate();
        }
    }

    void HandleUpdateInventory()
    {
        inventoryUI.HandleUpdateItemSelection();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            state = GameState.Busy;
            StartCoroutine(inventoryUI.Use());
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            state = GameState.OpenMenu;
            statusUI.Open();
            item.Open();
            inventoryUI.Close();
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
        statusUI.gameObject.SetActive(false);
        battleSystem.gameObject.SetActive(false);
        menuBar.gameObject.SetActive(true);
        player.gameObject.SetActive(true);
        if (IsBossBattle)
        {
        }
        else
        {
            state = GameState.FreeRoam;
            DialogManager.Instance.Close();
        }
    }

    public void StartSelect()
    {
        state = GameState.OpenMenu;
        statusUI.Open();
        Debug.Log("メニュー操作開始");
        item.Open();
        menuBar.SetActive(false);

    }
    public void EndSelect()
    {
        state = GameState.FreeRoam;
        statusUI.Close();
        Debug.Log("メニュー操作終了");
        item.Close();
        menuBar.SetActive(true);
        player.StartPlayer();
    }
}
