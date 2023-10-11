using UnityEngine;
using UnityEngine.UI;

public class SpBar : MonoBehaviour
{
    [SerializeField] private GameObject _skillPoint;
    [SerializeField] private Text _playerSpText;

    /// <summary>
    /// SpBerのセットアップ
    /// </summary>
    public void SetSp(float sp)
    {
        _skillPoint.transform.localScale = new Vector3(sp, 1, 1);
    }
    /// <summary>
    /// SpTextに数値を表示
    /// </summary>
    public void ShowPlayerSpText(int currentSp)
    {
        _playerSpText.text = currentSp.ToString();
    }
}
