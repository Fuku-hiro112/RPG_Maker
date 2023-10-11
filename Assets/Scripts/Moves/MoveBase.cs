using UnityEngine;
using Cysharp.Threading.Tasks;

// �Z�̃}�X�^�[�f�[�^
public class MoveBase : ScriptableObject
{
    // ���O
    [SerializeField] private string _name;
    
    [TextArea]// �Z�̐���
    [SerializeField] private string _description;
    // �Z�̃^�C�v
    [SerializeField] private CharaType _type;
    // ������
    [SerializeField] private int _accuracy;
    [SerializeField] private int _sp;

    // �J�e�S���[�@�����A����A�X�e�[�^�X�ω�
    [SerializeField] private MoveCategory _category;
    [SerializeField] private Sprite _sprite;
    // �Q�Ɨp�v���p�e�B
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