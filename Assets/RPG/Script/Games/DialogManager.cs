using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] GameObject battleDialogBox;
    [SerializeField] TMP_Text dialogText;
    [SerializeField] TMP_Text battleDialogText;
    [SerializeField] float letterPerSecond = 0.05f;

    public UnityAction OnShowDialog;
    public UnityAction OnCloseDialog;
    public UnityAction OnDialogFinished;
    public UnityAction<List<Item>, int> OnShopDialogFinished;

    bool isTyping;
    bool isItem = false;
    int currentLine;
    [SerializeField] Dialog dialog;
    readonly Line enpty;
    List<Item> items;
    int shopNpcIndex;

    public static DialogManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }


    public IEnumerator ShowDialog(Dialog dialog, UnityAction OnFinished = null)
    {
        for (int i = 0; i < dialog.Lines.Count; i++)
        {
            string str = dialog.Lines[i].Log;
            this.dialog.Lines.Add(dialog.Lines[i]);
        }
        this.OnShopDialogFinished = null;
        this.OnDialogFinished = OnFinished;
        this.OnShowDialog?.Invoke();
        yield return TypeDialog(dialog.Lines[currentLine].Log, false);
    }
    public IEnumerator ShopShowDialog(Dialog dialog, List<Item> items, int index, UnityAction<List<Item>, int> OnFinished = null)
    {
        for (int i = 0; i < dialog.Lines.Count; i++)
        {
            this.dialog.Lines.Add(dialog.Lines[i]);
        }
        this.OnDialogFinished = null;
        this.shopNpcIndex = index;
        this.items = items;
        this.OnShopDialogFinished = OnFinished;
        this.OnShowDialog?.Invoke();
        yield return TypeDialog(dialog.Lines[currentLine].Log, false);
    }

    public IEnumerator ItemShowDialog(string line, UnityAction OnFinished = null)
    {
        dialog.Lines.Add(enpty);
        this.OnShopDialogFinished = null;
        this.OnDialogFinished = OnFinished;
        this.OnShowDialog?.Invoke();
        isItem = true;
        yield return TypeDialog(line, false);
    }

    public IEnumerator TypeDialog(string line, bool isBattle = true, bool auto = true)
    {
        if (isBattle)
        {
            dialog.Lines.Add(enpty);
            battleDialogBox.SetActive(true);
            battleDialogText.text = "";
        }
        else
        {
            dialogBox.SetActive(true);
            dialogText.text = "";
        }
        isTyping = true;
        yield return null;
        if (isItem)
        {
            foreach (var letter in line)
            {
                dialogText.text += letter;

                yield return new WaitForSeconds(letterPerSecond);
            }

        }
        else if (!isBattle)
        {
            foreach (var letter in line)
            {
                dialogText.text += letter;

                yield return new WaitForSeconds(letterPerSecond);
            }
        }
        else
        {
            foreach (var letter in line)
            {
                battleDialogText.text += letter;

                yield return new WaitForSeconds(letterPerSecond);
            }
        }
        //WaitUntilでボタン押したタイミングでリターンできる
        if (auto == false)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));

        }
        isTyping = false;
        if (isBattle)
        {
            dialog.Lines.Clear();
        }
    }
    public IEnumerator FieldTypeDialog(string line, bool auto = false)
    {
        dialog.Lines.Add(enpty);
        battleDialogBox.SetActive(true);
        battleDialogText.text = "";
        isTyping = true;
        yield return null;
        foreach (var letter in line)
        {
            battleDialogText.text += letter;

            yield return new WaitForSeconds(letterPerSecond);
        }
        //WaitUntilでボタン押したタイミングでリターンできる
        if (auto == false)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));

        }
        isTyping = false;
        dialog.Lines.Clear();
        PlayerController.Instance.StartPlayer();
        Close();
    }

    public void StartTypingDialog(string line)
    {
        StartCoroutine(TypeDialog(line, true));
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Z) && isTyping == false)
        {
            currentLine++;
            if (currentLine < dialog.Lines.Count)
            {
                StartCoroutine(TypeDialog(dialog.Lines[currentLine].Log, false));
            }
            else
            {
                // 会話終了
                currentLine = 0;
                isItem = false;
                dialog.Lines.Clear();
                dialogBox.SetActive(false);
                battleDialogBox.SetActive(false);
                OnShopDialogFinished?.Invoke(items, shopNpcIndex);
                OnDialogFinished?.Invoke();
                OnCloseDialog?.Invoke();
            }
        }
    }

    public void Close()
    {
        dialogBox.SetActive(false);
        battleDialogBox.SetActive(false);
    }
}