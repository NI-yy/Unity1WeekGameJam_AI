using ObservableCollections;
using UnityEngine;
using R3;

public class EnemyManager : MonoBehaviour
{
    public ObservableList<EnemyBase> enemies = new ObservableList<EnemyBase>();


    private void Start()
    {
        foreach (EnemyBase enemy in FindObjectsByType<EnemyBase>(FindObjectsSortMode.None))
        {
            enemies.Add(enemy);

            // OnDestroyされたことを購読してリストから除く
            enemy.onParriedAndEnemyDestroy.Subscribe(e =>
            {
                enemies.Remove(e);
                Destroy(e.gameObject);
            });
        }
    }

}
