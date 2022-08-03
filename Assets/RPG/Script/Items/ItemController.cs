using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

enum MenuState
{
    SelectStart,
    Status,
    InventorySelection,
    SkillSelection,
    Config,
    End,
}

public class ItemController : MonoBehaviour
{
    [SerializeField] MenuSelectionUI menuSelectionUI;
    [SerializeField] StatusUI statusUI;
    [SerializeField] GameObject ItemMenu;

    MenuState state;

    public UnityAction OnMenuSelect;
    public UnityAction MenuSelectEnd;
    public UnityAction OnInventorySelect;

    public MenuSelectionUI MenuSelectionUI { get => menuSelectionUI; }
    public StatusUI StatusUI { get => statusUI;}
    public GameObject ItemMenu1 { get => ItemMenu;}
    internal MenuState State { get => state; set => state = value; }

    public void HandleUpdate()
    {
        menuSelectionUI.HandleUpdate();

    }


    public void MenuSelection()
    {
        state = MenuState.SelectStart;
        menuSelectionUI.Open();
        menuSelectionUI.Init();
    }

    public void InventorySelection()
    {
        state = MenuState.InventorySelection;

    }
    public void SkillSelection()
    {
        state = MenuState.SkillSelection;

    }

    public void Open()
    {
        ItemMenu.SetActive(true);
        MenuSelection();

    }

    public void Close()
    {
        ItemMenu.SetActive(false);
    }
}



