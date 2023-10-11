using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu]
public class AttackMoveBase : MoveBase
{
    [SerializeField] private int _power;

    public int Power { get => _power; }

    public override async UniTask<MoveResoltDetails> RunMoveResultAsync(BattleUnit attacker, BattleUnit defender)
    {
        // �A�j���[�V����
        await attacker.AttackAnimationAsync();// Player�U��
        defender.HitAnimation();               // Enemy��e

        // �G�t�F�N�g
        // attacker����摜��defender�ɔ�΂�
        switch (Category)
        {
            case MoveCategory.Physical:
                attacker.EfectAnimation.MovePhysicalAttackEfect(Sprite, defender);
                break;
            case MoveCategory.Special:
                attacker.EfectAnimation.MoveSpecialAttackEfect(Sprite, attacker, defender);
                break;
            default: 
                break;
        }

        // Enemy�_���[�W�v�Z
        MoveResoltDetails damageDetails = defender.Character.TakeDamege(this, _power, attacker.Character);
        damageDetails.MoveResult = $"{defender.Character.Base.Name}��{damageDetails.EfectValue}�̃_���[�W";

        return damageDetails;
    }
}
