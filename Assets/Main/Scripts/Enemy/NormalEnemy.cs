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
    private Vector3 initAttackCircleScale;
    private bool isMoving = false;
    private GameObject playerObj;

    private CancellationTokenSource cancellationTokenSource_move;

    protected override void Start()
    {
        base.Start();

        cancellationTokenSource_move = new CancellationTokenSource();
        initPos = transform.localPosition;
        initAttackCircleScale = attackCicle.transform.localScale;
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
        CancellationToken token_move = cancellationTokenSource_move.Token;

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
        cancellationTokenSource_move?.Cancel();
        cancellationTokenSource_move?.Dispose();
    }

    protected override void OnPlayerExit()
    {
        base.OnPlayerExit();
        isMoving = false;
        cancellationTokenSource_move = new CancellationTokenSource();
        //Debug.Log("再開");
    }

    protected override void OnAttackIntervalStart(CancellationToken token_attack)
    {
        base.OnAttackIntervalStart();
        //Debug.Log("IntervalStart");

        attackCicle.transform.localScale = initAttackCircleScale;
        attackCicle.SetActive(true);
        attackCicleEnd.SetActive(true);
        attackCicle.transform.DOScale(attackCicleEnd.transform.localScale, attackInterval)
            .SetEase(Ease.Linear)
            .ToUniTask(cancellationToken: token_attack);
    }

    protected override void OnAttackIntervalEnd()
    {
        base.OnAttackIntervalEnd();
        attackCicle.SetActive(false);
        attackCicleEnd.SetActive(false);
        //Debug.Log("IntervalEnd");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        cancellationTokenSource_move?.Dispose();
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        cancellationTokenSource_move?.Dispose();
    }
}
