using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    // 役割:dialogのTextを取得して、変更する
    [SerializeField] private int _letterPerSecond; // 1文字辺りの時間
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
        // this付けないとエラーが出ます
        token = this.GetCancellationTokenOnDestroy();
    }

    /// <summary>
    /// Textを変更するための関数
    /// </summary>
    public void SetDialog(string dialog)
    {
        _dialogText.text = dialog;
    }

    /// <summary>
    /// タイプ形式（１文字づつ）で文字を表示する
    /// </summary>
    public async UniTask TypeDialogAsync(string dialog)
    {
        _dialogText.text = "";
        foreach (char letter in dialog)
        {
            AudioManager.Instance.PlaySE(5);//選択音
            _dialogText.text += letter;
            await UniTask.Delay(TimeSpan.FromSeconds(1f / _letterPerSecond), cancellationToken:token); 
        }
        await UniTask.Delay(TimeSpan.FromSeconds(0.7f), cancellationToken:token);
    }

    /// <summary>
    /// BattleDialogの表示管理
    /// </summary>
    public void EnableBattleDialog(bool enabled)
    {
        _battleDialog.SetActive(enabled);
    }
    /// <summary>
    /// DoActionSerectorの表示管理
    /// </summary>
    public void EnableDoActionSelection(bool enabled)
    {
        _doActionSerection.SetActive(enabled);
    }
    /// <summary>
    /// ActionSerectorの表示管理
    /// </summary>
    public void EnableActionSelection(bool enabled)
    {
        _actionSerection.SetActive(enabled);
    }
    

    /// <summary>
    /// MoveSerectorの表示管理
    /// </summary>
    public void EnableMoveSelector(bool enabled)
    {
        _moveGuide.SetActive(enabled);
        _moveSerector.SetActive(enabled);
    }

    /// <summary>
    /// 選択中のアクションの色を変える
    /// </summary>
    public void UpdateDoActionSelection(int selectAction)
    {
        // selectActionが i の時は actionTexts[i] の色を黄色にする. それ以外を白

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
    /// 選択中のアクションの色を変える
    /// </summary>
    public void UpdateActionSelection(int selectAction)
    {
        // selectActionが i の時は actionTexts[i] の色を黄色にする. それ以外を白

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
    /// 選択中の技の色を変える
    /// </summary>
    public void UpdateMoveSelection(int selectAction, Move move)
    {
        // moveActionが i の時は moveTexts[i] の色を黄色にする. それ以外を白

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
        // TODO : 属性を日本語にしたい
        _descriptionText.text = $"属性:{move.Direction}  {move.Description} 消費Sp:{move.Sp}";
    }

    /// <summary>
    /// 技名の反映
    /// </summary>
    public void SetMoveNames(List<Move> moves)
    {
        for (int i=0; i < _moveTexts.Count; i++)
        {
            //覚えている数だけ反映
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
