using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//どのレベルで℃の技を覚えるのかを対応付ける
[System.Serializable]
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase MoveBase { get => moveBase;}
    public int Level { get => level; }
}
