using UnityEngine;
using Cysharp.Threading.Tasks;

// 技のマスターデータ
public class MoveBase : ScriptableObject
{
    // 名前
    [SerializeField] private string _name;
    
    [TextArea]// 技の説明
    [SerializeField] private string _description;
    // 技のタイプ
    [SerializeField] private CharaType _type;
    // 命中率
    [SerializeField] private int _accuracy;
    [SerializeField] private int _sp;

    // カテゴリー　物理、特殊、ステータス変化
    [SerializeField] private MoveCategory _category;
    [SerializeField] private Sprite _sprite;
    // 参照用プロパティ
    public string Name { get => _name;}
    public string Description { get => _description;}
    public CharaType Type { get => _type; }
    public int Accuracy { get => _accuracy; }
    public int Sp { get => _sp; }
    public MoveCategory Category { get => _category; }
    public Sprite Sprite { get => _sprite;}

    public virtual async UniTask<MoveResoltDetails> RunMoveResultAsync( BattleUnit moveUnit, BattleUnit waitUnit)
    {
        MoveResoltDetails damageDetails = null;
        return damageDetails;
    }
}
public enum MoveCategory
{
    Physical,
    Special,
    Stat
}