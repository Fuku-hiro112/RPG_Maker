using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EfectAnimation : MonoBehaviour
{
    private Image _image;

    private void Start()
    {
        _image = GetComponent<Image>();
        _image.color = Color.clear;
    }
    /// <summary>
    /// 物理技のエフェクト
    /// </summary>
    public void MovePhysicalAttackEfect(Sprite sprite,BattleUnit defender)
    {
        // エフェクトImageをSoceImageにセットする
        _image.sprite = sprite;
        transform.localPosition = defender.OriginalPos;

        _image.color = Color.white;// 表示

        _image.DOFade(endValue: 0f, duration: 2f).SetEase(Ease.InCubic);
    }
    /// <summary>
    /// 特殊技のエフェクト
    /// </summary>
    public void MoveSpecialAttackEfect(Sprite sprite, BattleUnit attacker, BattleUnit defender)
    {
        // エフェクトImageをSoceImageにセットする
        _image.sprite = sprite;

        _image.color = Color.white;// 表示

        transform.localPosition = attacker.OriginalPos;
        // Dotweenで敵の位置まで動かす
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(defender.transform.position,0.2f));
        sequence.Append(_image.DOFade( 0f, 0f));
    }
    /// <summary>
    /// 回復技のエフェクト
    /// </summary>
    public void MoveHealEfect(Sprite sprite, BattleUnit attacker)
    {
        // 円形に発生させたい為、最初は0に
        _image.fillAmount = 0f;

        // エフェクトImageをSoceImageにセットする
        _image.sprite = sprite;
        transform.localPosition = attacker.OriginalPos;

        _image.color = Color.white;// 表示

        Sequence sequence = DOTween.Sequence();
        sequence.Append(_image.DOFillAmount(endValue: 1f, duration: 0.5f));
        sequence.Append(_image.DOFade(endValue: 0f, duration: 1f).SetEase(Ease.InCubic));
    }
}
