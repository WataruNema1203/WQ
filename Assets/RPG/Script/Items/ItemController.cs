using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemController : MonoBehaviour
{
    [SerializeField] MenuSelectionUI menuSelectionUI;
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] GameObject ItemMenu;



    enum PatternState
    {
        SelectStart,
        InventorySelection,
        SkillSelection,
        Config,
        End,
    }


    PatternState state;

    public UnityAction OnMenuSelect;
    public UnityAction MenuSelectEnd;
    public UnityAction OnInventorySelect;



    public void HandleUpdate()
    {
        switch (state)
        {

            case PatternState.SelectStart:
                HandleMenuSelection();
                break;
            case PatternState.InventorySelection:
                
                break;
            case PatternState.SkillSelection:
                HandleMenuSelection();
                break;
            case PatternState.Config:
                break;
            case PatternState.End:
                break;
        }
    }


    void MenuSelection()
    {
        state = PatternState.SelectStart;
        menuSelectionUI.Open();
        menuSelectionUI.Init();
    }

    void InventorySelection()
    {
        state = PatternState.InventorySelection;

    }

    void HandleMenuSelection()
    {

        menuSelectionUI.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (menuSelectionUI.SelectedIndex == 0)
            {
                InventorySelection();
                OnInventorySelect();
            }
            else if (menuSelectionUI.SelectedIndex == 1)
            {
            }
            else if (menuSelectionUI.SelectedIndex == 2)
            {
            }
            else if (menuSelectionUI.SelectedIndex == 3)
            {
                menuSelectionUI.Close();
                state = PatternState.End;
                MenuSelectEnd();
            }
        }
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



