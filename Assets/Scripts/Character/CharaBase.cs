using System.Collections.Generic;
using System;
using UnityEngine;

// キャラのマスターデータ : 外部から変更しない（インスペクターだけ変更可能）
[CreateAssetMenu(fileName = "CharaBase", menuName = "Custom/CharaScriptableObject")]
public class CharaBase : ScriptableObject
{
    // 名前、説明、タイプ、ステータス
    [SerializeField] private string _name;
    [TextArea]
    [SerializeField] private string _description;

    // 画像
    [SerializeField] private Sprite _rightSprite;
    [SerializeField] private Sprite _leftSprite;

    // タイプ
    [SerializeField] private CharaType _type;

    // ステータス
    [SerializeField] private int _maxHp;
    [SerializeField] private int _maxSp;
    [SerializeField] private int _attack;
    [SerializeField] private int _defense;
    [SerializeField] private int _spAttack;
    [SerializeField] private int _spDefense;
    [SerializeField] private int _speed;
    [SerializeField] private int _experience;

    // 覚える技一覧
    [SerializeField] private List<LearableMove> _learnableMoves;

    // 他ファイルからattackの値の取得は出来るが変更は出来ない
    public int MaxHp { get => _maxHp; }
    public int MaxSp { get => _maxSp; }
    public int Attack{ get => _attack; /*get { return _attack; } と同じ意味*/}
    public int Defense { get => _defense; }
    public int SpAttack { get => _spAttack; }
    public int SpDefense { get => _spDefense; }
    public int Speed { get => _speed;}
    public int Experience { get => _experience; }
    public List<LearableMove> LearnableMoves { get => _learnableMoves; }
    public string Name { get => _name; }
    public string Description { get => _description; }
    public Sprite RightSprite { get => _rightSprite; }
    public Sprite LeftSprite { get => _leftSprite; }
    public CharaType Type { get => _type; }
}
// 覚える技 : どのレベルで何を覚えるのか
[Serializable]
public class LearableMove
{
    [SerializeField] MoveBase _base;
    [SerializeField] int _level;

    public MoveBase Base { get => _base; }
    public int Level { get => _level; }
}
// キャラのタイプ（属性）
public enum CharaType
{
    Nomal,
    Fire,
    Water,
    Wood,
    Dark,
    Holy,
}
// キャラのステータス
public enum Stat
{
    Attack,
    Defence,
    SpAttack,
    SpDefense,
    Speed,
}
// タイプ相性図
public class TypeChart
{
    private const int c_usual = 1;
    private const float c_weak = 0.75f;
    private const float c_strong = 1.5f;

    static float[][] s_charts =
    {
        //攻撃＼防御　     ノーマル    火      水       木       闇       光
        /*ノ*/ new float[]{c_usual, c_usual, c_usual, c_usual, c_usual, c_usual},
        /*火*/ new float[]{c_usual, c_usual, c_weak ,c_strong, c_usual, c_usual},
        /*水*/ new float[]{c_usual, c_strong, c_usual, c_weak , c_usual, c_usual},
        /*木*/ new float[]{c_usual, c_weak ,c_strong, c_usual, c_usual, c_usual},
        /*闇*/ new float[]{c_usual, c_usual, c_usual, c_usual, c_usual,c_strong},
        /*光*/ new float[]{c_usual, c_usual, c_usual, c_usual,c_strong, c_usual},
    };
    /// <summary>
    /// タイプ相性の結果を返す
    /// </summary>
    public static float GetEffectiveness(CharaType attackType, CharaType defenseType)
    {
        int row = (int)attackType;
        int col = (int)defenseType;
        return s_charts[row][col];
    }
}
