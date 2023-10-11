using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask _solidObjectsLayer;
    [SerializeField] private GameController _gameController;

    private Animator _animator;
    private bool _isMoving = false;

    private CancellationToken token;

    private void Start()
    {
        token = this.GetCancellationTokenOnDestroy();
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// GameState.FreeRoam の時にUpdate()の役割を果たす
    /// </summary>
    public void HandleUpdate()
    {
        // 動いていない時入力を受け付ける
        if (_isMoving == false)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");

            // 左右を押している時
            if (x != 0)
            {
                y = 0;
            }
            // 移動している時
            if (x != 0 || y != 0)
            {
                // 移動アニメーション
                _animator.SetFloat("InputX", x);
                _animator.SetFloat("InputY", y);
                
                // 移動
                MoveAsync(new Vector2(x, y)).Forget();
            }
        }
        _animator.SetBool("IsMoving",_isMoving);
    }

    /// <summary>
    /// 1マス徐々に近づける
    /// </summary>
    private async UniTask MoveAsync(Vector3 direction)
    {
        _isMoving = true;
        int speed = 5;
        Vector3 _targetPos = transform.position + direction;
        if (IsWalkable(_targetPos) == false)
        {
            _isMoving = false;
            return;
        }

        //現在とターゲットの場所が違うなら、近づける
        while ((_targetPos - transform.position).sqrMagnitude > Mathf.Epsilon/*0でない最小値*/)
        {
            //MoveTowards(現在地, 目標値, 速度) : 目標値に近づける
            transform.position = 
                Vector3.MoveTowards(transform.position, _targetPos, speed * Time.deltaTime);
            await UniTask.Yield(token);// 1フレーム待つ
        }

        transform.position = _targetPos; // 移動した位置に固定
        _isMoving = false;

        CheckForEncounters();// エンカウントしたかどうか

    }
    /// <summary>
    /// 特定の位置に移動出来るか判定
    /// </summary>
    private bool IsWalkable(Vector3 targetPos)
    {
        // targetPosを中心に円形のRayを作る : SolidObjectsLayerにぶつかったらtrueが帰ってくる.だからfalse
        return Physics2D.OverlapCircle(targetPos, 0.2f, _solidObjectsLayer) == false;
    }
    /// <summary>
    /// 敵とエンカウントしたかチェック
    /// </summary>
    private void CheckForEncounters()
    {
        // 10％の確率
        if (Random.Range(0,100) < 10)
        {
            // インターフェイス
            IGameController gameController = _gameController;
            gameController.StartBattle();

            _animator.SetBool("IsMoving",false);
        } 
    }
}
