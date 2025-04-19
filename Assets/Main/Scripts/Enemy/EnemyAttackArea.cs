using R3;
using UnityEngine;

public class EnemyAttackArea : MonoBehaviour
{
    private string playerParryAreaTagName = "PlayerParryArea";

    private Subject<Unit> _onParried = new();
    public Observable<Unit> OnParried => _onParried;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(playerParryAreaTagName))
        {
            _onParried.OnNext(Unit.Default);
        }
    }

    private void OnDestroy()
    {
        _onParried.Dispose();
    }
}
