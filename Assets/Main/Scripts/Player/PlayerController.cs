using UnityEngine;
using R3;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    public bool inputEnabled = false;
    public UnityAction OnCombatState;
    public UnityAction OnMoveState;

    [HideInInspector] public float speed = 5f; // プレイヤーの移動速度 
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashSecound = 1f;
    [SerializeField] private float dashCoolTime = 1f;
    [SerializeField] private float parryActiveTime = 0.5f;
    [SerializeField] private int parryCoolFrame = 30;
    [SerializeField] private GameObject parrtyArea;
    [SerializeField] private GameObject dashTrailEffectObj;

    private float hAxis; // 水平方向の入力
    private float vAxis; // 垂直方向の入力

    private Vector3 moveVector;
    private Vector3 prev_moveVector;

    private bool dashButtonDown;
    private bool parryButtonDown;

    private bool isDash = false;
    private bool canParry = true;

    private HashSet<Collider> enemiesInRange = new HashSet<Collider>();
    private bool inCombat = false;

    private SEManager seManager;

    private void Start()
    {
        seManager = SEManager.Instance;
    }

    void Update()
    {
        if (inputEnabled)
        {
            GetInput();
            Move();
            Turn();
            TriggerParry();
            Dash().Forget();
        }

        enemiesInRange.RemoveWhere(c => c == null);

        bool hasEnemy = enemiesInRange.Count > 0;
        if (hasEnemy != inCombat)
        {
            inCombat = hasEnemy;
            if (inCombat)
            {
                OnCombatState?.Invoke();
            }
            else
            {
                OnMoveState?.Invoke();
            }
               
        }
    }

    private void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal"); // 水平方向の入力を取得
        vAxis = Input.GetAxisRaw("Vertical"); // 垂直方向の入力を取得
        parryButtonDown = Input.GetMouseButtonDown(0);
        dashButtonDown = Input.GetMouseButtonDown(1);
    }

    private void Move()
    {
        Vector3 rawInput = new Vector3(hAxis, 0, vAxis);
        if (rawInput.sqrMagnitude < 0.01f)
        {
            moveVector = Vector3.zero;
        }
        else
        {
            moveVector = rawInput.normalized;
        }

        // 移動先の距離を設定
        float moveDistance = speed * Time.deltaTime;

        // Raycast で obstacle タグを検出したら移動しない
        float radius = 2f;
        Vector3 offset = new Vector3(0f, 2.5f, 0f);
        if (Physics.SphereCast(transform.position + offset, radius, moveVector, out var hit, moveDistance))
        {
            if (hit.collider.CompareTag("Obstacle") || hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Water"))
            {
                return; // 移動しない
            }
        }

        // 移動処理
        transform.position += moveVector * moveDistance;
    }

    private void Turn(){
        transform.LookAt(transform.position + moveVector);
    }

    private void TriggerParry()
    {
        if (parryButtonDown && canParry)
        {
            if (inCombat)
            {
                // 一番近い敵を探す
                Collider nearestEnemy = enemiesInRange
                    .Where(c => c != null) // null チェック（Destroyされた敵など）
                    .OrderBy(c => Vector3.Distance(transform.localPosition, c.transform.localPosition))
                    .FirstOrDefault();

                if (nearestEnemy != null)
                {
                    Vector3 nearestEnemy_arrangeY = new Vector3(nearestEnemy.transform.localPosition.x, transform.localPosition.y, nearestEnemy.transform.localPosition.z);
                    transform.LookAt(nearestEnemy_arrangeY);
                }
            }


            parrtyArea.SetActive(true);
            canParry = false;

            // 0.5秒後に判定オブジェクトを非アクティブ化
            Observable.Timer(System.TimeSpan.FromSeconds(parryActiveTime))
                .Subscribe(_ => parrtyArea.SetActive(false))
                .AddTo(this);

            // 30フレーム後に再び入力を受け付ける
            Observable.TimerFrame(parryCoolFrame)
                .Subscribe(_ => canParry = true)
                .AddTo(this);
        }
    }

    private async UniTask Dash(){
        if(dashButtonDown && !isDash)
        {
            isDash = true;
            float sphereRadius = 2f;
            Vector3 castOffset = new Vector3(0f, 2.5f, -2.0f);

            Vector3 origin = transform.position + castOffset;
            Vector3 dir = transform.forward;

            RaycastHit[] hits = Physics.SphereCastAll(origin, sphereRadius, dir, dashDistance + Mathf.Abs(castOffset.z));
            var ordered = hits.OrderBy(h => h.distance).ToArray();

            RaycastHit? groundHit = null;
            RaycastHit? nearestObstacle = null;
            RaycastHit? obstacleHit = null;
            RaycastHit? wallHit = null;
            RaycastHit? waterHit = null;

            foreach (var h in ordered)
            {
                if (h.collider.CompareTag("Ground"))
                {
                    Debug.Log($"Ground, {h.distance}");
                    groundHit = h;
                }
                else if (waterHit == null && h.collider.CompareTag("Water"))
                {
                    Debug.Log($"Water, {h.distance}");
                    waterHit = h;
                }
                else if (h.collider.CompareTag("Obstacle"))
                {
                    Debug.Log($"Obstacle, {h.distance}");
                    obstacleHit = h;
                }
                else if (h.collider.CompareTag("Wall"))
                {
                    Debug.Log($"Wall, {h.distance}");
                    wallHit = h;
                }
                
                if(!nearestObstacle.HasValue && (h.collider.CompareTag("Obstacle") || h.collider.CompareTag("Wall") || h.collider.CompareTag("Water")))
                {
                    nearestObstacle = h;
                    Debug.Log($"{nearestObstacle == null}, {!nearestObstacle.HasValue}");
                    Debug.Log($"{!nearestObstacle.HasValue && h.collider.CompareTag("Obstacle") || h.collider.CompareTag("Wall") || h.collider.CompareTag("Water")}");
                    Debug.Log($"{nearestObstacle.Value.distance}, {h.distance}");
                }
            }

            try
            {
                // 到達先を計算
                float safeDistance;
                if (nearestObstacle.HasValue)
                {
                    Debug.Log("障害物あり");
                    safeDistance = nearestObstacle.Value.distance - sphereRadius - Mathf.Abs(castOffset.z);
                    Debug.Log($"{nearestObstacle.Value.distance}, {sphereRadius}, {safeDistance}");

                    if (waterHit.HasValue)
                    {
                        if (!wallHit.HasValue && !obstacleHit.HasValue &&
                            groundHit.Value.distance - waterHit.Value.distance > 0)
                        {
                            Debug.Log("水飛び越え");
                            safeDistance = groundHit.Value.distance + sphereRadius;
                        }
                    }
                }
                else
                {
                    Debug.Log("障害物無し");
                    safeDistance = dashDistance;
                }

                safeDistance = Mathf.Max(safeDistance, 0f);
                Vector3 destinationLocal = transform.localPosition + dir * safeDistance;

                dashTrailEffectObj.SetActive(true);
                // 4) 実際に移動
                await transform.DOLocalMove(destinationLocal, dashSecound)
                               .SetEase(Ease.Linear)
                               .ToUniTask(cancellationToken: destroyCancellationToken);

                // 5) クールタイム
                await UniTask.Delay(TimeSpan.FromSeconds(dashCoolTime), cancellationToken: destroyCancellationToken);
                dashTrailEffectObj.SetActive(false);

            }
            catch(InvalidOperationException e)
            {
                Console.WriteLine(e);
            }


            
            seManager.PlaySE_Dash();
            isDash = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(other);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.localPosition + new Vector3(0f, 2.5f, -2.0f), 2f);

        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.localPosition + transform.forward * 10f, 5f);
    }
}
