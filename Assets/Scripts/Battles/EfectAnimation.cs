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
    /// �����Z�̃G�t�F�N�g
    /// </summary>
    public void MovePhysicalAttackEfect(Sprite sprite,BattleUnit defender)
    {
        // �G�t�F�N�gImage��SoceImage�ɃZ�b�g����
        _image.sprite = sprite;
        transform.localPosition = defender.OriginalPos;

        _image.color = Color.white;// �\��

        _image.DOFade(endValue: 0f, duration: 2f).SetEase(Ease.InCubic);
    }
    /// <summary>
    /// ����Z�̃G�t�F�N�g
    /// </summary>
    public void MoveSpecialAttackEfect(Sprite sprite, BattleUnit attacker, BattleUnit defender)
    {
        // �G�t�F�N�gImage��SoceImage�ɃZ�b�g����
        _image.sprite = sprite;

        _image.color = Color.white;// �\��

        transform.localPosition = attacker.OriginalPos;
        // Dotween�œG�̈ʒu�܂œ�����
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(defender.transform.position,0.2f));
        sequence.Append(_image.DOFade( 0f, 0f));
    }
    /// <summary>
    /// �񕜋Z�̃G�t�F�N�g
    /// </summary>
    public void MoveHealEfect(Sprite sprite, BattleUnit attacker)
    {
        // �~�`�ɔ������������ׁA�ŏ���0��
        _image.fillAmount = 0f;

        // �G�t�F�N�gImage��SoceImage�ɃZ�b�g����
        _image.sprite = sprite;
        transform.localPosition = attacker.OriginalPos;

        _image.color = Color.white;// �\��

        Sequence sequence = DOTween.Sequence();
        sequence.Append(_image.DOFillAmount(endValue: 1f, duration: 0.5f));
        sequence.Append(_image.DOFade(endValue: 0f, duration: 1f).SetEase(Ease.InCubic));
    }
}
