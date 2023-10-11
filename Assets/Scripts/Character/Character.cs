using System.Collections.Generic;
using UnityEngine;

public class Character
{
    // ベースとなるデータ
    public CharaBase Base { get; set; }
    public int Level { get; set; }
    public int HasExperience { set; get; }
    public int Hp { get; set; }
    public int Sp { get; set; }

    public int Experience { get; private set; }
    public int MaxHp { get; private set; }
    public int MaxSp { get; private set; }
    
    //levelに応じたステータスを返すプロパティ
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

    // レベル2になるために必要な経験値
    private const int c_needExperience = 10;
    private int _needExperience;

    // クリティカル率・倍率　ローカル変数に
    private const float c_criticalRate = 0.03125f;// 1/32
    private const float c_criticalMultiplier = 2.5f;

    // 使える技
    public List<Move> Moves { get; set; }
    // ステータスと追加ステータス
    public Dictionary<Stat, int> Stats { get; set; }

    // キャラの初期化 
    public void Initialize(CharaBase cBase, int cLevel)
    {
        Base = cBase;
        Level = cLevel;

        Moves = new List<Move>();
        // 使える技の設定:覚える技のレベル以上なら、Movesに追加
        foreach (LearableMove learnableMove in cBase.LearnableMoves)
        {
            if (Level >= learnableMove.Level)
            {
                // 技を覚える
                Moves.Add(new Move(learnableMove.Base));
            }
        }

        UpdateStats();
        Hp = MaxHp;
        Sp = MaxSp;
    }

    /// <summary>
    /// ステータスの更新
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
    /// ダメージ計算処理
    /// </summary><returns>
    /// Fainted: 戦闘不能かどうか
    /// Critical: クリティカル倍率
    /// TypeEffectiveness: タイプ倍率
    /// DamageValue: ダメージ値
    /// の4つを返す
    /// </returns>
    public MoveResoltDetails TakeDamege(AttackMoveBase move,int movePower, Character attacker )
    {
        float critical = 1;
        // _criticalRateでクリティカル率を決めている
        if (Random.value <= c_criticalRate) critical = c_criticalMultiplier;
        
        // タイプ相性確認
        float type = TypeChart.GetEffectiveness(move.Type, Base.Type);

        float attack = attacker.Attack;
        float defense = Defense;
        // 特殊技の場合
        if (move.Category == MoveCategory.Special)
        {
            attack = attacker.SpAttack;
            defense = SpDefense;
        }

        // ダメージ計算式
        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float levelAttack = (2 * attacker.Level + 10) / 250f;
        float damege = levelAttack * movePower * ((float)attack / defense) + 2;
        int resultDamage = Mathf.FloorToInt(damege * modifiers);
        
        MoveResoltDetails damageDetails = new MoveResoltDetails 
        {
            // DamageDetailsの初期化
            Critical = critical,
            TypeEffectiveness = type,
            EfectValue = resultDamage,
        };

        Hp -= resultDamage;
        if (Hp <= 0)// 戦闘不能かどうかを判定
        {
            Hp = 0;
            damageDetails.Fainted = true;
        }
        return damageDetails;
    }
    /// <summary>
    /// Hpを回復する
    /// </summary>
    public int HealMove(int healPoint)
    {
        int beforeHp = Hp;
        Hp = Mathf.Clamp(Hp + healPoint, 0, MaxHp);
        return Hp - beforeHp;// 実際に回復した値を返す
    }
    /// <summary>
    /// 消費SPを計算
    /// </summary>
    public void SpConsumption(Move move, Character attacker)
    {
        Sp -= move.Base.Sp;
    }

    /// <summary>
    /// 覚えている技の中からランダムで１つ選ぶ
    /// </summary>
    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }

    /// <summary>
    /// レベルアップしたかどうか
    /// </summary>
    /// <returns>何が返るのか書いた方が良いコメントになる</returns>
    public bool IsLevelUp()
    {
        _needExperience = (int)Mathf.Pow(1.2f, Level-1) * c_needExperience;
        Debug.Log(_needExperience);
        if (HasExperience >= _needExperience)
        {
            HasExperience -= (int)_needExperience;

            // ステータスの変更
            UpdateStats();
            /*
            // HPSPを全回復する
            Hp = MaxHp;
            Sp = MaxSp;
            Level++;// レベルアップしてる
            Debug.Log(MaxHp);
            */
            return true;
        }
        return false;
    }

    /// <summary>
    /// 自分のレベル以下の技を覚える
    /// </summary>
    /// <returns>
    /// 覚える技を返す
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
    public int EfectValue { get; set; }         // 効果値
    public float TypeEffectiveness { get; set; } // タイプ相性
    public float Critical { get; set; }          // クリティカル倍率
    public string MoveResult { get; set; }       //技の結果のテキスト
    
    private bool _fainted = false;
    public bool Fainted                         // 戦闘不能かどうか
    { 
        get => _fainted; 
        set => _fainted = value;
    }        
}