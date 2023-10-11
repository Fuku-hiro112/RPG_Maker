using UnityEngine;

public class GameClear : MonoBehaviour
{
    [SerializeField] private GameObject _goalCanvas;
    public void HandleUpdate(GameState state)
    {
        // ステートがゲームクリア以外だったらCanvasを表示させない
        if (state == GameState.GameClear)
        {
            _goalCanvas.SetActive(true);
        }
        if (Button.Accept())
        {
            //ゲームプレイ終了
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
Application.Quit();
#endif
        }
    }
}
