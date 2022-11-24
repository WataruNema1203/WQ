using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionDB
{
    //�L�[�AValue
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.LowContinuation,
            new Condition()
            {
                Id=ConditionID.LowContinuation,
                Name="�f���C",
                StartMessage="�͓f�������ɂȂ���",
                OnAfterTurn=(Battler target) =>
                {
                    //��p���_���[�W��^����
                    target.HP-=target.MaxHP/16;
                    //��p���_���[�W�̃��O���o��
                    target.StatusChanges.Enqueue($"{target.Base.Name}�͓f�������ɂȂ��ċꂵ��ł���");
                }
            }
        },
        {
            ConditionID.HighContinuation,
            new Condition()
            {
                Id=ConditionID.HighContinuation,
                Name="�q�f",
                StartMessage="�͓f���n�߂�",
                OnAfterTurn=(Battler target) =>
                {
                    //�₯�ǃ_���[�W��^����
                    target.HP-=target.MaxHP/8;
                    //�₯�ǃ_���[�W�̃��O���o��
                    target.StatusChanges.Enqueue($"{target.Base.Name}�͓f���܂����ċꂵ��ł���");
                }
            }
        },
        {
            ConditionID.Barn,
            new Condition()
            {
                Id=ConditionID.HighContinuation,
                Name="���ǂ肠��",
                StartMessage="�̓t���t���ŕ����n�߂�",
                OnAfterTurn=(Battler target) =>
                {
                    //�₯�ǃ_���[�W��^����
                    target.HP-=target.MaxHP/16;
                    //�₯�ǃ_���[�W�̃��O���o��
                    target.StatusChanges.Enqueue($"{target.Base.Name}�̓t���t�������ē]�т܂����Ă���I");
                }
            }
        },
        {
            ConditionID.Paralisis,
            new Condition()
            {
                Id=ConditionID.Paralisis,
                Name="�A�����׶�",
                StartMessage="�̓A�����ł��炾���ӂ邦�n�߂�",
                OnBeforeMove=(Battler target) =>
                {
                    //true:�Z���o����@false:�܂Ђœ����Ȃ�

                    //�E���m����
                    //�E�Z���o�����Ɏ����̃^�[�����I���
                    
                    //1,2,3,4,�̒��łP���o����i25���j
                    if (Random.Range(1,5)==1)
                    {
                        //�Z���o���Ȃ�
                        target.StatusChanges.Enqueue($"{target.Base.name}�͂��炾���ӂ邦�Ăē����Ȃ��I");
                        return false;
                    }
                    return true;
                }
            }
        },
        {
            ConditionID.Freeze,
            new Condition()
            {
                Id=ConditionID.Freeze,
                Name="�ׂ�Ă�",
                StartMessage="�ׂ͒�Ė���n�߂�",

                OnBeforeMove=(Battler target) =>
                {
                    //true:�������܂܁@false:�������Ԃ���񕜂��ē�����悤�ɂȂ�

                    //�E���m����
                    //�E�Z���o�����Ɏ����̃^�[�����I���
                    
                    //0,1,2,3,4�̒���0���o����i20���j
                    if (Random.Range(0,5)==1)
                    {
                        target.CureStatus();
                        target.StatusChanges.Enqueue($"{target.Base.name}�͈ӎ������߂����I");
                        return true;
                    }
                    target.StatusChanges.Enqueue($"{target.Base.name}�ׂ͒�ē����Ȃ��I");
                    return false;
                }
            }
        }, {
            ConditionID.Binding,
            new Condition()
            {
                Id=ConditionID.Binding,
                Name="�O�C�O�C",
                StartMessage="�̓O�C�O�C����͂��߂�",
                OnStart=(Battler target) =>
                {
                    //�Z���󂯂����ɁA���^�[�����邩���߂�
                    target.StatusTime=Random.Range(1,5);//1,2,3�^�[���̂ǂꂩ

                },

                OnBeforeMove=(Battler target) =>
                {
                    //true:�˂ނ����܂܁@false:�˂ނ��Ԃ���񕜂��ă^�[�����I������

                    //�E�����X�^�[���Z���g���Ƃ�
                    //�E�˂ނ�̃^�[���J�E���g�����炷
                    //�E�˂ނ�̃^�[���J�E���g���O�ɂȂ�����s���\
                    //�E�O����Ȃ�������s���s�\
                    if (target.StatusTime <= 0)
                    {
                        target.CureStatus();
                        target.StatusChanges.Enqueue($"{target.Base.name}�͂������݂Ď��肩�瓦�����I");
                        return true;
                    }
                    else
                    {
                    target.StatusChanges.Enqueue($"{target.Base.name}�̓O�C�O�C����Ă�I");
                    target.StatusTime--;
                    return false;
                    }
                }
            }
        }, {
            ConditionID.Confusion,
            new Condition()
            {
                Id=ConditionID.Confusion,
                Name="��������",
                StartMessage="�͐����ς����",
                 OnStart=(Battler target) =>
                {
                    //�Z���󂯂����ɁA���^�[���������邩���߂�
                    target.VolatileStatusTime=Random.Range(1,5);//1,2,3�^�[���̂ǂꂩ

                },

                OnBeforeMove=(Battler target) =>
                {
                    //true:�����񂵂��܂܂Ŏ����_���[�W���󂯂�@false:�������Ԃ���񕜂��ē�����悤�ɂȂ�

                    //�E���m����
                    //�E�Z���o�����Ɏ����Ƀ_���[�W
                    
                    //0,1,2,3,4�̒���0���o����i20���j
                     if (target.VolatileStatusTime <= 0)
                    {
                        target.CureVolatileStatus();
                        target.StatusChanges.Enqueue($"{target.Base.name}�͐��������߂��I");
                        return true;
                    }

                    target.VolatileStatusTime--;

                    if (Random.Range(1,3)==1)
                    {
                        return true;
                    }

                    target.StatusChanges.Enqueue($"{target.Base.name}�͐����ς���Ă���I");
                    target.HP=target.MaxHP/8;
                    target.StatusChanges.Enqueue($"{target.Base.name}�͐����o�܂��Ŏ����̊��@���͂��߂��I");

                    return false;

                }
            }
        },

    };
}

public enum ConditionID
{
    None,             //�Ȃ�
    LowContinuation,  //��p���_��(��)
    HighContinuation, //���p���_���i�ғŁj
    Barn,             //��p���_���[�W�{�U���͒ቺ�i�Ώ��j
    Binding,          //�����{�m���ŉ񕜁i�񕜌�^�[���I���j(�˂ނ�)
    Freeze,           //�����{�m���ŉ񕜁i�񕜌�U���\�j(������)
    Paralisis,        //�����{���܂ɍs��(���)
    Confusion,        //�m���Ŏ��g�Ƀ_���[�Wor����ɍU��
}