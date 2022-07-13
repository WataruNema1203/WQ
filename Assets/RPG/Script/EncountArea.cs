using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncountArea : MonoBehaviour
{
    //•¡”‚Ì“G‚ğƒZƒbƒg

    [SerializeField] List<Battler> enemies;

    //ƒ‰ƒ“ƒ_ƒ€‚É‚P‘Ì“n‚·
    public Battler GetRandomBattler()
    {
        int r = Random.Range(0, enemies.Count);
        return enemies[r];
    }

}
