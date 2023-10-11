using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

// �������̂��߁A�܂�������
[CreateAssetMenu]
public class StatMoveBase : MoveBase
{
    // �X�e�[�^�X�ω��̃^�[�Q�b�g
    [SerializeField] private MoveTarget _target;
    // �ǂ̃X�e�[�^�X���ǂꂭ�炢�ω������邩���X�g
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
                damageDetails.MoveResult = $"{moveUnit.name}��{_effects}��{moveUnit.Character.StatBoosts}�{�ɂȂ���";// TODO : �C�����K�v
            }
            else
            {
                waitUnit.Character.ApplyBoosts(_effects.Boosts);
                damageDetails.MoveResult = $"{waitUnit.name}��{waitUnit.Character.Stats}��{moveUnit.Character}�{�ɂȂ���";// TODO : List�Ȃ̂Ń��[�v����C�����K�v
            }
        }*/
        return damageDetails;// TODO : �����̌��ʂ�����ꍇ�����邽�߁A�e�L�X�g��z��ɂ���
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
    // �X�e�[�^�X
    public Stat Stat;
    // �㏸�l
    public int Boost;
}
