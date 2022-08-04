using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialog
{
    [SerializeField] List<Line> lines = new List<Line>();

    internal List<Line> Lines { get => lines; set => lines = value; }
}

[System.Serializable]
class Line
{
    [TextArea]
    [SerializeField] string log;
    [SerializeField] BattlerBase battlerBase;

    public string Log { get => log; set => log = value; }
    public BattlerBase BattlerBase { get => battlerBase; set => battlerBase = value; }
}