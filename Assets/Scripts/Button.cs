using UnityEngine;

public class Button
{
    // ����,�L�����Z��,�㉺���E�@���ꂼ��̃{�^���̊��蓖�Ă�������Ă���
    public static bool Accept() => Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return);
    public static bool Cancel() => Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Escape);
    public static bool Up()     => Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
    public static bool Down()   => Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);
    public static bool Left()   => Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
    public static bool Right()  => Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);
}
