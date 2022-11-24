using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class BattleUnit : MonoBehaviour
{

    //UI�̊Ǘ�
    //Battler�̊Ǘ�
    public Battler Battler { get; set; }
    public Image Image { get => image; }
    public TextMeshProUGUI NameText { get => nameText; }
    Color originalColor;

    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI nameText;
    private void Awake()
    {
        originalColor = image.color;
    }

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

    public void SetImage()
    {
        this.image = GetComponent<Image>();
        return;
    }

    //���x������
    public void ResetAnimation()
    {
        image.color = originalColor;

    }

    public IEnumerator FadeBattleOver()
    {
        yield return image.DOFade(
           0,     // �t�F�[�h��̃A���t�@�l
          1f      // ���o����
       );
        image.color = originalColor;
    }

}
