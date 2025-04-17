using UnityEngine;
using R3;
using ObservableCollections;

public class EnemyManager : MonoBehaviour
{
    public ObservableList<EnemyBase> enemies = new ObservableList<EnemyBase>();

    public void AddEnemy(EnemyBase enemy)
    {
        enemies.Add(enemy);
    }

    public void RemoveEnemy(EnemyBase enemy)
    {
        enemies.Remove(enemy);
    }
}
