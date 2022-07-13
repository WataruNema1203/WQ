using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : SelectableText
{

    public bool IsEmpty
    {
        get => base.text.text == "-";
    }
}