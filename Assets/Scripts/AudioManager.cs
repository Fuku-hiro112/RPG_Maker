using UnityEngine;
using DG.Tweening;
// シングルトン
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    // フェードの間隔
    [SerializeField] private float _fadeInDuration;
    [SerializeField] private float _fadeOutDuration;

    // BGM
    [SerializeField, Range(0f, 1f)] private float _volumeBGM;
    [SerializeField] private AudioSource _audioSourceBGM; // BGMのスピーカー
    [SerializeField] private AudioClip[] _audioClipsBGM;  // BGMの音源

    // SE
    [SerializeField, Range(0f, 1f)] private float _volumeSE;
    [SerializeField] private AudioSource _audioSourceSE; // SEのスピーカー
    [SerializeField] private AudioClip[] _audioClipsSE;// SEの音源

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// BGMを鳴らす
    /// </summary>
    public void PlayBGM(GameState state)
    {
        Sequence sequence = DOTween.Sequence();
        // フェードアウト
        if (_audioSourceBGM.clip != null)// スタート時空白の時間が出来るため
        {
            sequence.Append(_audioSourceBGM.DOFade(0, _fadeOutDuration)).
                AppendCallback(() => _audioSourceBGM.Stop());
        }

        switch (state)
        {
            case GameState.FreeRoam:
                _audioSourceBGM.clip = _audioClipsBGM[(byte)GameState.FreeRoam];
                break;
            case GameState.Battle:
                _audioSourceBGM.clip = _audioClipsBGM[(byte)GameState.Battle];
                break;
        }

        // フェードイン
        sequence.AppendCallback(() => _audioSourceBGM.Play())
            .Append(_audioSourceBGM.DOFade(_volumeBGM, _fadeInDuration));
    }
    /// <summary>
    /// 効果音を鳴らす
    /// </summary>
    public void PlaySE(int index)
    {
        //TODO: マジックナンバーを入力しなければならないため要修正
        _audioSourceSE.PlayOneShot(_audioClipsSE[index]);
    }
}
