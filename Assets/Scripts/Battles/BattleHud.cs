using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] private Text _nameText;
    [SerializeField] private Text _levelText;
    [SerializeField] private HpBar _hpBar;
    [SerializeField] private SpBar _spBar;

    private Character _character;

    /// <summary>
    /// データのセットアップ
    /// </summary>
    public void SetData(Character chara)
    {
        _character = chara;
        _nameText.text = chara.Base.Name;
        _levelText.text = $"LV:{chara.Level}";
        _hpBar.SetHp((float)chara.Hp / chara.MaxHp);
        _spBar.SetSp((float)chara.Sp / chara.MaxSp);
    }
    /// <summary>
    /// HPデータのみのセットアップ
    /// </summary>
    public void SetHpData(Character chara)
    {
        _character = chara;
        _hpBar.SetHp((float)chara.Hp / chara.MaxHp);
    }

    /*public void UpdateDate()
    {
        UpdateHp();
        UpdateSp();
        UpdateLevel();
    }*/

    /// <summary>
    /// データの反映
    /// </summary>
    public async UniTask UpdateHpAsync()
    {
        await _hpBar.SetHpSmooth((float)_character.Hp / _character.MaxHp);
    }      
    public void UpdateSp()
    {
        _spBar.SetSp((float)_character.Sp / _character.MaxSp);
    }
    public void UpdateLevel()
    {
        _levelText.text = $"LV:{_character.Level}";
    }
    /*/// HpSplevel全てを更新する
    public IEnumerator UpdateDate()
    {
        yield return _hpBar.SetHpSmooth((float)_character.Hp / _character.MaxHp);
        _spBar.SetSp((float)_character.Sp / _character.MaxSp);
        _levelText.text = $"LV:{_character.Level}";
    }*/

    public void ShowHpText(Character chara)
    {
        _character = chara;
        _hpBar.ShowPlayerHpText(_character.Hp);
    }
    public void ShowSpText(Character chara)
    {
        _character = chara;
        _spBar.ShowPlayerSpText(_character.Sp);
    }
}
