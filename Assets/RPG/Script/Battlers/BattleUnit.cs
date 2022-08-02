using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class BattleUnit : MonoBehaviour
{

    //UIの管理
    //Battlerの管理
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
        //UIの初期化
        //Enemy:画像と名前の設定
        //Player：名前とステータスの設定
    }

    public virtual void UpdateUI()
    {

    }

    //登場アニメーション
    public void PlayerResetAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMoveY(originalPos.y + 150f, 0f));
        sequence.Join(image.DOFade(255, 0f));
        sequence.Join(image1.DOFade(255, 0f));
        sequence.Join(image2.DOFade(255, 0f));
        sequence.Join(NameText.DOFade(255, 0f));

    }

    //戦闘不能アニメーション
    public void PlayerFaintAnimaion()
    {
        //下に下がりながら、薄くなる
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0, 0.5f));
        sequence.Join(image1.DOFade(0, 0.5f));
        sequence.Join(image2.DOFade(0, 0.5f));
        sequence.Join(NameText.DOFade(0, 0.5f));

    }


}
