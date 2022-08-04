using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] GameObject battleDialogBox;
    [SerializeField] Text dialogText;
    [SerializeField] Text battleDialogText;
    [SerializeField] Text nameText;
    [SerializeField] float letterPerSecond = 0.05f;

    public UnityAction OnShowDialog;
    public UnityAction OnCloseDialog;
    public UnityAction OnDialogFinished;

    bool isTyping;
    bool isBattle = true;
    int currentLine;
    [SerializeField] Dialog dialog;
    Line enpty;

    public static DialogManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public IEnumerator ShowDialog(Dialog dialog, UnityAction OnFinished = null)
    {
        for (int i = 0; i < dialog.Lines.Count; i++)
        {
            this.dialog.Lines.Add(dialog.Lines[i]);
            Debug.Log(i);

        }
        this.OnDialogFinished = OnFinished;
        this.OnShowDialog?.Invoke();
        isBattle = false;
        yield return TypeDialog(dialog.Lines[currentLine].Log);
    }

    public IEnumerator TypeDialog(string line, bool auto = true)
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
            nameText.text = "";
        }
        isTyping = true;
        yield return null;
        if (!isBattle)
        {
            nameText.text = $"【{dialog.Lines[currentLine].BattlerBase.Name}】";
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
    public IEnumerator FieldTypeDialog(string line, bool auto = true)
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
    }

    public void StartTypingDialog(string line)
    {
        StartCoroutine(TypeDialog(line));
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Z) && isTyping == false)
        {
            currentLine++;
            if (currentLine < dialog.Lines.Count)
            {
                StartCoroutine(TypeDialog(dialog.Lines[currentLine].Log));
            }
            else
            {
                // 会話終了
                currentLine = 0;
                isBattle = true;
                dialog.Lines.Clear();
                dialogBox.SetActive(false);
                battleDialogBox.SetActive(false);
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