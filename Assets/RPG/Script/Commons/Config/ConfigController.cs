using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum ConfigState
{
    Select,
    PlayerNameChange,
    WordsSelect,
    Busy,
}
enum WordsState
{
    Hiragana,
    Katakana,
    English,
}

public class ConfigController : MonoBehaviour
{
    [SerializeField] PlayerController player;
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
    int selectedWords=0;
    int selectedIndex=0;
    int selectedWordType=0;



    internal ConfigState ConfigState { get => configState; }
    public int SelectedWords { get => selectedWords; }
    internal WordsState WordsState { get => wordsState; }

    public void ConfigMenuInit()
    {
        configState = ConfigState.Select;
        selectableTexts = configMenuPanel.GetComponentsInChildren<SelectableText>();
    }
    public void WordTypeInit()
    {
        configState = ConfigState.PlayerNameChange;
        selectableTexts = wordsPanel.GetComponentsInChildren<SelectableText>();
    }
    public void HiraInit()
    {
        wordsState = WordsState.Hiragana;
        textSlots = hiraPanel.GetComponentsInChildren<TextUI>();
        texts = hiraPanel.GetComponentsInChildren<Text>();
    }
    public void KanaInit()
    {
        wordsState = WordsState.Katakana;
        textSlots = kanaPanel.GetComponentsInChildren<TextUI>();
        texts = kanaPanel.GetComponentsInChildren<Text>();
    }
    public void EngInit()
    {
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
                else if (Input.GetKeyDown(KeyCode.X))
                {

                }

                break;
            case ConfigState.PlayerNameChange:
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (selectedWordType == 4)
                    {
                        selectedWordType = 0;
                    }
                    else
                    {
                        selectedWordType++;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (selectedWordType == 0)
                    {
                        selectedWordType = 4;
                    }
                    else
                    {
                        selectedWordType--;
                    }
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
                        case 3:
                            PlayerNameChengeClose();
                            break;
                        case 4:
                            StartCoroutine(SetName());
                            break;
                        default:
                            break;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.X))
                {
                    PlayerNameChengeClose();
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
                                selectedWords = 84;
                            }
                            else if (selectedWords==37||selectedWords==39||selectedWords==47||selectedWords==49||selectedWords==82||selectedWords==84)
                            {
                                selectedWords -= 2;
                            }
                            else if (selectedWords == 85)
                            {
                                selectedWords++;
                            }
                            else
                            {
                                selectedWords--;

                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.DownArrow))
                        {
                            if (selectedWords == 84)
                            {
                                selectedWords = 0;
                            }
                            else if (selectedWords == 35 || selectedWords == 37 || selectedWords == 45 || selectedWords == 47 || selectedWords == 80 || selectedWords == 82)
                            {
                                selectedWords += 2;

                            }
                            else if (selectedWords == 86)
                            {
                                selectedWords--;
                            }
                            else
                            {
                                selectedWords++;

                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            if (selectedWords == 85 || selectedWords == 86)
                            {
                                selectedWords -= 82;
                            }
                            else if (selectedWords == 31 || selectedWords == 33 || selectedWords == 41 || selectedWords == 43)
                            {
                                selectedWords += 10;
                            }
                            else if (selectedWords == 76)
                            {
                                selectedWords -= 75;
                            }
                            else if (selectedWords == 78)
                            {
                                selectedWords += 7;
                            }
                            else if (selectedWords == 84)
                            {
                                selectedWords += 2;
                            }
                            else if (selectedWords >= 80)
                            {
                                selectedWords -= 80;
                            }
                            else
                            {
                                selectedWords += 5;

                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            if (selectedWords == 85)
                            {
                                selectedWords -= 7;
                            }
                            else if (selectedWords == 86)
                            {
                                selectedWords -= 2;
                            }
                            else if (selectedWords == 3 || selectedWords == 4)
                            {
                                selectedWords += 82;
                            }
                            else if (selectedWords == 1 )
                            {
                                selectedWords += 75;
                            }
                            else if (selectedWords == 41 || selectedWords == 43 || selectedWords == 51 || selectedWords == 53)
                            {
                                selectedWords -= 10;
                            }
                            else if (selectedWords <= 4)
                            {
                                selectedWords += 80;
                            }
                            else
                            {
                                selectedWords -= 5;

                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.Z))
                        {
                            if (selectedWords == 86)
                            {
                                StartCoroutine(SetName());

                            }
                            else if (selectedWords == 85)
                            {
                                HiraClose();
                            }
                            else if (nameText1.text == "-")
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
                        else if (Input.GetKeyDown(KeyCode.Backspace))
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
                                selectedWords = 84;
                            }
                            else if (selectedWords == 85)
                            {
                                selectedWords++;
                            }
                            else
                            {
                                selectedWords--;

                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.DownArrow))
                        {
                            if (selectedWords == 84)
                            {
                                selectedWords = 0;
                            }
                            else if (selectedWords == 86)
                            {
                                selectedWords--;
                            }
                            else
                            {
                                selectedWords++;

                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            if (selectedWords == 85 || selectedWords == 86)
                            {
                                selectedWords -= 82;
                            }
                            else if (selectedWords == 83 || selectedWords == 84)
                            {
                                selectedWords += 2;
                            }
                            else if (selectedWords >= 80)
                            {
                                selectedWords -= 80;
                            }
                            else
                            {
                                selectedWords += 5;

                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            if (selectedWords == 85 || selectedWords == 86)
                            {
                                selectedWords -= 2;
                            }
                            else if (selectedWords == 3 || selectedWords == 4)
                            {
                                selectedWords += 82;
                            }
                            else if (selectedWords <= 4)
                            {
                                selectedWords += 80;
                            }
                            else
                            {
                                selectedWords -= 5;

                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.Z))
                        {
                            if (selectedWords == 86)
                            {
                                StartCoroutine(SetName());
                            }
                            else if (selectedWords == 85)
                            {
                                KanaClose();
                            }
                            else if (nameText1.text == "-")
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
                        else if (Input.GetKeyDown(KeyCode.Backspace))
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
                                selectedWords = 49;
                            }
                            else if (selectedWords == 50)
                            {
                                selectedWords = 51;
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
                                selectedWords = 0;
                            }
                            else if (selectedWords == 51)
                            {
                                selectedWords = 50;
                            }
                            else
                            {
                                selectedWords++;

                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            if (selectedWords == 50 || selectedWords == 51)
                            {
                                selectedWords -= 47;
                            }
                            else if (selectedWords == 48 || selectedWords == 49)
                            {
                                selectedWords += 2;
                            }
                            else if (selectedWords >= 45)
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
                            if (selectedWords == 50 || selectedWords == 51)
                            {
                                selectedWords -= 2;
                            }
                            else if (selectedWords == 3 || selectedWords == 4)
                            {
                                selectedWords += 47;
                            }
                            else if (selectedWords <= 4)
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
                            if (selectedWords == 51)
                            {
                                StartCoroutine(SetName());
                            }
                            else if (selectedWords == 50)
                            {
                                EngClose();
                            }
                            else if (nameText1.text == "-")
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
                        else if (Input.GetKeyDown(KeyCode.Backspace))
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
        selectedIndex = 0;
        configMenuPanel.SetActive(true);
        ConfigMenuInit();
    }
    public void ConfigClose()
    {
        configMenuPanel.SetActive(false);
    }

    public void PlayerNameChengeOpen()
    {
        selectedIndex = 0;
        wordsPanel.SetActive(true);
        hiraPanel.SetActive(true);
        playerNamePanel.SetActive(true);
        enterOrBackPanel.SetActive(true);
        WordTypeInit();
    }
    public void PlayerNameChengeClose()
    {
        wordsPanel.SetActive(false);
        hiraPanel.SetActive(false);
        kanaPanel.SetActive(false);
        engPanel.SetActive(false);
        playerNamePanel.SetActive(false);
        enterOrBackPanel.SetActive(false);
        ConfigOpen();
    }

    public void HiraOpen()
    {
        configState = ConfigState.WordsSelect;
        hiraPanel.SetActive(true);
        HiraInit();
    }
    public void KanaOpen()
    {
        configState = ConfigState.WordsSelect;
        kanaPanel.SetActive(true);
        KanaInit();
    }
    public void EngOpen()
    {
        configState = ConfigState.WordsSelect;
        engPanel.SetActive(true);
        EngInit();
    }
    public void HiraClose()
    {
        selectedWords = 0;
        PlayerNameChengeOpen();
    }
    public void KanaClose()
    {
        selectedWords = 0;
        PlayerNameChengeOpen();
    }
    public void EngClose()
    {
        selectedWords = 0;
        PlayerNameChengeOpen();
    }
    IEnumerator SetName()
    {
        configState = ConfigState.Busy;
        if (nameText5.text != "-")
        {
            yield return StartCoroutine(DialogManager.Instance.FieldTypeDialog($"y_•ƒz\n{player.Battlers[0].Base.Name}‚Ì–¼‘O‚ð{nameText1.text}{nameText2.text}{nameText3.text}{nameText4.text}{nameText5.text}‚É•Ï‚¦‚Ü‚µ‚½B\n‚ ‚È‚½‚É_‚Ì‚²‰ÁŒì‚ª‚ ‚è‚Ü‚·‚æ‚¤‚ÉB"));
            player.Battlers[0].Base.SetNeme($"{nameText1.text}{nameText2.text}{nameText3.text}{nameText4.text}{nameText5.text}");
        }
        else if (nameText4.text != "-")
        {
            yield return StartCoroutine(DialogManager.Instance.FieldTypeDialog($"y_•ƒz\n{player.Battlers[0].Base.Name}‚Ì–¼‘O‚ð{nameText1.text}{nameText2.text}{nameText3.text}{nameText4.text}‚É•Ï‚¦‚Ü‚µ‚½B\n‚ ‚È‚½‚É_‚Ì‚²‰ÁŒì‚ª‚ ‚è‚Ü‚·‚æ‚¤‚ÉB"));
            player.Battlers[0].Base.SetNeme($"{nameText1.text}{nameText2.text}{nameText3.text}{nameText4.text}");
        }
        else if (nameText3.text != "-")
        {
            yield return StartCoroutine(DialogManager.Instance.FieldTypeDialog($"y_•ƒz\n{player.Battlers[0].Base.Name}‚Ì–¼‘O‚ð{nameText1.text}{nameText2.text}{nameText3.text}‚É•Ï‚¦‚Ü‚µ‚½B\n‚ ‚È‚½‚É_‚Ì‚²‰ÁŒì‚ª‚ ‚è‚Ü‚·‚æ‚¤‚ÉB"));
            player.Battlers[0].Base.SetNeme($"{nameText1.text}{nameText2.text}{nameText3.text}");
        }
        else if (nameText2.text != "-")
        {
            yield return StartCoroutine(DialogManager.Instance.FieldTypeDialog($"y_•ƒz\n{player.Battlers[0].Base.Name}‚Ì–¼‘O‚ð{nameText1.text}{nameText2.text}‚É•Ï‚¦‚Ü‚µ‚½B\n‚ ‚È‚½‚É_‚Ì‚²‰ÁŒì‚ª‚ ‚è‚Ü‚·‚æ‚¤‚ÉB"));
            player.Battlers[0].Base.SetNeme($"{nameText1.text}{nameText2.text}");
        }
        else if (nameText1.text != "-")
        {
            yield return StartCoroutine(DialogManager.Instance.FieldTypeDialog($"y_•ƒz\n{player.Battlers[0].Base.Name}‚Ì–¼‘O‚ð{nameText1.text}‚É•Ï‚¦‚Ü‚µ‚½B\n‚ ‚È‚½‚É_‚Ì‚²‰ÁŒì‚ª‚ ‚è‚Ü‚·‚æ‚¤‚ÉB"));
            player.Battlers[0].Base.SetNeme($"{nameText1.text}");
        }
        else
        {
            yield return StartCoroutine(DialogManager.Instance.FieldTypeDialog($"y_•ƒz\n‚Ü‚¾”Y‚ñ‚Å‚¨‚ç‚ê‚Ä‚¢‚é‚æ‚¤‚Å‚·‚ËB"));
        }
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        nameText1.text = "-";
        nameText2.text = "-";
        nameText3.text = "-";
        nameText4.text = "-";
        nameText5.text = "-";
        DialogManager.Instance.Close();
        PlayerNameChengeClose();
        configState = ConfigState.Select;
    }
}
