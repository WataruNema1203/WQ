using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSelectionUI: MonoBehaviour
{
    //アクションUIの管理
    //たたかうorにげるのどちらを選択中かを把握して色を変える

    SelectableText[] selectableTexts;

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
        selectedIndex = Mathf.Clamp(selectedIndex, 0, 2);
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

}
