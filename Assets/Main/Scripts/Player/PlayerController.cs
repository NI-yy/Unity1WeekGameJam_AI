using UnityEngine;
using R3;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

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
                Debug.Log("戦闘");
                OnCombatState?.Invoke();
            }
            else
            {
                Debug.Log("移動");
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
            if (hit.collider.CompareTag("Obstacle") || hit.collider.CompareTag("Wall"))
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

            // Raycast で obstacle タグを検出したら移動しない
            float radius = 2f;
            Vector3 offset = new Vector3(0f, 2.5f, 0f);
            if (Physics.SphereCast(transform.position + offset, radius, transform.forward, out var hit, dashDistance))
            {
                if (hit.collider.CompareTag("Obstacle") || hit.collider.CompareTag("Wall"))
                {
                    Debug.Log("cannot Dash");
                    isDash = false;
                    return;
                }
            }


            Vector3 destination = transform.localPosition + transform.forward * dashDistance;
            await transform.DOLocalMove(destination, dashSecound)
                .SetEase(Ease.Linear)
                .ToUniTask(cancellationToken: destroyCancellationToken);

            await UniTask.Delay(TimeSpan.FromSeconds(dashCoolTime), cancellationToken: destroyCancellationToken);

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
        Gizmos.DrawWireSphere(transform.localPosition + new Vector3(0f, 2.5f, 0f), 2f);

        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.localPosition + transform.forward * 10f, 5f);
    }
}
