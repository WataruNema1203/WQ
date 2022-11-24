using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class BattleUnit : MonoBehaviour
{

    //UIの管理
    //Battlerの管理
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
        //UIの初期化
        //Enemy:画像と名前の設定
        //Player：名前とステータスの設定
    }

    public virtual void UpdateUI()
    {

    }

    public void SetImage()
    {
        this.image = GetComponent<Image>();
        return;
    }

    //明度初期化
    public void ResetAnimation()
    {
        image.color = originalColor;

    }

    public IEnumerator FadeBattleOver()
    {
        yield return image.DOFade(
           0,     // フェード後のアルファ値
          1f      // 演出時間
       );
        image.color = originalColor;
    }

}
