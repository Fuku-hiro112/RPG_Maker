using UnityEngine;
// ƒLƒƒƒ‰‚Ìî•ñ‚ðŠÇ—‚µ‚»‚Ìî•ñ‚ð“n‚·
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
