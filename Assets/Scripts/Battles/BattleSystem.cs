using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState
{
    Start,        // 始めた
    DoAction,     // 行動するかを選択
    PlayerAction, // 行動選択
    PlayerMove,   // 技選択
    EnemyMove,    // 敵の行動
    Busy,         // 処理中
}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] private BattleUnit _playerUnit;
    [SerializeField] private BattleUnit _enemyUnit;
    [SerializeField] private BattleHud _playerHud;
    [SerializeField] private BattleHud _enemyHud;
    [SerializeField] private BattleDialogBox _dialogBox;

    [SerializeField] private GameController _gameController;
    [SerializeField] private GridLayoutGroup _moveSerectionGridLayoutGroup;
    
    private BattleState _state;
    private DoActionSelect _currentDoAct;// 0:たたかう, 1:にげる
    private enum DoActionSelect
    {
        Fight,
        Escape
    }
    private ActionSelect _currentAct;  // 0:こうげき, 1:スキル, 2:ぼうぎょ, 3:アイテム
    private enum ActionSelect// 防御、アイテムはまだ未実装
    {
        Move,
        Defense,
        Item
    }

    private int _currentMove;// 0～16まで技がある
    private int runNum = 0;// 逃走回数

    private IGameController _iGameController;
    private CancellationToken token;

    /// <summary>
    /// バトル開始時
    /// </summary>
    public void StartBattle()
    {
        runNum = 0;
        _iGameController = _gameController;
        token = this.GetCancellationTokenOnDestroy();
        SetupBattleAsync().Forget();
    }
    /// <summary>
    /// スタート時のセットアップ
    /// </summary>
    private async UniTaskVoid SetupBattleAsync()
    {
        _state = BattleState.Start;

        // キャラの生成と描画
        _playerUnit.SetUp();
        _enemyUnit.SetUp();

        // HUDの描画 _playerUnit.Characterを渡すのでは無く
        _playerHud.SetData(_playerUnit.Character);// Playerデータ
        _enemyHud.SetHpData(_enemyUnit.Character);// 敵HPデータ
        _playerHud.ShowHpText(_playerUnit.Character);// HPテキスト
        _playerHud.ShowSpText(_playerUnit.Character);// SPテキスト

        // パネルの描画
        _dialogBox.SetMoveNames(_playerUnit.Character.Moves); // 技名テキスト
        await _dialogBox.TypeDialogAsync($"{_enemyUnit.Character.Base.Name}が現れた");
        await UniTask.Delay(TimeSpan.FromSeconds(0.7f), cancellationToken: token);

        ChooseFirstTurn();
    }
    /// <summary>
    /// どちらのスピードが早いか判定して先攻後攻を決める
    /// </summary>
    private void ChooseFirstTurn()
    {
        if (IsPlayerFastar())
        {
            DoAction();
        }
        else if (_playerUnit.Character.Speed == _enemyUnit.Character.Speed)
        {
            // ランダムでターンを決める
            byte player = 0;
            byte enemy = 1;
            byte turn = (byte)UnityEngine.Random.Range(player, enemy+1);// CS0104エラーの為UnityEngine.を付けました

            if (turn == player)
            {
                DoAction();
            }
            else
            {
                EnemyMoveAsync().Forget();
            }
        }
        else
        {
            EnemyMoveAsync().Forget();
        }
    }


    /// <summary>
    /// PlayerActionに移行するか
    /// </summary>
    private void DoAction()
    {
        _state = BattleState.DoAction;

        // パネルの描画
        _dialogBox.EnableDoActionSelection(true);// 行動するか選択するテキストの表示
        _dialogBox.TypeDialogAsync("たたかう？").Forget();
    }
    /// <summary>
    /// Playerの行動選択画面へ
    /// </summary>
    private void PlayerAction()
    {
        _state = BattleState.PlayerAction;

        // パネルの描画
        _dialogBox.EnableDoActionSelection(false);// 行動するか選択するテキストの非表示　←長っ
        _dialogBox.EnableActionSelection(true);   // 行動選択テキストの表示
        _dialogBox.TypeDialogAsync("どうする？").Forget();
    }
    /// <summary>
    /// Playerの技選択画面へ
    /// </summary>
    private void PlayerMove()
    {
        _state = BattleState.PlayerMove;
        //_dialogBox.EnableActionSerection(false);
        _dialogBox.EnableBattleDialog(false);// 戦闘メッセージパネル・テキストの非表示
        _dialogBox.EnableMoveSelector(true); // 技選択パネル・テキストの表示
    }
    /// <summary>
    /// 行動選択画面をキャンセルして行動するか選択する画面に戻る
    /// </summary>
    private void CancelPlayerAction()
    {
        _dialogBox.EnableActionSelection(false);// 行動選択テキストの非表示
        DoAction();
    }
    /// <summary>
    /// 技選択画面をキャンセルして行動選択画面に戻る
    /// </summary>
    private void CancelPlayerMove()
    {
        _dialogBox.EnableBattleDialog(true);// 戦闘メッセージパネル・テキストの表示
        _dialogBox.EnableMoveSelector(false);
        PlayerAction();
    }

    /// <summary>
    /// 逃げる処理
    /// </summary>
    private async UniTask PlayerEscapeMoveAsync()
    {
        _state = BattleState.Busy;
        // 成功確率は逃走回数に起因する　1: 25％ 2: 50% 3: 75% 4:100%
        await _dialogBox.TypeDialogAsync($"{_playerUnit.Character.Base.Name}は逃げ出した");

        byte baseEscapeChance = 25;
        runNum++;
        //成功　敵よりも素早さが高い & 成功率を上回る
        if (IsPlayerFastar() || 
            UnityEngine.Random.value * 100 >= runNum * baseEscapeChance)
        {
            _dialogBox.EnableDoActionSelection(false);// 行動するか選択するテキストの非表示
            // バトル終了処理
            _iGameController.EndBattle();
        }
        else //失敗
        {
            Debug.Log(runNum * baseEscapeChance + " " + UnityEngine.Random.value * 100);
            await _dialogBox.TypeDialogAsync($"しかし、回り込まれてしまった");
            // 相手のターン
            EnemyMoveAsync().Forget();
        }

    }
    /// <summary>
    /// Playerの方が素早さが早いかどうか
    /// </summary>
    public bool IsPlayerFastar()=> _playerUnit.Character.Speed > _enemyUnit.Character.Speed;
    

    /// <summary>
    /// Playerの行動を実行
    /// </summary>
    public async UniTask PerformPlayerMoveAsync()
    {
        _state = BattleState.Busy;
        // 技を決定
        Move move = _playerUnit.Character.Moves[_currentMove];

        // Sp消費
        _playerUnit.Character.SpConsumption(move, _playerUnit.Character);// Sp計算
        _playerHud.UpdateSp();                                           // SpBar反映
        _playerHud.ShowSpText(_playerUnit.Character);                    // SpText反映

        // 技使用テキスト表示
        await _dialogBox.TypeDialogAsync($"{_playerUnit.Character.Base.Name}は{move.Base.Name}をつかった");

        // Player技使用結果
        MoveResoltDetails damageDetails = await move.Base.RunMoveResultAsync(_playerUnit, _enemyUnit);

        // Hp反映
        await _enemyHud.UpdateHpAsync();
        await _playerHud.UpdateHpAsync();
        _playerHud.ShowHpText(_playerUnit.Character);

        //ダメージ結果を表示
        await _dialogBox.TypeDialogAsync(damageDetails.MoveResult);

        // ダメージ詳細をテキストで表示
        await ShowDamageDetailsAsync(damageDetails);

        if (damageDetails.Fainted)// 戦闘不能
        {
            // 戦闘不能アニメーション
            _enemyUnit.FaintAnimation();
            
            // 戦闘不能時メッセージ
            await _dialogBox.TypeDialogAsync($"{_enemyUnit.Character.Base.Name}はたおれた");
            
            // 倒した敵から経験値をえる
            _playerUnit.Character.HasExperience += _enemyUnit.Character.Experience;

            await _dialogBox.TypeDialogAsync
                ($"{_playerUnit.Character.Base.Name}は経験値{_enemyUnit.Character.Experience}を得た");
            
            if (_playerUnit.Character.IsLevelUp())// 一定以上経験値が溜まると,レベルが上がる
            {
                // レベルアップ
                _playerUnit.Character.Level++;
                // Hp,Spの回復
                _playerUnit.Character.Hp = _playerUnit.Character.MaxHp;
                _playerUnit.Character.Sp = _playerUnit.Character.MaxSp;

                // データの反映
                _playerHud.UpdateLevel();
                _playerHud.SetHpData(_playerUnit.Character);
                _playerHud.UpdateSp();
                _playerHud.ShowHpText( _playerUnit.Character);
                _playerHud.ShowSpText( _playerUnit.Character);

                await _dialogBox.TypeDialogAsync
                    ($"{_playerUnit.Character.Base.Name}はレベル{_playerUnit.Character.Level}になった！");

                // 特定のレベルなら技を覚える
                Move learnedMove = _playerUnit.Character.LearnedMove();
                if ( learnedMove != null )
                {
                    await _dialogBox.TypeDialogAsync
                        ($"{_playerUnit.Character.Base.Name}は{learnedMove.Base.Name}を得た");
                }
            }

            // バトル終了処理
            _iGameController.EndBattle();
        }
        else // 戦闘可能
        {
            // 敵の行動
            EnemyMoveAsync().Forget();
        }
    }
    /// <summary>
    /// 敵の行動
    /// </summary>
    private async UniTask EnemyMoveAsync()
    {
        _state = BattleState.EnemyMove;

        // 技を決定
        Move move = _enemyUnit.Character.GetRandomMove();
        // 技使用テキスト表示
        await _dialogBox.TypeDialogAsync($"{_enemyUnit.Character.Base.Name}は{move.Base.Name}をつかった");

        // Enemy行動結果
        MoveResoltDetails damageDetails = await move.Base.RunMoveResultAsync(_enemyUnit, _playerUnit);

        // Hp反映
        await _playerHud.UpdateHpAsync();　　　　　 // HpBar反映
        await _enemyHud.UpdateHpAsync();　　　　　 // HpBar反映
        _playerHud.ShowHpText(_playerUnit.Character); // HpText反映

        //ダメージ結果を表示
        await _dialogBox.TypeDialogAsync(damageDetails.MoveResult);


        await ShowDamageDetailsAsync(damageDetails);// ダメージ詳細をテキスト表示

        if (damageDetails.Fainted)// 戦闘不能
        {
            // 戦闘不能アニメーション
            _playerUnit.FaintAnimation();
            // 戦闘不能時メッセージ
            await _dialogBox.TypeDialogAsync($"{_playerUnit.Character.Base.Name}はたおれた");

            // バトル終了処理

            //ゲームプレイ終了
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
Application.Quit();
#endif
        }
        else // 戦闘可能
        {
            // Playerの行動へ
            DoAction();
        }
    }

    /// <summary>
    /// ダメージの詳細をテキストで表示
    /// </summary>
    private async UniTask ShowDamageDetailsAsync(MoveResoltDetails damageDetails)
    {
        int nomal = 1;

        if (damageDetails.Critical > nomal)
        {
            await _dialogBox.TypeDialogAsync($"急所に当たった");
        }
        if (damageDetails.TypeEffectiveness > nomal)
        {
            await _dialogBox.TypeDialogAsync($"効果はバツグンだ");
        }
        else if (damageDetails.TypeEffectiveness < nomal)
        {
            await _dialogBox.TypeDialogAsync($"効果はいまひとつ");
        }
    }

    /// <summary>
    /// Update 状態ごとの処理
    /// </summary>
    public void HandleUpdate()
    {
        if (_state == BattleState.DoAction)
        {
            // 行動するかを選択
            HandleDoActionSelection();
        }
        else if (_state == BattleState.PlayerAction)
        {
            // 行動選択
            HandleActionSelection();
        }
        else if (_state == BattleState.PlayerMove)
        {
            // 技選択
            HandleMoveSelection();
        }
    }

