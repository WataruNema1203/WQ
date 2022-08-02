using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTypeUI: SelectableText
{

    public bool IsEmpty
    {
        get => base.text.text == "-";
    }
}