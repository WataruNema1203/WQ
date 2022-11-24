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

    MenuState state;

    public UnityAction OnMenuSelect;
    public UnityAction MenuSelectEnd;
    public UnityAction OnInventorySelect;

    public MenuSelectionUI MenuSelectionUI { get => menuSelectionUI; }
    public StatusUI StatusUI { get => statusUI;}
     internal MenuState State { get => state; set => state = value; }

    public void HandleUpdate()
    {
        menuSelectionUI.HandleUpdate();

    }


    public void MenuSelection()
    {
        state = MenuState.SelectStart;
        menuSelectionUI.gameObject.SetActive(true);
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
    public void ConfigSelection()
    {
        state = MenuState.Config;

    }

    public void Open()
    {
        MenuSelection();

    }

    public void Close()
    {
        menuSelectionUI.gameObject.SetActive(false);
    }
}



