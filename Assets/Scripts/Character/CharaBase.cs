using System.Collections.Generic;
using System;
using UnityEngine;

// �L�����̃}�X�^�[�f�[�^ : �O������ύX���Ȃ��i�C���X�y�N�^�[�����ύX�\�j
[CreateAssetMenu(fileName = "CharaBase", menuName = "Custom/CharaScriptableObject")]
public class CharaBase : ScriptableObject
{
    // ���O�A�����A�^�C�v�A�X�e�[�^�X
    [SerializeField] private string _name;
    [TextArea]
    [SerializeField] private string _description;

    // �摜
    [SerializeField] private Sprite _rightSprite;
    [SerializeField] private Sprite _leftSprite;

    // �^�C�v
    [SerializeField] private CharaType _type;

    // �X�e�[�^�X
    [SerializeField] private int _maxHp;
    [SerializeField] private int _maxSp;
    [SerializeField] private int _attack;
    [SerializeField] private int _defense;
    [SerializeField] private int _spAttack;
    [SerializeField] private int _spDefense;
    [SerializeField] private int _speed;
    [SerializeField] private int _experience;

    // �o����Z�ꗗ
    [SerializeField] private List<LearableMove> _learnableMoves;

    // ���t�@�C������attack�̒l�̎擾�͏o���邪�ύX�͏o���Ȃ�
    public int MaxHp { get => _maxHp; }
    public int MaxSp { get => _maxSp; }
    public int Attack{ get => _attack; /*get { return _attack; } �Ɠ����Ӗ�*/}
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
// �o����Z : �ǂ̃��x���ŉ����o����̂�
[Serializable]
public class LearableMove
{
    [SerializeField] MoveBase _base;
    [SerializeField] int _level;

    public MoveBase Base { get => _base; }
    public int Level { get => _level; }
}
// �L�����̃^�C�v�i�����j
public enum CharaType
{
    Nomal,
    Fire,
    Water,
    Wood,
    Dark,
    Holy,
}
// �L�����̃X�e�[�^�X
public enum Stat
{
    Attack,
    Defence,
    SpAttack,
    SpDefense,
    Speed,
}
// �^�C�v�����}
public class TypeChart
{
    private const int c_usual = 1;
    private const float c_weak = 0.75f;
    private const float c_strong = 1.5f;

    static float[][] s_charts =
    {
        //�U���_�h��@     �m�[�}��    ��      ��       ��       ��       ��
        /*�m*/ new float[]{c_usual, c_usual, c_usual, c_usual, c_usual, c_usual},
        /*��*/ new float[]{c_usual, c_usual, c_weak ,c_strong, c_usual, c_usual},
        /*��*/ new float[]{c_usual, c_strong, c_usual, c_weak , c_usual, c_usual},
        /*��*/ new float[]{c_usual, c_weak ,c_strong, c_usual, c_usual, c_usual},
        /*��*/ new float[]{c_usual, c_usual, c_usual, c_usual, c_usual,c_strong},
        /*��*/ new float[]{c_usual, c_usual, c_usual, c_usual,c_strong, c_usual},
    };
    /// <summary>
    /// �^�C�v�����̌��ʂ�Ԃ�
    /// </summary>
    public static float GetEffectiveness(CharaType attackType, CharaType defenseType)
    {
        int row = (int)attackType;
        int col = (int)defenseType;
        return s_charts[row][col];
    }
}
