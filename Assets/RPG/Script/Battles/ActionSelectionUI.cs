using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionSelectionUI: MonoBehaviour
{
    //アクションUIの管理
    //たたかうorにげるのどちらを選択中かを把握して色を変える

    SelectableText[] selectableTexts;
    [SerializeField] TextMeshProUGUI CharaName;
    [SerializeField] GameObject CharaNamePanel;

    int selectedIndex = 0; //0:たたかう 1:にげる　を選択している

    public int SelectedIndex { get => selectedIndex;}

    public void Init()
    {
        //)自分の子要素で＜selectableText＞コンポーネントを持っているものを集める
        selectableTexts = GetComponentsInChildren<SelectableText>();
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex++;
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex--;
        }
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
        selectedIndex = Mathf.Clamp(selectedIndex, 0, 3);
    }

    public void Open(Battler battler)
    {
        gameObject.SetActive(true);
        CharaNamePanel.SetActive(true);
        CharaName.text = battler.Base.Name;
    }

    public void Close()
    {
        CharaNamePanel.SetActive(false);
        gameObject.SetActive(false);
    }

}
