using UnityEngine;

public class GameClear : MonoBehaviour
{
    [SerializeField] private GameObject _goalCanvas;
    public void HandleUpdate(GameState state)
    {
        // �X�e�[�g���Q�[���N���A�ȊO��������Canvas��\�������Ȃ�
        if (state == GameState.GameClear)
        {
            _goalCanvas.SetActive(true);
        }
        if (Button.Accept())
        {
            //�Q�[���v���C�I��
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
Application.Quit();
#endif
        }
    }
}
