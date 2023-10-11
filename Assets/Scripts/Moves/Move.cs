using UnityEngine;

// 技のマスターデータを持つ
// Charaが実際に使う時の技データ
public class Move
{
    public MoveBase Base {  get; set; }
    public int Sp { get; set; }
    public string Description { get; }
    public CharaType Direction { get; }//TODO: 適切な名前に変更が必要
    public Sprite Sprite { get; }

    // 初期設定 コンストラクター
    public Move(MoveBase cBase)
    {
        Base = cBase;
        Sp = cBase.Sp;
        Description = cBase.Description;
        Direction = cBase.Type;
        Sprite = cBase.Sprite;
    }
}