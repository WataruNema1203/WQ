using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TreasureBoxController : MonoBehaviour
{
    [SerializeField] List<TreasureBoxBase> boxBases = new List<TreasureBoxBase>();

    public List<TreasureBoxBase> BoxBase { get => boxBases; set => boxBases = value; }
}
