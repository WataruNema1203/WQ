using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//‚Ç‚ÌƒŒƒxƒ‹‚ÅŽ‚Ì‹Z‚ðŠo‚¦‚é‚Ì‚©‚ð‘Î‰ž•t‚¯‚é
[System.Serializable]
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase MoveBase { get => moveBase;}
    public int Level { get => level; }
}
