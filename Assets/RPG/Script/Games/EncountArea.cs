using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncountArea : MonoBehaviour
{
    //�����̓G���Z�b�g

    [SerializeField] List<Battler> enemies;

    //�����_���ɂP�̓n��
    public Battler GetRandomBattler()
    {
        int r = Random.Range(0, enemies.Count);
        return enemies[r];
    }

}
