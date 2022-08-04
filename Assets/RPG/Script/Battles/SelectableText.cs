using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectableText : MonoBehaviour
{
    //Text�̐F��������
    //�I�𒆂Ȃ物�F�F�����łȂ��Ȃ甒
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

    //�I�𒆂Ȃ�F��ς���
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
