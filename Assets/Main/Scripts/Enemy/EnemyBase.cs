using System;
using UnityEngine;
using R3;

// protected: そのクラス自身と派生クラスからのみ参照可能
// virtual: オーバーライド可能

public class EnemyBase : MonoBehaviour
{
    [SerializeField] protected GameObject attackArea;
    [SerializeField] protected float attackInterval = 2f;
    [SerializeField] protected float attackDuration = 0.3f;

    protected bool playerInRange = false;
    private IDisposable attackLoop;

    protected virtual void Start()
    {
        SetupAttackLoop();
    }

    private void SetupAttackLoop()
    {
        attackLoop = Observable.Interval(System.TimeSpan.FromSeconds(attackInterval))
            .Where(_ => playerInRange)
            .Subscribe(_ => PerformAttack())
            .AddTo(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            OnPlayerEnter();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            OnPlayerExit();
            attackArea.SetActive(false); // 念のため停止
        }
    }

    private void PerformAttack()
    {
        attackArea.SetActive(true);
        OnAttackStart(); // 拡張（アニメーションなど）

        Observable.Timer(System.TimeSpan.FromSeconds(attackDuration))
            .Subscribe(_ =>
            {
                attackArea.SetActive(false);
                OnAttackEnd(); // 拡張
            })
            .AddTo(this);
    }

    // --- 拡張 ---
    protected virtual void OnPlayerEnter() { }
    protected virtual void OnPlayerExit() { }
    protected virtual void OnAttackStart() { }
    protected virtual void OnAttackEnd() { }
}
