using DG.Tweening;
using UnityEngine;

public enum GameState
{
    FreeRoam, // マップ移動
    Battle,　　// 戦闘中
    GameClear  // ゲームクリア
}

public class GameController : MonoBehaviour, IGameController
{
    // ゲームの状態を管理
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private BattleSystem _battleSystem;
    [SerializeField] private GameClear _gameClear;
    [SerializeField] private Camera _worldCamera;
    [SerializeField] private float _goalPositionY;

    private GameState _state = GameState.FreeRoam;
    private void Start()
    {
        AudioManager.Instance.PlayBGM(_state);// BGMを再生
    }
    private void Update()
    {
        // フィールドを歩いている状態
        if (_state == GameState.FreeRoam)
        {
            _playerController.HandleUpdate();
            if (_playerController.transform.position.y >= _goalPositionY)
            {
                _state = GameState.GameClear;
            }
        }// バトルしている状態
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
    /// バトルを始める, IGameController に実装
    /// </summary>
    public void StartBattle()
    {
        _state = GameState.Battle;
        AudioManager.Instance.PlayBGM(_state);// BGMを再生

        _battleSystem.gameObject.SetActive(true);
        _worldCamera.gameObject.SetActive(false);

        // 戦闘開始
        _battleSystem.StartBattle();
    }
    /// <summary>
    /// バトルを終える, IGameController に実装
    /// </summary>
    public void EndBattle()
    {
        _state = GameState.FreeRoam;
        AudioManager.Instance.PlayBGM(_state);// BGMを再生

        _battleSystem.gameObject.SetActive(false);
        _worldCamera.gameObject.SetActive(true);
    }

}
