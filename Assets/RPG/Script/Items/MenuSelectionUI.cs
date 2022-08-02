using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSelectionUI : MonoBehaviour
{
    //メニュUIの管理

    //選択肢をUIに反映
    //何をを選択中かを把握して色を変える
    SelectableText[] selectableTexts;


    int selectedIndex = 0; //0:たたかう 1:にげる　を選択している

    public int SelectedIndex { get => selectedIndex; }

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
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex--;
        }
        selectedIndex = Mathf.Clamp(selectedIndex, 0, 4);

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
