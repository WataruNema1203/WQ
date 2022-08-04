using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectableText : MonoBehaviour
{
    //Textの色をかえる
    //選択中なら黄色：そうでないなら白
    protected Text text;

    private void Awake()
    {
        this.text = GetComponent<Text>();
    }

    public void SetText(string text)
    {
        if (this.text == null)
        {
            this.text = GetComponent<Text>();
        }
        this.text.text = text;
    }

    //選択中なら色を変える
    public void SetSelectedColor(bool selected)
    {
        if (this.text ==null)
        {
            this.text = GetComponent<Text>();
        }
        if (selected)
        {
            text.color = Color.yellow;
            text.fontStyle = FontStyle.Bold;
        }
        else
        {
            text.color = Color.white;
            text.fontStyle = FontStyle.Normal;
        }
    } 
}
