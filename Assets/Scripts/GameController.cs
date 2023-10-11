using DG.Tweening;
using UnityEngine;

public enum GameState
{
    FreeRoam, // �}�b�v�ړ�
    Battle,�@�@// �퓬��
    GameClear  // �Q�[���N���A
}

public class GameController : MonoBehaviour, IGameController
{
    // �Q�[���̏�Ԃ��Ǘ�
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private BattleSystem _battleSystem;
    [SerializeField] private GameClear _gameClear;
    [SerializeField] private Camera _worldCamera;
    [SerializeField] private float _goalPositionY;

    private GameState _state = GameState.FreeRoam;
    private void Start()
    {
        AudioManager.Instance.PlayBGM(_state);// BGM���Đ�
    }
    private void Update()
    {
        // �t�B�[���h������Ă�����
        if (_state == GameState.FreeRoam)
        {
            _playerController.HandleUpdate();
            if (_playerController.transform.position.y >= _goalPositionY)
            {
                _state = GameState.GameClear;
            }
        }// �o�g�����Ă�����
        else if (_state == GameState.Battle)
        {
            _battleSystem.HandleUpdate();
        }
        else 
        {
            _gameClear.HandleUpdate(_state);
        }
    }

    /// <summary>
    /// �o�g�����n�߂�, IGameController �Ɏ���
    /// </summary>
    public void StartBattle()
    {
        _state = GameState.Battle;
        AudioManager.Instance.PlayBGM(_state);// BGM���Đ�

        _battleSystem.gameObject.SetActive(true);
        _worldCamera.gameObject.SetActive(false);

        // �퓬�J�n
        _battleSystem.StartBattle();
    }
    /// <summary>
    /// �o�g�����I����, IGameController �Ɏ���
    /// </summary>
    public void EndBattle()
    {
        _state = GameState.FreeRoam;
        AudioManager.Instance.PlayBGM(_state);// BGM���Đ�

        _battleSystem.gameObject.SetActive(false);
        _worldCamera.gameObject.SetActive(true);
    }

}
