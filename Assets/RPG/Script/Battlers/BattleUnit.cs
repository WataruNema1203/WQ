using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class BattleUnit : MonoBehaviour
{

    //UI�̊Ǘ�
    //Battler�̊Ǘ�
    public Battler Battler { get; set; }
    public Image Image { get => image; }
    public Text NameText { get => nameText; }

    Vector3 originalPos;

    [SerializeField] Image image;
    [SerializeField] Image image1;
    [SerializeField] Image image2;
    [SerializeField] Text nameText;


    public virtual void Setup(Battler battler)
    {
        Battler = battler;
        //UI�̏�����
        //Enemy:�摜�Ɩ��O�̐ݒ�
        //Player�F���O�ƃX�e�[�^�X�̐ݒ�
    }

    public virtual void UpdateUI()
    {

    }

    //�o��A�j���[�V����
    public void PlayerResetAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMoveY(originalPos.y + 150f, 0f));
        sequence.Join(image.DOFade(255, 0f));
        sequence.Join(image1.DOFade(255, 0f));
        sequence.Join(image2.DOFade(255, 0f));
        sequence.Join(NameText.DOFade(255, 0f));

    }

    //�퓬�s�\�A�j���[�V����
    public void PlayerFaintAnimaion()
    {
        //���ɉ�����Ȃ���A�����Ȃ�
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0, 0.5f));
        sequence.Join(image1.DOFade(0, 0.5f));
        sequence.Join(image2.DOFade(0, 0.5f));
        sequence.Join(NameText.DOFade(0, 0.5f));

    }


}
