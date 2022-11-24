using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

enum ConfigStat
{
    Select,
    NameChange,
    endGame,
}

public class ConfigController : MonoBehaviour
{
    ConfigStat stat;
    [SerializeField] NameChangeController nameChange;
    [SerializeField] EndGameScript endGame;
    [SerializeField] GameObject configMenuPanel;
    SelectableText[] selectableTexts;
    int selectedIndex;
    public UnityAction OnConfigClose;


    private void Start()
    {
        nameChange.OnConfigOpen += ConfigOpen;
        endGame.OnConfimationOff += ConfigOpen;
    }

    public void ConfigMenuInit()
    {
        selectableTexts = configMenuPanel.GetComponentsInChildren<SelectableText>();
    }

    public void HandleUpdate()
    {
        switch (stat)
        {
            case ConfigStat.Select:
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    selectedIndex++;
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    selectedIndex--;
                }
                selectedIndex = Mathf.Clamp(selectedIndex, 0, selectableTexts.Length - 1);

                for (int i = 0; i < selectableTexts.Length; i++)
                {
                    if (selectedIndex == i)
                    {
                        selectableTexts[i].SetSelectedColor(true);
                    }
                    else
                    {
                        selectableTexts[i].SetSelectedColor(false);
                    }
                }
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    if (selectedIndex == 0)
                    {
                        stat = ConfigStat.NameChange;
                        nameChange.CharacterSelectOpen();
                    }
                    else if (selectedIndex == 1)
                    {
                        stat = ConfigStat.endGame;
                        endGame.confirmation_On();
                    }
                }
                else if (Input.GetKeyDown(KeyCode.X))
                {
                    OnConfigClose.Invoke();
                }
                break;
            case ConfigStat.NameChange:
                nameChange.HandleUpdate();
                break;
            case ConfigStat.endGame:
                endGame.HandleUpdate();
                break;
        }
    }

    public void ConfigOpen()
    {
        stat = ConfigStat.Select;
        selectedIndex = 0;
        configMenuPanel.SetActive(true);
        ConfigMenuInit();
    }

    public void ConfigClose()
    {
        configMenuPanel.SetActive(false);
    }

}
