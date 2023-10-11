using UnityEngine;
using DG.Tweening;
// �V���O���g��
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    // �t�F�[�h�̊Ԋu
    [SerializeField] private float _fadeInDuration;
    [SerializeField] private float _fadeOutDuration;

    // BGM
    [SerializeField, Range(0f, 1f)] private float _volumeBGM;
    [SerializeField] private AudioSource _audioSourceBGM; // BGM�̃X�s�[�J�[
    [SerializeField] private AudioClip[] _audioClipsBGM;  // BGM�̉���

    // SE
    [SerializeField, Range(0f, 1f)] private float _volumeSE;
    [SerializeField] private AudioSource _audioSourceSE; // SE�̃X�s�[�J�[
    [SerializeField] private AudioClip[] _audioClipsSE;// SE�̉���

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
    /// BGM��炷
    /// </summary>
    public void PlayBGM(GameState state)
    {
        Sequence sequence = DOTween.Sequence();
        // �t�F�[�h�A�E�g
        if (_audioSourceBGM.clip != null)// �X�^�[�g���󔒂̎��Ԃ��o���邽��
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

        // �t�F�[�h�C��
        sequence.AppendCallback(() => _audioSourceBGM.Play())
            .Append(_audioSourceBGM.DOFade(_volumeBGM, _fadeInDuration));
    }
    /// <summary>
    /// ���ʉ���炷
    /// </summary>
    public void PlaySE(int index)
    {
        //TODO: �}�W�b�N�i���o�[����͂��Ȃ���΂Ȃ�Ȃ����ߗv�C��
        _audioSourceSE.PlayOneShot(_audioClipsSE[index]);
    }
}
