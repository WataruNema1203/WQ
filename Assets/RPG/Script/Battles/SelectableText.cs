using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SelectableText : MonoBehaviour
{
    //Textの色をかえる
    //選択中なら黄色：そうでないなら白
    protected TextMeshProUGUI text;

    private void Awake()
    {
        this.text = GetComponent<TextMeshProUGUI>();
    }

    public void SetText(string text)
    {
        if (this.text == null)
        {
            this.text = GetComponent<TextMeshProUGUI>();
        }
        this.text.text = text;
    }

    public string GetText()
    {
        return this.text.text;
    }

    //選択中なら色を変える
    public void SetSelectedColor(bool selected)
    {
        if (this.text == null)
        {
            this.text = GetComponent<TextMeshProUGUI>();
        }
        if (selected)
        {
            text.color = Color.yellow;
            text.fontStyle = FontStyles.Bold;
        }
        else
        {
            text.color = Color.white;
            text.fontStyle = FontStyles.Normal;
        }
    }
}
