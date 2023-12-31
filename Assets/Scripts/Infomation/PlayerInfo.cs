using UnityEngine;
// キャラの情報を管理しその情報を渡す
public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private CharaBase _base;
    [SerializeField] private int _level;

    private Character _character;
    public CharaBase Base { get => _base; }
    public int Level { get => _level; }

    private void Awake()
    {
        _character = new Character();
        _character.Initialize(Base, Level);
    }
    public Character Initialize()
    {
        return _character;
    }
}
