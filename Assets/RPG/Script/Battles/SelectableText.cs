using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SelectableText : MonoBehaviour
{
    //Text�̐F��������
    //�I�𒆂Ȃ物�F�F�����łȂ��Ȃ甒
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

    //�I�𒆂Ȃ�F��ς���
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
