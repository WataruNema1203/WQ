using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�ǂ̃��x���Ł��̋Z���o����̂���Ή��t����
[System.Serializable]
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase MoveBase { get => moveBase;}
    public int Level { get => level; }
}
