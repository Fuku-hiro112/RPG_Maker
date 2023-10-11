using System.Collections.Generic;
using UnityEngine;

public class Character
{
    // �x�[�X�ƂȂ�f�[�^
    public CharaBase Base { get; set; }
    public int Level { get; set; }
    public int HasExperience { set; get; }
    public int Hp { get; set; }
    public int Sp { get; set; }

    public int Experience { get; private set; }
    public int MaxHp { get; private set; }
    public int MaxSp { get; private set; }
    
    //level�ɉ������X�e�[�^�X��Ԃ��v���p�e�B
    public int Attack
    {
        get { return Stats[Stat.Attack]; }
    }
    public int Defense
    {
        get { return Stats[Stat.Defence]; }
    }
    public int SpAttack
    {
        get { return Stats[Stat.SpAttack]; }
    }
    public int SpDefense
    {
        get { return Stats[Stat.SpDefense]; }
    }
    public int Speed
    {
        get { return Stats[Stat.Speed]; }
    }

    // ���x��2�ɂȂ邽�߂ɕK�v�Ȍo���l
    private const int c_needExperience = 10;
    private int _needExperience;

    // �N���e�B�J�����E�{���@���[�J���ϐ���
    private const float c_criticalRate = 0.03125f;// 1/32
    private const float c_criticalMultiplier = 2.5f;

    // �g����Z
    public List<Move> Moves { get; set; }
    // �X�e�[�^�X�ƒǉ��X�e�[�^�X
    public Dictionary<Stat, int> Stats { get; set; }

    // �L�����̏����� 
    public void Initialize(CharaBase cBase, int cLevel)
    {
        Base = cBase;
        Level = cLevel;

        Moves = new List<Move>();
        // �g����Z�̐ݒ�:�o����Z�̃��x���ȏ�Ȃ�AMoves�ɒǉ�
        foreach (LearableMove learnableMove in cBase.LearnableMoves)
        {
            if (Level >= learnableMove.Level)
            {
                // �Z���o����
                Moves.Add(new Move(learnableMove.Base));
            }
        }

        UpdateStats();
        Hp = MaxHp;
        Sp = MaxSp;
    }

    /// <summary>
    /// �X�e�[�^�X�̍X�V
    /// </summary>
    private void UpdateStats()
    {
        Stats = new Dictionary<Stat, int>
        {
            { Stat.Attack, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5 },
            { Stat.Defence, Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5 },
            { Stat.SpAttack, Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5 },
            { Stat.SpDefense, Mathf.FloorToInt((Base.SpDefense * Level) / 100f) + 5 },
            { Stat.Speed, Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5 }
        };
        Experience = Mathf.FloorToInt((Base.Experience * Level) / 100f);
        MaxHp = Mathf.FloorToInt((Base.MaxHp * Level) / 100f) + 10;
        MaxSp = Mathf.FloorToInt((Base.MaxSp * Level) / 100f) + 5;
    }

    /// <summary>
    /// �_���[�W�v�Z����
    /// </summary><returns>
    /// Fainted: �퓬�s�\���ǂ���
    /// Critical: �N���e�B�J���{��
    /// TypeEffectiveness: �^�C�v�{��
    /// DamageValue: �_���[�W�l
    /// ��4��Ԃ�
    /// </returns>
    public MoveResoltDetails TakeDamege(AttackMoveBase move,int movePower, Character attacker )
    {
        float critical = 1;
        // _criticalRate�ŃN���e�B�J���������߂Ă���
        if (Random.value <= c_criticalRate) critical = c_criticalMultiplier;
        
        // �^�C�v�����m�F
        float type = TypeChart.GetEffectiveness(move.Type, Base.Type);

        float attack = attacker.Attack;
        float defense = Defense;
        // ����Z�̏ꍇ
        if (move.Category == MoveCategory.Special)
        {
            attack = attacker.SpAttack;
            defense = SpDefense;
        }

        // �_���[�W�v�Z��
        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float levelAttack = (2 * attacker.Level + 10) / 250f;
        float damege = levelAttack * movePower * ((float)attack / defense) + 2;
        int resultDamage = Mathf.FloorToInt(damege * modifiers);
        
        MoveResoltDetails damageDetails = new MoveResoltDetails 
        {
            // DamageDetails�̏�����
            Critical = critical,
            TypeEffectiveness = type,
            EfectValue = resultDamage,
        };

        Hp -= resultDamage;
        if (Hp <= 0)// �퓬�s�\���ǂ����𔻒�
        {
            Hp = 0;
            damageDetails.Fainted = true;
        }
        return damageDetails;
    }
    /// <summary>
    /// Hp���񕜂���
    /// </summary>
    public int HealMove(int healPoint)
    {
        int beforeHp = Hp;
        Hp = Mathf.Clamp(Hp + healPoint, 0, MaxHp);
        return Hp - beforeHp;// ���ۂɉ񕜂����l��Ԃ�
    }
    /// <summary>
    /// ����SP���v�Z
    /// </summary>
    public void SpConsumption(Move move, Character attacker)
    {
        Sp -= move.Base.Sp;
    }

    /// <summary>
    /// �o���Ă���Z�̒����烉���_���łP�I��
    /// </summary>
    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }

    /// <summary>
    /// ���x���A�b�v�������ǂ���
    /// </summary>
    /// <returns>�����Ԃ�̂������������ǂ��R�����g�ɂȂ�</returns>
    public bool IsLevelUp()
    {
        _needExperience = (int)Mathf.Pow(1.2f, Level-1) * c_needExperience;
        Debug.Log(_needExperience);
        if (HasExperience >= _needExperience)
        {
            HasExperience -= (int)_needExperience;

            // �X�e�[�^�X�̕ύX
            UpdateStats();
            /*
            // HPSP��S�񕜂���
            Hp = MaxHp;
            Sp = MaxSp;
            Level++;// ���x���A�b�v���Ă�
            Debug.Log(MaxHp);
            */
            return true;
        }
        return false;
    }

    /// <summary>
    /// �����̃��x���ȉ��̋Z���o����
    /// </summary>
    /// <returns>
    /// �o����Z��Ԃ�
    /// </returns>
    public Move LearnedMove()
    {
        foreach (LearableMove learnableMove in Base.LearnableMoves)
        {
            if (Level >= learnableMove.Level && !Moves.Exists(move => move.Base == learnableMove.Base))
            {
                Move move = new Move(learnableMove.Base);
                Moves.Add(move);
                return move;
            }
        }
        return null;
    }
}

public class MoveResoltDetails
{
    public int EfectValue { get; set; }         // ���ʒl
    public float TypeEffectiveness { get; set; } // �^�C�v����
    public float Critical { get; set; }          // �N���e�B�J���{��
    public string MoveResult { get; set; }       //�Z�̌��ʂ̃e�L�X�g
    
    private bool _fainted = false;
    public bool Fainted                         // �퓬�s�\���ǂ���
    { 
        get => _fainted; 
        set => _fainted = value;
    }        
}