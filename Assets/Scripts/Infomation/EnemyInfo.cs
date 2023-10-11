using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo : MonoBehaviour
{
    // Listで何体も追加出来るように
    [SerializeField] private List<EnemyBase> _bases;
    public List<EnemyBase> Bases { get => _bases;}
    private Character _character;

    public Character Initialize()
    {
        _character = new Character();
        int enemeyNum = Random.Range(0, Bases.Count);
        EnemyBase enemyBase = Bases[enemeyNum];
        _character.Initialize(enemyBase.Base, enemyBase.Level);
        return _character;
    }
}
// 覚える技 : どのレベルで何を覚えるのか
[System.Serializable]
public class EnemyBase
{
    [SerializeField] CharaBase _base;
    [SerializeField] int _level;

    public CharaBase Base { get => _base; }
    public int Level { get => _level; }
}
