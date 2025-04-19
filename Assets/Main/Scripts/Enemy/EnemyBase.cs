using System;
using UnityEngine;
using R3;
using Cysharp.Threading.Tasks;
using System.Threading;

// protected: そのクラス自身と派生クラスからのみ参照可能
// virtual: オーバーライド可能

public class EnemyBase : MonoBehaviour
{
    public Observable<EnemyBase> onParriedAndEnemyDestroy => _onParriedAndEnemyDestroy;
    private Subject<EnemyBase> _onParriedAndEnemyDestroy = new Subject<EnemyBase>();


    [SerializeField] protected GameObject attackArea;
    [SerializeField] protected float attackInterval = 2f;
    [SerializeField] protected float attackDuration = 0.3f;

    protected bool playerInRange = false;
    private bool attackLoopEnd_And_Trigger = false;

    private EnemyAttackArea enemyAttackArea;

    private CancellationTokenSource cancellationTokenSource_attack;

    protected virtual void Awake()
    {
        enemyAttackArea = attackArea.GetComponent<EnemyAttackArea>();

        if (enemyAttackArea != null)
        {
            enemyAttackArea.OnParried
                .Subscribe(_ => OnParriedAndDestroy())
                .AddTo(this); // GameObjectと紐づけて破棄も自動化
        }
        else
        {
            Debug.LogError("EnemyAttackArea is not found. Please Attach.");
        }
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        if (playerInRange && attackLoopEnd_And_Trigger)
        {
            OnAttackIntervalTrigger().Forget();
        }
    }

    private void OnParriedAndDestroy()
    {
        _onParriedAndEnemyDestroy.OnNext(this);
        _onParriedAndEnemyDestroy.OnCompleted();
        _onParriedAndEnemyDestroy.Dispose();

    }

    private async UniTask OnAttackIntervalTrigger()
    {
        attackLoopEnd_And_Trigger = false;

        CancellationToken token_attack = cancellationTokenSource_attack.Token;

        OnAttackIntervalStart(token_attack);
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(attackInterval), cancellationToken: token_attack);
            OnAttackIntervalEnd();
            PerformAttack(token_attack);
        }
        catch (OperationCanceledException)
        {
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            cancellationTokenSource_attack = new CancellationTokenSource();
            playerInRange = true;
            attackLoopEnd_And_Trigger = true;
            OnPlayerEnter();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            attackArea.SetActive(false); // 念のため停止
            playerInRange = false;
            attackLoopEnd_And_Trigger = false;
            cancellationTokenSource_attack?.Cancel();
            cancellationTokenSource_attack?.Dispose();
            OnPlayerExit();
        }
    }

    private async void PerformAttack(CancellationToken token_attack)
    {
        attackArea.SetActive(true);
        OnAttackDurationStart(); // 拡張（アニメーションなど）
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(attackDuration), cancellationToken: token_attack);
            attackArea.SetActive(false);

            OnAttackDurationEnd(); // 拡張

            attackLoopEnd_And_Trigger = true;
        }
        catch (OperationCanceledException)
        {
            return;
        }
    }


    protected virtual void OnPlayerEnter()
    {
        
    }
    protected virtual void OnPlayerExit()
    {
        OnAttackIntervalEnd();
        OnAttackDurationEnd();
    }

    protected virtual void OnDestroy()
    {
        cancellationTokenSource_attack?.Dispose();
    }

    protected virtual void OnApplicationQuit()
    {
        cancellationTokenSource_attack?.Dispose();
    }

    protected virtual void OnAttackIntervalStart(CancellationToken token = default) { }
    protected virtual void OnAttackIntervalEnd() { }
    protected virtual void OnAttackDurationStart() { }
    protected virtual void OnAttackDurationEnd() { }
}
