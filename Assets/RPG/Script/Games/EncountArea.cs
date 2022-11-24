using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncountArea : MonoBehaviour
{
    //•¡”‚Ì“G‚ğƒZƒbƒg

    [SerializeField] List<EnemyIndex> enemyIndex;

    //ƒ‰ƒ“ƒ_ƒ€‚É‚P‘Ì“n‚·
    public Battler GetRandomBattler()
    {
        int r = Random.Range(0, enemyIndex.Count);

        if (PlayerController.Instance.EnemyBattlers.Count == 0)
        {
            return enemyIndex[r].Enemies[0];
        }
        else
        {
            int battleIndex = 0;
            for (int i = 0; i < PlayerController.Instance.EnemyBattlers.Count; i++)
            {
                for (int j = 0; j < enemyIndex[r].Enemies.Count; j++)
                {
                    if (PlayerController.Instance.EnemyBattlers[i].Base.Name == enemyIndex[r].Enemies[j].Base.Name && PlayerController.Instance.EnemyBattlers[i].BattlerIndex == enemyIndex[r].Enemies[j].BattlerIndex)
                    {
                        battleIndex++;
                    }
                }

            }
            return enemyIndex[r].Enemies[battleIndex];
        }
    }

}

[System.Serializable]
public class EnemyIndex
{
    [SerializeField] List<Battler> enemies;

    public List<Battler> Enemies { get => enemies; }
}