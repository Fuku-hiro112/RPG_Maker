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

    // プロパティ
    public Character Character { get; set; }
    public EfectAnimation EfectAnimation { get => _efectAnimation; }
    public Vector3 OriginalPos { get => _originalPos; private set => _originalPos = value; }

    private void Awake()
    {
        // thisを付けないと、現在のコンテキストに存在しないエラーが出る
        _token = this.GetCancellationTokenOnDestroy();
        OriginalPos = transform.localPosition;
        _image = GetComponent<Image>();
        _originalColor = _image.color;
    }

    /// <summary>
    /// バトルで使うキャラを保持し画像を反映する。
    /// </summary>
    public void SetUp()
    {
        // _baseからレベルに応じたキャラを生成する
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

//-------------アニメーション---------------------------------------------------------------------
    /// <summary>
    /// 登場アニメーション
    /// </summary>
    public void PlayerEnterAnimation()
    {
        Vector3 playerAwakePos = new Vector3(880, OriginalPos.y);
        // 画面外右端に配置
        transform.localPosition = playerAwakePos;
        // 戦闘時の位置までアニメーション
        transform.DOLocalMoveX(OriginalPos.x, 1f);
    }

    /// <summary>
    /// 攻撃アニメーション
    /// </summary>
    public async UniTask AttackAnimationAsync()
    {
        AudioManager.Instance.PlaySE(2);//キャンセル音
        // 左右どちらかに動いた後、元の位置に戻る
        Sequence sequence = DOTween.Sequence();
        if (_isPlayerUnit)
        {
            sequence.Append(transform.DOLocalMoveX(OriginalPos.x - 50f, 0.25f));
        }
        else
        {
            sequence.Append(transform.DOLocalMoveX(OriginalPos.x + 50f, 0.25f));
        }
        //Apendは順番にやってくれる　コルーチンみたいに
        sequence.Append(transform.DOLocalMoveX(OriginalPos.x, 0.2f));

        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: _token);
    }
    /// <summary>
    /// 変化技アニメーション
    /// </summary>
    // 今後追加予定

    /// <summary>
    /// ダメージアニメーション
    /// </summary>
    public void HitAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        AudioManager.Instance.PlaySE(3);// ダメージ音
        for (int i = 0; i<3; i++)
        {
            sequence.Append(_image.DOColor(Color.gray, 0.05f));
            sequence.Append(_image.DOColor(_originalColor, 0.05f));
        }
    }
    /// <summary>
    /// 戦闘不能アニメーション
    /// </summary>
    public void FaintAnimation()
    {
        AudioManager.Instance.PlaySE(4);//たおれた音
        // 下に下がりながら、薄くなる
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMoveY(OriginalPos.y - 150, 0.2f));
        sequence.Join(_image.DOFade(0, 0.2f));

        sequence.Append(transform.DOLocalMoveY(OriginalPos.y, 0f));
    }
}
