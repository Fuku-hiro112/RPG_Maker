using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu]
public class HealMovebase : MoveBase
{
    [SerializeField] private int _healPoint;

    public int HealPoint { get => _healPoint; }

    public override async UniTask<MoveResoltDetails> RunMoveResultAsync(BattleUnit moveUnit, BattleUnit waitUnit)
    {
        // �񕜃G�t�F�N�g
        moveUnit.EfectAnimation.MoveHealEfect(Sprite, moveUnit);

        MoveResoltDetails damageDetails = new MoveResoltDetails();
        damageDetails.EfectValue = moveUnit.Character.HealMove(_healPoint);
        damageDetails.MoveResult = $"{moveUnit.Character.Base.Name}��{damageDetails.EfectValue}��";
        return damageDetails;
    }
}
