using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialog
{
    [SerializeField] List<Line> lines = new List<Line>();

    internal List<Line> Lines { get => lines;}
}

[System.Serializable]
class Line
{
    [TextArea]
    [SerializeField] string log;
    [SerializeField] NPCBase nPC;
    [SerializeField] int faceIndex;

    public string Log { get => log;}
    public int FaceIndex { get => faceIndex; }
    public NPCBase NPC { get => nPC;}

    public void SetLog(string str)
    {
        log = str;
    }
}