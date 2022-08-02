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

    internal MenuState State { get => state; }

    public void HandleUpdate()
    {
        menuSelectionUI.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (menuSelectionUI.SelectedIndex == 0)
            {
                if (state == MenuState.SelectStart)
                {
                    statusUI.Open();
                    state = MenuState.Status;

                }
            }
            else if (menuSelectionUI.SelectedIndex == 1)
            {
                if (state == MenuState.SelectStart)
                {

                    InventorySelection();
                    OnInventorySelect();
                }
            }
            else if (menuSelectionUI.SelectedIndex == 2)
            {
                if (state == MenuState.SelectStart)
                {

                }

            }
            else if (menuSelectionUI.SelectedIndex == 3)
            {
                if (state == MenuState.SelectStart)
                {

                }

            }
            else if (menuSelectionUI.SelectedIndex == 4)
            {
                if (state == MenuState.SelectStart)
                {

                    menuSelectionUI.Close();
                    state = MenuState.End;
                    MenuSelectEnd();
                }

            }
        }
    }


    void MenuSelection()
    {
        state = MenuState.SelectStart;
        menuSelectionUI.Open();
        menuSelectionUI.Init();
    }

    void InventorySelection()
    {
        state = MenuState.InventorySelection;

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



