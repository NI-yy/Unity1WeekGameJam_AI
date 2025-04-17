using UnityEngine;

public class PlayerParryArea : MonoBehaviour
{
    private string enemyAttackTagName = "EnemyAttack";
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(enemyAttackTagName))
        {
            Debug.Log("パリィ!!!!");
        }
    }
}
