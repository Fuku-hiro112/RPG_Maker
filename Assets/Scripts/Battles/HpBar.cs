using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField]  private GameObject _health;
    [SerializeField] private Text _playerHpText;

    private CancellationToken token;

    private void Start()
    {
        token = this.GetCancellationTokenOnDestroy();
    }

    public void SetHp(float hp)
    {
        _health.transform.localScale = new Vector3(hp, 1, 1);
    }

    public async UniTask SetHpSmooth(float newHp)
    {
        float currentHp = _health.transform.localScale.x;
        float changeAmount = currentHp - newHp;

        // currentHp ‚Æ newHp‚É·‚ª‚ ‚é‚È‚çŒJ‚è•Ô‚·
        while (currentHp - newHp > Mathf.Epsilon)
        {
            currentHp -= changeAmount * Time.deltaTime;
            _health.transform.localScale = new Vector3(currentHp, 1, 1);
            await UniTask.Yield(token);
        }
        _health.transform.localScale = new Vector3(newHp, 1, 1);
    }
    public void ShowPlayerHpText(int currentHp)
    {
        _playerHpText.text = currentHp.ToString();
    }
}
