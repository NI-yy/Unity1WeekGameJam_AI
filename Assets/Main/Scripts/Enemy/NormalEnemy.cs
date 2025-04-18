using System;
using UnityEngine;
using R3;
using Cysharp.Threading.Tasks;
using System.Threading;
using DG.Tweening;

public class NormalEnemy : EnemyBase
{
    [SerializeField] private Animator animator;
    [SerializeField] private float maxMoveRadius = 1.0f;
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private GameObject attackCicle;
    [SerializeField] private GameObject attackCicleEnd;

    private Vector3 initPos;
    private bool isMoving = false;
    private GameObject playerObj;

    private CancellationTokenSource cancellationTokenSource;

    protected override void Start()
    {
        base.Start();

        cancellationTokenSource = new CancellationTokenSource();
        initPos = transform.localPosition;
        playerObj = GameObject.FindGameObjectWithTag("Player");
    }

    protected override void Update()
    {
        base.Update();
        if (!playerInRange)
        {
            if (!isMoving)
            {
                PerformMove().Forget();
            }
        }
        else
        {
            transform.LookAt(playerObj.transform.position);
        }
    }

    private async UniTask PerformMove()
    {
        isMoving = true;
        Vector3 destination = ExtractRandomPos(initPos, maxMoveRadius);
        CancellationToken token_move = cancellationTokenSource.Token;

        transform.LookAt(destination);
        await transform.DOLocalMove(destination, moveSpeed).SetEase(Ease.Linear)
                 .SetSpeedBased().ToUniTask(cancellationToken: token_move);
        isMoving = false;
    }

    private Vector3 ExtractRandomPos(Vector3 objectPos, float radius_max, float radius_min = 0.1f)
    {
        Vector2 destinationPos = new Vector2();
        float angel = UnityEngine.Random.Range(0, 360f);
        float radius = UnityEngine.Random.Range(radius_min, radius_max);
        destinationPos.x = objectPos.x + (radius * Mathf.Sin(angel));
        destinationPos.y = objectPos.z + (radius * Mathf.Cos(angel));
        return new Vector3(destinationPos.x, objectPos.y, destinationPos.y);
    }

    protected override void OnAttackDurationStart()
    {
        //animator.SetTrigger("Attack");
    }

    protected override void OnPlayerEnter()
    {
        base.OnPlayerEnter();
        cancellationTokenSource?.Cancel();
        cancellationTokenSource?.Dispose();
    }

    protected override void OnPlayerExit()
    {
        base.OnPlayerExit();
        isMoving = false;
        cancellationTokenSource = new CancellationTokenSource();
        Debug.Log("再開");
    }

    protected override void OnAttackIntervalStart()
    {
        base.OnAttackIntervalStart();
        Debug.Log("IntervalStart");
    }

    protected override void OnAttackIntervalEnd()
    {
        base.OnAttackIntervalStart();
        Debug.Log("IntervalEnd");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        cancellationTokenSource?.Dispose();
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        cancellationTokenSource?.Dispose();
    }
}
