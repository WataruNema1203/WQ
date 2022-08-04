using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUI : SelectableText
{
    public bool IsEmpty
    {
        get => base.text.text == "-";
    }
}