//-----------------コマンド処理----------------------------------------------------------------------
    /// <summary>
    /// DoActionでの行動を処理する
    /// </summary>
    private void HandleDoActionSelection()
    {
        // 0:たたかう　1:にげる
        if (Button.Down())
        {
            if (_currentDoAct < DoActionSelect.Escape)// TODO: ActionSerectionの数を参照したい
            {
                _currentDoAct++;
            }
        }
        if (Button.Up())
        {
            if (_currentDoAct > DoActionSelect.Fight)
            {
                _currentDoAct--;
            }
        }
        // 色をつけてどちらを選択しているか分かるようにする
        _dialogBox.UpdateDoActionSelection((int)_currentDoAct);

        if (Button.Accept())
        {
            AudioManager.Instance.PlaySE(0);//選択音
            if (_currentDoAct == DoActionSelect.Fight)
            {
                PlayerAction();
            }
            else if (_currentDoAct == DoActionSelect.Escape)
            {
                PlayerEscapeMoveAsync().Forget();
            }
        }
    }
    /// <summary>
    /// PlayerActionでの行動を処理する
    /// </summary>
    private void HandleActionSelection()
    {
        // 0:こうげき, 1:スキル, 2:ぼうぎょ, 3:アイテム
        if (Button.Down())
        {
            if (_currentAct < ActionSelect.Item)// ActionSerectionの数を参照したほうがいいかな？
            {
                _currentAct++;
            }
        }
        if (Button.Up())
        {
            if (_currentAct > ActionSelect.Move)
            {
                _currentAct--;
            }
        }

        // 色をつけてどちらを選択しているか分かるようにする
        _dialogBox.UpdateActionSelection((int)_currentAct);

        if (Button.Accept())
        {
            if (_currentAct == ActionSelect.Move)
            {
                AudioManager.Instance.PlaySE(0);//選択音
                PlayerMove();
            }
            else if (_currentAct == ActionSelect.Defense)
            {
                // 防御する
                // まだ未実装のためキャンセル音を
                AudioManager.Instance.PlaySE(1);//キャンセル音
            }
            else if (_currentAct == ActionSelect.Item)
            {
                // アイテムパネル
                // まだ未実装のためキャンセル音を
                AudioManager.Instance.PlaySE(1);//キャンセル音
            }
        }
        else if (Button.Cancel())
        {
            AudioManager.Instance.PlaySE(1);//キャンセル音
            CancelPlayerAction();
        }
    }
    /// <summary>
    /// PlayerMoveでの行動を処理する
    /// </summary>
    private void HandleMoveSelection()
    {
  /*0　1
    2　3
    4　5
    という並びになっている*/
        // 行数
        int lineNum = _moveSerectionGridLayoutGroup.constraintCount;

        if (Button.Right())
        {
            if (_currentMove < _playerUnit.Character.Moves.Count - 1)
            {
                _currentMove++;
            }
        }
        if (Button.Left())
        {
            if (_currentMove > 0)
            {
                _currentMove--;
            }
        }
        //列を変える
        if (Button.Down())
        {
            // 一番下の列にいる時、下に動かさないように
            if (_currentMove < _playerUnit.Character.Moves.Count - lineNum)
            {
                _currentMove += lineNum;
            }
        }
        if (Button.Up())// 上の列へ
        {
            // 1列目にいる時、上に動かさないようにする
            if (_currentMove > lineNum - 1)
            {
                _currentMove -= lineNum;
            }
        }

        // 色をつけてどちらを選択しているか分かるようにする
        _dialogBox.UpdateMoveSelection(_currentMove, _playerUnit.Character.Moves[_currentMove]);

        if (Button.Accept())
        {
            // SPが足りていれば下記の処理を実行
            if (_playerUnit.Character.Moves[_currentMove].Sp <= _playerUnit.Character.Sp)
            {
                AudioManager.Instance.PlaySE(0);//選択音
                
                // 技選択のUIは非表示
                _dialogBox.EnableMoveSelector(false);
                _dialogBox.EnableActionSelection(false);
                
                // メッセージダイヤログ復活
                _dialogBox.EnableBattleDialog(true);
                
                // 技決定処理
                PerformPlayerMoveAsync().Forget();
            }
            else
            {
                // Spが足りていない場合の処理
                AudioManager.Instance.PlaySE(1);//キャンセル音
            }
            
        }
        else if (Button.Cancel())
        {
            AudioManager.Instance.PlaySE(1);//キャンセル音
            CancelPlayerMove();
        }
    }
//-------------------------------------------------------------------------------------------------------
}
