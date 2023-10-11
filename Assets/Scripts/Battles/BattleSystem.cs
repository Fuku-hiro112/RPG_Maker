using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState
{
    Start,        // �n�߂�
    DoAction,     // �s�����邩��I��
    PlayerAction, // �s���I��
    PlayerMove,   // �Z�I��
    EnemyMove,    // �G�̍s��
    Busy,         // ������
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
    private DoActionSelect _currentDoAct;// 0:��������, 1:�ɂ���
    private enum DoActionSelect
    {
        Fight,
        Escape
    }
    private ActionSelect _currentAct;  // 0:��������, 1:�X�L��, 2:�ڂ�����, 3:�A�C�e��
    private enum ActionSelect// �h��A�A�C�e���͂܂�������
    {
        Move,
        Defense,
        Item
    }

    private int _currentMove;// 0�`16�܂ŋZ������
    private int runNum = 0;// ������

    private IGameController _iGameController;
    private CancellationToken token;

    /// <summary>
    /// �o�g���J�n��
    /// </summary>
    public void StartBattle()
    {
        runNum = 0;
        _iGameController = _gameController;
        token = this.GetCancellationTokenOnDestroy();
        SetupBattleAsync().Forget();
    }
    /// <summary>
    /// �X�^�[�g���̃Z�b�g�A�b�v
    /// </summary>
    private async UniTaskVoid SetupBattleAsync()
    {
        _state = BattleState.Start;

        // �L�����̐����ƕ`��
        _playerUnit.SetUp();
        _enemyUnit.SetUp();

        // HUD�̕`�� _playerUnit.Character��n���̂ł͖���
        _playerHud.SetData(_playerUnit.Character);// Player�f�[�^
        _enemyHud.SetHpData(_enemyUnit.Character);// �GHP�f�[�^
        _playerHud.ShowHpText(_playerUnit.Character);// HP�e�L�X�g
        _playerHud.ShowSpText(_playerUnit.Character);// SP�e�L�X�g

        // �p�l���̕`��
        _dialogBox.SetMoveNames(_playerUnit.Character.Moves); // �Z���e�L�X�g
        await _dialogBox.TypeDialogAsync($"{_enemyUnit.Character.Base.Name}�����ꂽ");
        await UniTask.Delay(TimeSpan.FromSeconds(0.7f), cancellationToken: token);

        ChooseFirstTurn();
    }
    /// <summary>
    /// �ǂ���̃X�s�[�h�����������肵�Đ�U��U�����߂�
    /// </summary>
    private void ChooseFirstTurn()
    {
        if (IsPlayerFastar())
        {
            DoAction();
        }
        else if (_playerUnit.Character.Speed == _enemyUnit.Character.Speed)
        {
            // �����_���Ń^�[�������߂�
            byte player = 0;
            byte enemy = 1;
            byte turn = (byte)UnityEngine.Random.Range(player, enemy+1);// CS0104�G���[�̈�UnityEngine.��t���܂���

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
    /// PlayerAction�Ɉڍs���邩
    /// </summary>
    private void DoAction()
    {
        _state = BattleState.DoAction;

        // �p�l���̕`��
        _dialogBox.EnableDoActionSelection(true);// �s�����邩�I������e�L�X�g�̕\��
        _dialogBox.TypeDialogAsync("���������H").Forget();
    }
    /// <summary>
    /// Player�̍s���I����ʂ�
    /// </summary>
    private void PlayerAction()
    {
        _state = BattleState.PlayerAction;

        // �p�l���̕`��
        _dialogBox.EnableDoActionSelection(false);// �s�����邩�I������e�L�X�g�̔�\���@������
        _dialogBox.EnableActionSelection(true);   // �s���I���e�L�X�g�̕\��
        _dialogBox.TypeDialogAsync("�ǂ�����H").Forget();
    }
    /// <summary>
    /// Player�̋Z�I����ʂ�
    /// </summary>
    private void PlayerMove()
    {
        _state = BattleState.PlayerMove;
        //_dialogBox.EnableActionSerection(false);
        _dialogBox.EnableBattleDialog(false);// �퓬���b�Z�[�W�p�l���E�e�L�X�g�̔�\��
        _dialogBox.EnableMoveSelector(true); // �Z�I���p�l���E�e�L�X�g�̕\��
    }
    /// <summary>
    /// �s���I����ʂ��L�����Z�����čs�����邩�I�������ʂɖ߂�
    /// </summary>
    private void CancelPlayerAction()
    {
        _dialogBox.EnableActionSelection(false);// �s���I���e�L�X�g�̔�\��
        DoAction();
    }
    /// <summary>
    /// �Z�I����ʂ��L�����Z�����čs���I����ʂɖ߂�
    /// </summary>
    private void CancelPlayerMove()
    {
        _dialogBox.EnableBattleDialog(true);// �퓬���b�Z�[�W�p�l���E�e�L�X�g�̕\��
        _dialogBox.EnableMoveSelector(false);
        PlayerAction();
    }

    /// <summary>
    /// �����鏈��
    /// </summary>
    private async UniTask PlayerEscapeMoveAsync()
    {
        _state = BattleState.Busy;
        // �����m���͓����񐔂ɋN������@1: 25�� 2: 50% 3: 75% 4:100%
        await _dialogBox.TypeDialogAsync($"{_playerUnit.Character.Base.Name}�͓����o����");

        byte baseEscapeChance = 25;
        runNum++;
        //�����@�G�����f���������� & ������������
        if (IsPlayerFastar() || 
            UnityEngine.Random.value * 100 >= runNum * baseEscapeChance)
        {
            _dialogBox.EnableDoActionSelection(false);// �s�����邩�I������e�L�X�g�̔�\��
            // �o�g���I������
            _iGameController.EndBattle();
        }
        else //���s
        {
            Debug.Log(runNum * baseEscapeChance + " " + UnityEngine.Random.value * 100);
            await _dialogBox.TypeDialogAsync($"�������A��荞�܂�Ă��܂���");
            // ����̃^�[��
            EnemyMoveAsync().Forget();
        }

    }
    /// <summary>
    /// Player�̕����f�������������ǂ���
    /// </summary>
    public bool IsPlayerFastar()=> _playerUnit.Character.Speed > _enemyUnit.Character.Speed;
    

    /// <summary>
    /// Player�̍s�������s
    /// </summary>
    public async UniTask PerformPlayerMoveAsync()
    {
        _state = BattleState.Busy;
        // �Z������
        Move move = _playerUnit.Character.Moves[_currentMove];

        // Sp����
        _playerUnit.Character.SpConsumption(move, _playerUnit.Character);// Sp�v�Z
        _playerHud.UpdateSp();                                           // SpBar���f
        _playerHud.ShowSpText(_playerUnit.Character);                    // SpText���f

        // �Z�g�p�e�L�X�g�\��
        await _dialogBox.TypeDialogAsync($"{_playerUnit.Character.Base.Name}��{move.Base.Name}��������");

        // Player�Z�g�p����
        MoveResoltDetails damageDetails = await move.Base.RunMoveResultAsync(_playerUnit, _enemyUnit);

        // Hp���f
        await _enemyHud.UpdateHpAsync();
        await _playerHud.UpdateHpAsync();
        _playerHud.ShowHpText(_playerUnit.Character);

        //�_���[�W���ʂ�\��
        await _dialogBox.TypeDialogAsync(damageDetails.MoveResult);

        // �_���[�W�ڍׂ��e�L�X�g�ŕ\��
        await ShowDamageDetailsAsync(damageDetails);

        if (damageDetails.Fainted)// �퓬�s�\
        {
            // �퓬�s�\�A�j���[�V����
            _enemyUnit.FaintAnimation();
            
            // �퓬�s�\�����b�Z�[�W
            await _dialogBox.TypeDialogAsync($"{_enemyUnit.Character.Base.Name}�͂����ꂽ");
            
            // �|�����G����o���l������
            _playerUnit.Character.HasExperience += _enemyUnit.Character.Experience;

            await _dialogBox.TypeDialogAsync
                ($"{_playerUnit.Character.Base.Name}�͌o���l{_enemyUnit.Character.Experience}�𓾂�");
            
            if (_playerUnit.Character.IsLevelUp())// ���ȏ�o���l�����܂��,���x�����オ��
            {
                // ���x���A�b�v
                _playerUnit.Character.Level++;
                // Hp,Sp�̉�
                _playerUnit.Character.Hp = _playerUnit.Character.MaxHp;
                _playerUnit.Character.Sp = _playerUnit.Character.MaxSp;

                // �f�[�^�̔��f
                _playerHud.UpdateLevel();
                _playerHud.SetHpData(_playerUnit.Character);
                _playerHud.UpdateSp();
                _playerHud.ShowHpText( _playerUnit.Character);
                _playerHud.ShowSpText( _playerUnit.Character);

                await _dialogBox.TypeDialogAsync
                    ($"{_playerUnit.Character.Base.Name}�̓��x��{_playerUnit.Character.Level}�ɂȂ����I");

                // ����̃��x���Ȃ�Z���o����
                Move learnedMove = _playerUnit.Character.LearnedMove();
                if ( learnedMove != null )
                {
                    await _dialogBox.TypeDialogAsync
                        ($"{_playerUnit.Character.Base.Name}��{learnedMove.Base.Name}�𓾂�");
                }
            }

            // �o�g���I������
            _iGameController.EndBattle();
        }
        else // �퓬�\
        {
            // �G�̍s��
            EnemyMoveAsync().Forget();
        }
    }
    /// <summary>
    /// �G�̍s��
    /// </summary>
    private async UniTask EnemyMoveAsync()
    {
        _state = BattleState.EnemyMove;

        // �Z������
        Move move = _enemyUnit.Character.GetRandomMove();
        // �Z�g�p�e�L�X�g�\��
        await _dialogBox.TypeDialogAsync($"{_enemyUnit.Character.Base.Name}��{move.Base.Name}��������");

        // Enemy�s������
        MoveResoltDetails damageDetails = await move.Base.RunMoveResultAsync(_enemyUnit, _playerUnit);

        // Hp���f
        await _playerHud.UpdateHpAsync();�@�@�@�@�@ // HpBar���f
        await _enemyHud.UpdateHpAsync();�@�@�@�@�@ // HpBar���f
        _playerHud.ShowHpText(_playerUnit.Character); // HpText���f

        //�_���[�W���ʂ�\��
        await _dialogBox.TypeDialogAsync(damageDetails.MoveResult);


        await ShowDamageDetailsAsync(damageDetails);// �_���[�W�ڍׂ��e�L�X�g�\��

        if (damageDetails.Fainted)// �퓬�s�\
        {
            // �퓬�s�\�A�j���[�V����
            _playerUnit.FaintAnimation();
            // �퓬�s�\�����b�Z�[�W
            await _dialogBox.TypeDialogAsync($"{_playerUnit.Character.Base.Name}�͂����ꂽ");

            // �o�g���I������

            //�Q�[���v���C�I��
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
Application.Quit();
#endif
        }
        else // �퓬�\
        {
            // Player�̍s����
            DoAction();
        }
    }

    /// <summary>
    /// �_���[�W�̏ڍׂ��e�L�X�g�ŕ\��
    /// </summary>
    private async UniTask ShowDamageDetailsAsync(MoveResoltDetails damageDetails)
    {
        int nomal = 1;

        if (damageDetails.Critical > nomal)
        {
            await _dialogBox.TypeDialogAsync($"�}���ɓ�������");
        }
        if (damageDetails.TypeEffectiveness > nomal)
        {
            await _dialogBox.TypeDialogAsync($"���ʂ̓o�c�O����");
        }
        else if (damageDetails.TypeEffectiveness < nomal)
        {
            await _dialogBox.TypeDialogAsync($"���ʂ͂��܂ЂƂ�");
        }
    }

    /// <summary>
    /// Update ��Ԃ��Ƃ̏���
    /// </summary>
    public void HandleUpdate()
    {
        if (_state == BattleState.DoAction)
        {
            // �s�����邩��I��
            HandleDoActionSelection();
        }
        else if (_state == BattleState.PlayerAction)
        {
            // �s���I��
            HandleActionSelection();
        }
        else if (_state == BattleState.PlayerMove)
        {
            // �Z�I��
            HandleMoveSelection();
        }
    }

//-----------------�R�}���h����----------------------------------------------------------------------
    /// <summary>
    /// DoAction�ł̍s������������
    /// </summary>
    private void HandleDoActionSelection()
    {
        // 0:���������@1:�ɂ���
        if (Button.Down())
        {
            if (_currentDoAct < DoActionSelect.Escape)// TODO: ActionSerection�̐����Q�Ƃ�����
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
        // �F�����Ăǂ����I�����Ă��邩������悤�ɂ���
        _dialogBox.UpdateDoActionSelection((int)_currentDoAct);

        if (Button.Accept())
        {
            AudioManager.Instance.PlaySE(0);//�I����
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
    /// PlayerAction�ł̍s������������
    /// </summary>
    private void HandleActionSelection()
    {
        // 0:��������, 1:�X�L��, 2:�ڂ�����, 3:�A�C�e��
        if (Button.Down())
        {
            if (_currentAct < ActionSelect.Item)// ActionSerection�̐����Q�Ƃ����ق����������ȁH
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

        // �F�����Ăǂ����I�����Ă��邩������悤�ɂ���
        _dialogBox.UpdateActionSelection((int)_currentAct);

        if (Button.Accept())
        {
            if (_currentAct == ActionSelect.Move)
            {
                AudioManager.Instance.PlaySE(0);//�I����
                PlayerMove();
            }
            else if (_currentAct == ActionSelect.Defense)
            {
                // �h�䂷��
                // �܂��������̂��߃L�����Z������
                AudioManager.Instance.PlaySE(1);//�L�����Z����
            }
            else if (_currentAct == ActionSelect.Item)
            {
                // �A�C�e���p�l��
                // �܂��������̂��߃L�����Z������
                AudioManager.Instance.PlaySE(1);//�L�����Z����
            }
        }
        else if (Button.Cancel())
        {
            AudioManager.Instance.PlaySE(1);//�L�����Z����
            CancelPlayerAction();
        }
    }
    /// <summary>
    /// PlayerMove�ł̍s������������
    /// </summary>
    private void HandleMoveSelection()
    {
  /*0�@1
    2�@3
    4�@5
    �Ƃ������тɂȂ��Ă���*/
        // �s��
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
        //���ς���
        if (Button.Down())
        {
            // ��ԉ��̗�ɂ��鎞�A���ɓ������Ȃ��悤��
            if (_currentMove < _playerUnit.Character.Moves.Count - lineNum)
            {
                _currentMove += lineNum;
            }
        }
        if (Button.Up())// ��̗��
        {
            // 1��ڂɂ��鎞�A��ɓ������Ȃ��悤�ɂ���
            if (_currentMove > lineNum - 1)
            {
                _currentMove -= lineNum;
            }
        }

        // �F�����Ăǂ����I�����Ă��邩������悤�ɂ���
        _dialogBox.UpdateMoveSelection(_currentMove, _playerUnit.Character.Moves[_currentMove]);

        if (Button.Accept())
        {
            // SP������Ă���Ή��L�̏��������s
            if (_playerUnit.Character.Moves[_currentMove].Sp <= _playerUnit.Character.Sp)
            {
                AudioManager.Instance.PlaySE(0);//�I����
                
                // �Z�I����UI�͔�\��
                _dialogBox.EnableMoveSelector(false);
                _dialogBox.EnableActionSelection(false);
                
                // ���b�Z�[�W�_�C�����O����
                _dialogBox.EnableBattleDialog(true);
                
                // �Z���菈��
                PerformPlayerMoveAsync().Forget();
            }
            else
            {
                // Sp������Ă��Ȃ��ꍇ�̏���
                AudioManager.Instance.PlaySE(1);//�L�����Z����
            }
            
        }
        else if (Button.Cancel())
        {
            AudioManager.Instance.PlaySE(1);//�L�����Z����
            CancelPlayerMove();
        }
    }
//-------------------------------------------------------------------------------------------------------
}
