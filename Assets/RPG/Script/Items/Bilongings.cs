using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//何個持っているか
[System.Serializable]
public class Bilogings
{
    [SerializeField] ItemBase itemBase;
    [SerializeField] int Possession;

    public ItemBase ItemBase { get => itemBase; }
    public int Possession1 { get => Possession; }
}
