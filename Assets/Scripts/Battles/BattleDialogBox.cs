using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    // ����:dialog��Text���擾���āA�ύX����
    [SerializeField] private int _letterPerSecond; // 1�����ӂ�̎���
    [SerializeField] private Text _dialogText;
    
    [SerializeField] private GameObject _battleDialog;
    [SerializeField] private GameObject _moveGuide;
    [SerializeField] private GameObject _moveSerector;
    [SerializeField] private GameObject _doActionSerection;
    [SerializeField] private GameObject _actionSerection;
    [SerializeField] private Text _descriptionText;

    [SerializeField] private List<Text> _doActionTexts;
    [SerializeField] private List<Text> _actionTexts;
    [SerializeField] private List<Text> _moveTexts;

    private CancellationToken token;
    private void Start()
    {
        // this�t���Ȃ��ƃG���[���o�܂�
        token = this.GetCancellationTokenOnDestroy();
    }

    /// <summary>
    /// Text��ύX���邽�߂̊֐�
    /// </summary>
    public void SetDialog(string dialog)
    {
        _dialogText.text = dialog;
    }

    /// <summary>
    /// �^�C�v�`���i�P�����Âj�ŕ�����\������
    /// </summary>
    public async UniTask TypeDialogAsync(string dialog)
    {
        _dialogText.text = "";
        foreach (char letter in dialog)
        {
            AudioManager.Instance.PlaySE(5);//�I����
            _dialogText.text += letter;
            await UniTask.Delay(TimeSpan.FromSeconds(1f / _letterPerSecond), cancellationToken:token); 
        }
        await UniTask.Delay(TimeSpan.FromSeconds(0.7f), cancellationToken:token);
    }

    /// <summary>
    /// BattleDialog�̕\���Ǘ�
    /// </summary>
    public void EnableBattleDialog(bool enabled)
    {
        _battleDialog.SetActive(enabled);
    }
    /// <summary>
    /// DoActionSerector�̕\���Ǘ�
    /// </summary>
    public void EnableDoActionSelection(bool enabled)
    {
        _doActionSerection.SetActive(enabled);
    }
    /// <summary>
    /// ActionSerector�̕\���Ǘ�
    /// </summary>
    public void EnableActionSelection(bool enabled)
    {
        _actionSerection.SetActive(enabled);
    }
    

    /// <summary>
    /// MoveSerector�̕\���Ǘ�
    /// </summary>
    public void EnableMoveSelector(bool enabled)
    {
        _moveGuide.SetActive(enabled);
        _moveSerector.SetActive(enabled);
    }

    /// <summary>
    /// �I�𒆂̃A�N�V�����̐F��ς���
    /// </summary>
    public void UpdateDoActionSelection(int selectAction)
    {
        // selectAction�� i �̎��� actionTexts[i] �̐F�����F�ɂ���. ����ȊO��

        for (int i = 0; i < _doActionTexts.Count; i++)
        {
            if (selectAction == i)
            {
                _doActionTexts[i].color = Color.yellow;
            }
            else
            {
                _doActionTexts[i].color = Color.white;
            }
        }
    }
    /// <summary>
    /// �I�𒆂̃A�N�V�����̐F��ς���
    /// </summary>
    public void UpdateActionSelection(int selectAction)
    {
        // selectAction�� i �̎��� actionTexts[i] �̐F�����F�ɂ���. ����ȊO��

        for (int i = 0; i < _actionTexts.Count; i++)
        {
            if (selectAction == i)
            {
                _actionTexts[i].color = Color.yellow;
            }
            else
            {
                _actionTexts[i].color = Color.white;
            }
        }
    }
    /// <summary>
    /// �I�𒆂̋Z�̐F��ς���
    /// </summary>
    public void UpdateMoveSelection(int selectAction, Move move)
    {
        // moveAction�� i �̎��� moveTexts[i] �̐F�����F�ɂ���. ����ȊO��

        for (int i = 0; i < _moveTexts.Count; i++)
        {
            if (selectAction == i)
            {
                _moveTexts[i].color = Color.yellow;
            }
            else
            {
                _moveTexts[i].color = Color.white;
            }
        }
        // TODO : ��������{��ɂ�����
        _descriptionText.text = $"����:{move.Direction}  {move.Description} ����Sp:{move.Sp}";
    }

    /// <summary>
    /// �Z���̔��f
    /// </summary>
    public void SetMoveNames(List<Move> moves)
    {
        for (int i=0; i < _moveTexts.Count; i++)
        {
            //�o���Ă��鐔�������f
            if (i < moves.Count)
            {
                _moveTexts[i].text = moves[i].Base.Name;
            }
            else
            {
                _moveTexts[i].text = " ";
            }
        }
    }
}
