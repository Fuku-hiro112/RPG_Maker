using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu]
public class AttackMoveBase : MoveBase
{
    [SerializeField] private int _power;

    public int Power { get => _power; }

    public override async UniTask<MoveResoltDetails> RunMoveResultAsync(BattleUnit attacker, BattleUnit defender)
    {
        // アニメーション
        await attacker.AttackAnimationAsync();// Player攻撃
        defender.HitAnimation();               // Enemy被弾

        // エフェクト
        // attackerから画像をdefenderに飛ばす
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

        // Enemyダメージ計算
        MoveResoltDetails damageDetails = defender.Character.TakeDamege(this, _power, attacker.Character);
        damageDetails.MoveResult = $"{defender.Character.Base.Name}は{damageDetails.EfectValue}のダメージ";

        return damageDetails;
    }
}
