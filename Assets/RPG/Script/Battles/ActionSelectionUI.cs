using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionSelectionUI: MonoBehaviour
{
    //�A�N�V����UI�̊Ǘ�
    //��������or�ɂ���̂ǂ����I�𒆂���c�����ĐF��ς���

    SelectableText[] selectableTexts;
    [SerializeField] TextMeshProUGUI CharaName;
    [SerializeField] GameObject CharaNamePanel;

    int selectedIndex = 0; //0:�������� 1:�ɂ���@��I�����Ă���

    public int SelectedIndex { get => selectedIndex;}

    public void Init()
    {
        //)�����̎q�v�f�Ł�selectableText���R���|�[�l���g�������Ă�����̂��W�߂�
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
