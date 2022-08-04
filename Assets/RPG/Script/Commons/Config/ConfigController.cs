using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum ConfigState
{
    Select,
    PlayerNameChange,
    WordsSelect,
}
enum WordsState
{
    Hiragana,
    Katakana,
    English,
}

public class ConfigController : MonoBehaviour
{
    [SerializeField] GameObject configMenuPanel;
    [SerializeField] GameObject hiraPanel;
    [SerializeField] GameObject kanaPanel;
    [SerializeField] GameObject engPanel;
    [SerializeField] GameObject wordsPanel;
    [SerializeField] GameObject playerNamePanel;
    [SerializeField] GameObject enterOrBackPanel;
    [SerializeField] Text nameText1;
    [SerializeField] Text nameText2;
    [SerializeField] Text nameText3;
    [SerializeField] Text nameText4;
    [SerializeField] Text nameText5;
    ConfigState configState;
    WordsState wordsState;
    SelectableText[] selectableTexts;
    TextUI[] textSlots;
    Text[] texts;
    int selectedWords;
    int selectedIndex;
    int selectedWordType;



    internal ConfigState ConfigState { get => configState; }
    public int SelectedWords { get => selectedWords; }
    internal WordsState WordsState { get => wordsState; }

    public void ConfigMenuInit()
    {
        configState = ConfigState.Select;
        selectedIndex = 0;
        selectableTexts = configMenuPanel.GetComponentsInChildren<SelectableText>();
    }
    public void WordTypeInit()
    {
        configState = ConfigState.PlayerNameChange;
        selectedWordType = 0;
        selectableTexts = wordsPanel.GetComponentsInChildren<SelectableText>();
    }
    public void HiraInit()
    {
        selectedWords = 0;
        wordsState = WordsState.Hiragana;
        textSlots = hiraPanel.GetComponentsInChildren<TextUI>();
        texts = hiraPanel.GetComponentsInChildren<Text>();
    }
    public void KanaInit()
    {
        selectedWords = 0;
        wordsState = WordsState.Katakana;
        textSlots = kanaPanel.GetComponentsInChildren<TextUI>();
        texts = kanaPanel.GetComponentsInChildren<Text>();
    }
    public void EngInit()
    {
        selectedWords = 0;
        wordsState = WordsState.English;
        textSlots = engPanel.GetComponentsInChildren<TextUI>();
        texts = engPanel.GetComponentsInChildren<Text>();
    }




    public void HandleUpdate()
    {
        switch (configState)
        {
            case ConfigState.Select:
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
                    PlayerNameChengeOpen();
                }
                break;
            case ConfigState.PlayerNameChange:
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    selectedWordType++;
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    selectedWordType--;
                }
                selectedWordType = Mathf.Clamp(selectedWordType, 0, selectableTexts.Length - 1);

                for (int i = 0; i < selectableTexts.Length; i++)
                {
                    if (selectedWordType == i)
                    {
                        selectableTexts[i].SetSelectedColor(true);
                    }
                    else
                    {
                        selectableTexts[i].SetSelectedColor(false);
                    }
                }
                switch (selectedWordType)
                {
                    case 0:
                        hiraPanel.SetActive(true);
                        kanaPanel.SetActive(false);
                        engPanel.SetActive(false);
                        break;
                    case 1:
                        hiraPanel.SetActive(false);
                        kanaPanel.SetActive(true);
                        engPanel.SetActive(false);
                        break;
                    case 2:
                        hiraPanel.SetActive(false);
                        kanaPanel.SetActive(false);
                        engPanel.SetActive(true);
                        break;
                }

                if (Input.GetKeyDown(KeyCode.Z))
                {
                    configState = ConfigState.WordsSelect;
                    switch (selectedWordType)
                    {
                        case 0:
                            HiraOpen();
                            break;
                        case 1:
                            KanaOpen();
                            break;
                        case 2:
                            EngOpen();
                            break;
                    }
                }

