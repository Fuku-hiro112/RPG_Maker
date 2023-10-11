using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading;
using Cysharp.Threading.Tasks;
using System;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] private CharaBase _base;
    [SerializeField] private EfectAnimation _efectAnimation;
    [SerializeField] private int _level;
    [SerializeField] private bool _isPlayerUnit;
    [SerializeField] private PlayerInfo _playerInfo;
    [SerializeField] private EnemyInfo _enemyInfo;

    private Vector3 _originalPos;
    private Image _image;
    private Color _originalColor;
    
    private CancellationToken _token;

    // �v���p�e�B
    public Character Character { get; set; }
    public EfectAnimation EfectAnimation { get => _efectAnimation; }
    public Vector3 OriginalPos { get => _originalPos; private set => _originalPos = value; }

    private void Awake()
    {
        // this��t���Ȃ��ƁA���݂̃R���e�L�X�g�ɑ��݂��Ȃ��G���[���o��
        _token = this.GetCancellationTokenOnDestroy();
        OriginalPos = transform.localPosition;
        _image = GetComponent<Image>();
        _originalColor = _image.color;
    }

    /// <summary>
    /// �o�g���Ŏg���L������ێ����摜�𔽉f����B
    /// </summary>
    public void SetUp()
    {
        // _base���烌�x���ɉ������L�����𐶐�����
        if (_isPlayerUnit)
        {
            Character = _playerInfo.Initialize();
            _image.sprite = Character.Base.LeftSprite;
            PlayerEnterAnimation();
        }
        else
        {
            Character = _enemyInfo.Initialize();
            _image.sprite = Character.Base.RightSprite;
        }
        _image.color = _originalColor;
    }

//-------------�A�j���[�V����---------------------------------------------------------------------
    /// <summary>
    /// �o��A�j���[�V����
    /// </summary>
    public void PlayerEnterAnimation()
    {
        Vector3 playerAwakePos = new Vector3(880, OriginalPos.y);
        // ��ʊO�E�[�ɔz�u
        transform.localPosition = playerAwakePos;
        // �퓬���̈ʒu�܂ŃA�j���[�V����
        transform.DOLocalMoveX(OriginalPos.x, 1f);
    }

    /// <summary>
    /// �U���A�j���[�V����
    /// </summary>
    public async UniTask AttackAnimationAsync()
    {
        AudioManager.Instance.PlaySE(2);//�L�����Z����
        // ���E�ǂ��炩�ɓ�������A���̈ʒu�ɖ߂�
        Sequence sequence = DOTween.Sequence();
        if (_isPlayerUnit)
        {
            sequence.Append(transform.DOLocalMoveX(OriginalPos.x - 50f, 0.25f));
        }
        else
        {
            sequence.Append(transform.DOLocalMoveX(OriginalPos.x + 50f, 0.25f));
        }
        //Apend�͏��Ԃɂ���Ă����@�R���[�`���݂�����
        sequence.Append(transform.DOLocalMoveX(OriginalPos.x, 0.2f));

        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: _token);
    }
    /// <summary>
    /// �ω��Z�A�j���[�V����
    /// </summary>
    // ����ǉ��\��

    /// <summary>
    /// �_���[�W�A�j���[�V����
    /// </summary>
    public void HitAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        AudioManager.Instance.PlaySE(3);// �_���[�W��
        for (int i = 0; i<3; i++)
        {
            sequence.Append(_image.DOColor(Color.gray, 0.05f));
            sequence.Append(_image.DOColor(_originalColor, 0.05f));
        }
    }
    /// <summary>
    /// �퓬�s�\�A�j���[�V����
    /// </summary>
    public void FaintAnimation()
    {
        AudioManager.Instance.PlaySE(4);//�����ꂽ��
        // ���ɉ�����Ȃ���A�����Ȃ�
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMoveY(OriginalPos.y - 150, 0.2f));
        sequence.Join(_image.DOFade(0, 0.2f));

        sequence.Append(transform.DOLocalMoveY(OriginalPos.y, 0f));
    }
}
