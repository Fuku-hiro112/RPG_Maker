using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

// 未完成のため、まだ未実装
[CreateAssetMenu]
public class StatMoveBase : MoveBase
{
    // ステータス変化のターゲット
    [SerializeField] private MoveTarget _target;
    // どのステータスをどれくらい変化させるかリスト
    [SerializeField] private MoveEffects _effects;

    public MoveTarget Target { get => _target; }
    public MoveEffects Effects { get => _effects; }

    public override async UniTask<MoveResoltDetails> RunMoveResultAsync(BattleUnit moveUnit, BattleUnit waitUnit)
    {
        MoveResoltDetails damageDetails = new MoveResoltDetails();
        /*if (_effects.Boosts != null)
        {
            if (_target == MoveTarget.Self)
            {
                moveUnit.Character.ApplyBoosts(_effects.Boosts);
                damageDetails.MoveResult = $"{moveUnit.name}の{_effects}が{moveUnit.Character.StatBoosts}倍になった";// TODO : 修正が必要
            }
            else
            {
                waitUnit.Character.ApplyBoosts(_effects.Boosts);
                damageDetails.MoveResult = $"{waitUnit.name}の{waitUnit.Character.Stats}が{moveUnit.Character}倍になった";// TODO : Listなのでループする修正が必要
            }
        }*/
        return damageDetails;// TODO : 複数の効果がある場合があるため、テキストを配列にする
    }
}
public enum MoveTarget
{
    Foe,
    Self
}
[Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoost> _boosts;
    public List<StatBoost> Boosts { get => _boosts; }
}
[Serializable]
public class StatBoost
{
    // ステータス
    public Stat Stat;
    // 上昇値
    public int Boost;
}
