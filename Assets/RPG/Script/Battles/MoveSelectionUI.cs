using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSelectionUI : MonoBehaviour
{
    //技UIの管理
    //技の多様化：回復技
    //レベルアップ時に技を覚える
    //経験値＆レベルアップ実装

    //使える技をUIに反映
    //使える技の数だけTextコマンドを生成=>Prefabを生成
    [SerializeField] SelectableText moveTextPrefab;
    //技の数だけUIが伸びるようにする
    [SerializeField] RectTransform moveParent;

    //たたかうorにげるのどちらを選択中かを把握して色を変える
    readonly List<SelectableText> slots = new List<SelectableText>();

    const int INIT_HIGHT = 50;
    const int RANGE = 30;


    int selectedIndex = 0; //0:たたかう 1:にげる　を選択している

    public int SelectedIndex { get => selectedIndex; }


    public void Init(List<Move> moves)
    {
        //)自分の子要素で＜selectableText＞コンポーネントを持っているものを集める
        SetMovesUISize(moves);
        //selectableTexts = GetComponentsInChildren<SelectableText>();
    }

    void SetMovesUISize(List<Move> moves)
    {
        Vector2 uiSize = moveParent.sizeDelta;
        uiSize.y = 30 + 30 * moves.Count;
        moveParent.sizeDelta = uiSize;

        for (int i = 0; i < moves.Count; i++)
        {
            SelectableText moveText = Instantiate(moveTextPrefab, moveParent);
            moveText.SetText(moves[i].Base.Name);
            slots.Add(moveText);
        }
    }

    public void SetText(List<Move> moves)
    {
        slots.Clear();
        SelectableText[] tmp = moveParent.GetComponentsInChildren<SelectableText>();
        foreach (SelectableText child in tmp)
        {
            DestroyImmediate(child.gameObject);
        }
        Vector2 preSize = moveParent.sizeDelta;
        preSize.y = INIT_HIGHT + RANGE * moves.Count;
        moveParent.sizeDelta = preSize;
        foreach (var move in moves)
        {
            SelectableText selectableText = Instantiate(moveTextPrefab, moveParent);
            slots.Add(selectableText);
            selectableText.SetText(move.Base.Name);
        }
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
        selectedIndex = Mathf.Clamp(selectedIndex, 0, slots.Count-1);

        for (int i = 0; i < slots.Count; i++)
        {
            if (selectedIndex == i)
            {
                slots[i].SetSelectedColor(true);
            }
            else
            {
                slots[i].SetSelectedColor(false);
            }
        }
    }

    public void Open()
    {
        gameObject.SetActive(true);
        selectedIndex = 0;
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void DeleteMoveText()
    {
        foreach (var text in slots)
        {
            Destroy(text.gameObject);
        }
        slots.Clear();
    }
}