                break;
            case ConfigState.WordsSelect:
                switch (wordsState)
                {
                    case WordsState.Hiragana:
                        if (Input.GetKeyDown(KeyCode.UpArrow))
                        {
                            if (selectedWords == 0)
                            {
                                selectedWords += 49;
                            }
                            else
                            {
                                selectedWords--;

                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.DownArrow))
                        {
                            if (selectedWords == 49)
                            {
                                selectedWords -= 49;
                            }
                            else
                            {
                                selectedWords++;

                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            if (selectedWords >= 45)
                            {
                                selectedWords -= 45;
                            }
                            else
                            {
                                selectedWords += 5;

                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            if (selectedWords <= 4)
                            {
                                selectedWords += 45;
                            }
                            else
                            {
                                selectedWords -= 5;

                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.Z))
                        {
                            if (nameText1.text == "-")
                            {
                                nameText1.text = texts[selectedWords].text;

                            }
                            else
                            {
                                if (nameText2.text == "-")
                                {
                                    nameText2.text = texts[selectedWords].text;

                                }
                                else
                                {
                                    if (nameText3.text == "-")
                                    {
                                        nameText3.text = texts[selectedWords].text;

                                    }
                                    else
                                    {
                                        if (nameText4.text == "-")
                                        {
                                            nameText4.text = texts[selectedWords].text;

                                        }
                                        else
                                        {
                                            if (nameText5.text == "-")
                                            {
                                                nameText5.text = texts[selectedWords].text;

                                            }
                                            else
                                            {

                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.C))
                        {
                            if (nameText5.text != "-")
                            {
                                nameText5.text = "-";
                            }
                            else
                            {
                                if (nameText4.text != "-")
                                {
                                    nameText4.text = "-";
                                }
                                else
                                {
                                    if (nameText3.text != "-")
                                    {
                                        nameText3.text = "-";
                                    }
                                    else
                                    {
                                        if (nameText2.text != "-")
                                        {
                                            nameText2.text = "-";
                                        }
                                        else
                                        {
                                            if (nameText1.text != "-")
                                            {
                                                nameText1.text = "-";
                                            }
                                            else
                                            {

                                            }

                                        }
                                    }
                                }
                            }
                        }

                        else if (Input.GetKeyDown(KeyCode.X))
                        {
                            HiraClose();
                        }


                        selectedWords = Mathf.Clamp(selectedWords, 0, textSlots.Length - 1);

                        for (int i = 0; i < textSlots.Length; i++)
                        {
                            if (selectedWords == i)
                            {
                                textSlots[i].SetSelectedColor(true);
                            }
                            else
                            {
                                textSlots[i].SetSelectedColor(false);
                            }
                        }

                        break;
                    case WordsState.Katakana:
                        if (Input.GetKeyDown(KeyCode.UpArrow))
                        {
                            if (selectedWords == 0)
                            {
                                selectedWords += 49;
                            }
                            else
                            {
                                selectedWords--;

                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.DownArrow))
                        {
                            if (selectedWords == 49)
                            {
                                selectedWords -= 49;
                            }
                            else
                            {
                                selectedWords++;

                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            if (selectedWords >= 45)
                            {
                                selectedWords -= 45;
                            }
                            else
                            {
                                selectedWords += 5;

                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            if (selectedWords <= 4)
                            {
                                selectedWords += 45;
                            }
                            else
                            {
                                selectedWords -= 5;

                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.Z))
                        {
                            if (nameText1.text == "-")
                            {
                                nameText1.text = texts[selectedWords].text;

                            }
                            else
                            {
                                if (nameText2.text == "-")
                                {
                                    nameText2.text = texts[selectedWords].text;

                                }
                                else
                                {
                                    if (nameText3.text == "-")
                                    {
                                        nameText3.text = texts[selectedWords].text;

                                    }
                                    else
                                    {
                                        if (nameText4.text == "-")
                                        {
                                            nameText4.text = texts[selectedWords].text;

                                        }
                                        else
                                        {
                                            if (nameText5.text == "-")
                                            {
                                                nameText5.text = texts[selectedWords].text;

                                            }
                                            else
                                            {

                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.C))
                        {
                            if (nameText5.text != "-")
                            {
                                nameText5.text = "-";
                            }
                            else
                            {
                                if (nameText4.text != "-")
                                {
                                    nameText4.text = "-";
                                }
                                else
                                {
                                    if (nameText3.text != "-")
                                    {
                                        nameText3.text = "-";
                                    }
                                    else
                                    {
                                        if (nameText2.text != "-")
                                        {
                                            nameText2.text = "-";
                                        }
                                        else
                                        {
                                            if (nameText1.text != "-")
                                            {
                                                nameText1.text = "-";
                                            }
                                            else
                                            {

                                            }

                                        }
                                    }
                                }
                            }
                        }

                        else if (Input.GetKeyDown(KeyCode.X))
                        {
                            KanaClose();
                        }


                        selectedWords = Mathf.Clamp(selectedWords, 0, textSlots.Length - 1);

                        for (int i = 0; i < textSlots.Length; i++)
                        {
                            if (selectedWords == i)
                            {
                                textSlots[i].SetSelectedColor(true);
                            }
                            else
                            {
                                textSlots[i].SetSelectedColor(false);
                            }
                        }

                        break;
                    case WordsState.English:
                        if (Input.GetKeyDown(KeyCode.UpArrow))
                        {
                            if (selectedWords == 0)
                            {
                                selectedWords += 49;
                            }
                            else
                            {
                                selectedWords--;

                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.DownArrow))
                        {
                            if (selectedWords == 49)
                            {
                                selectedWords -= 49;
                            }
                            else
                            {
                                selectedWords++;

                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            if (selectedWords >= 45)
                            {
                                selectedWords -= 45;
                            }
                            else
                            {
                                selectedWords += 5;

                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            if (selectedWords <= 4)
                            {
                                selectedWords += 45;
                            }
                            else
                            {
                                selectedWords -= 5;

                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.Z))
                        {
                            if (nameText1.text == "-")
                            {
                                nameText1.text = texts[selectedWords].text;

                            }
                            else
                            {
                                if (nameText2.text == "-")
                                {
                                    nameText2.text = texts[selectedWords].text;

                                }
                                else
                                {
                                    if (nameText3.text == "-")
                                    {
                                        nameText3.text = texts[selectedWords].text;

                                    }
                                    else
                                    {
                                        if (nameText4.text == "-")
                                        {
                                            nameText4.text = texts[selectedWords].text;

                                        }
                                        else
                                        {
                                            if (nameText5.text == "-")
                                            {
                                                nameText5.text = texts[selectedWords].text;

                                            }
                                            else
                                            {

                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.C))
                        {
                            if (nameText5.text != "-")
                            {
                                nameText5.text = "-";
                            }
                            else
                            {
                                if (nameText4.text != "-")
                                {
                                    nameText4.text = "-";
                                }
                                else
                                {
                                    if (nameText3.text != "-")
                                    {
                                        nameText3.text = "-";
                                    }
                                    else
                                    {
                                        if (nameText2.text != "-")
                                        {
                                            nameText2.text = "-";
                                        }
                                        else
                                        {
                                            if (nameText1.text != "-")
                                            {
                                                nameText1.text = "-";
                                            }
                                            else
                                            {

                                            }

                                        }
                                    }
                                }
                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.X))
                        {
                            EngClose();
                        }

                        selectedWords = Mathf.Clamp(selectedWords, 0, textSlots.Length - 1);

                        for (int i = 0; i < textSlots.Length; i++)
                        {
                            if (selectedWords == i)
                            {
                                textSlots[i].SetSelectedColor(true);
                            }
                            else
                            {
                                textSlots[i].SetSelectedColor(false);
                            }
                        }

                        break;
                }

                break;
            default:
                break;
        }
    }

    public void ConfigOpen()
    {
        configMenuPanel.SetActive(true);
        ConfigMenuInit();
    }

    public void PlayerNameChengeOpen()
    {
        wordsPanel.SetActive(true);
        hiraPanel.SetActive(true);
        playerNamePanel.SetActive(true);
        enterOrBackPanel.SetActive(true);
        WordTypeInit();
    }

    public void HiraOpen()
    {
        hiraPanel.SetActive(true);
        HiraInit();
    }
    public void KanaOpen()
    {
        kanaPanel.SetActive(true);
        KanaInit();
    }
    public void EngOpen()
    {
        engPanel.SetActive(true);
        EngInit();
    }
    public void HiraClose()
    {
        PlayerNameChengeOpen();
    }
    public void KanaClose()
    {
        PlayerNameChengeOpen();
    }
    public void EngClose()
    {
        PlayerNameChengeOpen();
    }

}
