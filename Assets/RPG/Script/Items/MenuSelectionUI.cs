using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSelectionUI : MonoBehaviour
{
    //���j��UI�̊Ǘ�

    //�I������UI�ɔ��f
    //������I�𒆂���c�����ĐF��ς���
    SelectableText[] selectableTexts;


    int selectedIndex = 0; //0:�������� 1:�ɂ���@��I�����Ă���

    public int SelectedIndex { get => selectedIndex; }

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
