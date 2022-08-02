using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Condition
{
    public ConditionID Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    //��Ԉُ�̃��b�Z�[�W
    public string StartMessage { get; set; }

    //�^�[���̈�ԍŏ��ɂ�肽������
    public Action<Battler> OnStart;
    //�^�[���I�����Ɏ��s����������
    public Func<Battler, bool> OnBeforeMove;
    public Action<Battler> OnAfterTurn;
}

