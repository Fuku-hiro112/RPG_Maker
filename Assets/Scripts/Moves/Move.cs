using UnityEngine;

// �Z�̃}�X�^�[�f�[�^������
// Chara�����ۂɎg�����̋Z�f�[�^
public class Move
{
    public MoveBase Base {  get; set; }
    public int Sp { get; set; }
    public string Description { get; }
    public CharaType Direction { get; }//TODO: �K�؂Ȗ��O�ɕύX���K�v
    public Sprite Sprite { get; }

    // �����ݒ� �R���X�g���N�^�[
    public Move(MoveBase cBase)
    {
        Base = cBase;
        Sp = cBase.Sp;
        Description = cBase.Description;
        Direction = cBase.Type;
        Sprite = cBase.Sprite;
    }
}