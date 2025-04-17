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

    }

    protected override void OnPlayerExit()
    {

    }
}
