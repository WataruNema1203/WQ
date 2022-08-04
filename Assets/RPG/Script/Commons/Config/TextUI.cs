using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextUI : SelectableText
{
    public bool IsEmpty
    {
        get => base.text.text == "-";
    }
}
