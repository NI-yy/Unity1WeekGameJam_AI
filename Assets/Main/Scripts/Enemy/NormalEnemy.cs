using UnityEngine;

public class NormalEnemy : EnemyBase
{
    [SerializeField] private Animator animator;

    protected override void OnAttackStart()
    {
        //animator.SetTrigger("Attack");
    }

    protected override void OnPlayerEnter()
    {
        Debug.Log("プレイヤーを見つけた！");
    }

    protected override void OnPlayerExit()
    {
        Debug.Log("プレイヤーを見失った");
    }
}
