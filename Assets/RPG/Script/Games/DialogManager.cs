using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] Text dialogText;
    [SerializeField] Text nameText;
    [SerializeField] float letterPerSecond = 0.05f;

    public UnityAction OnShowDialog;
    public UnityAction OnCloseDialog;
    public UnityAction OnDialogFinished;

    bool isTyping;
    int currentLine;
    [SerializeField] Dialog dialog;

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
        yield return TypeDialog(dialog.Lines[currentLine].Log);
    }

    public IEnumerator TypeDialog(string line, bool auto = true)
    {

        dialogBox.SetActive(true);
        isTyping = true;
        dialogText.text = "";
        nameText.text = "";
        yield return null;
        if (dialog.Lines!=null)
        {
            nameText.text = $"【{dialog.Lines[currentLine].BattlerBase.Name}】";
        }
        foreach (var letter in line)
        {
            dialogText.text += letter;

            yield return new WaitForSeconds(letterPerSecond);
        }
        //WaitUntilでボタン押したタイミングでリターンできる
        if (auto == false)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));

        }
        isTyping = false;
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
                dialog.Lines.Clear();
                dialogBox.SetActive(false);
                OnDialogFinished?.Invoke();
                OnCloseDialog?.Invoke();
            }
        }
    }

    public void Close()
    {
        dialogBox.SetActive(false);
    }
}