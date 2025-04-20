using ObservableCollections;
using UnityEngine;
using R3;
using VContainer;

public class EnemyManager : MonoBehaviour
{
    [Inject]
    private GameManager gameManager;

    public ObservableList<EnemyBase> enemies = new ObservableList<EnemyBase>();
    public ReactiveProperty<int> parriedCount = new ReactiveProperty<int>();

    public void Start()
    {
        parriedCount.Value = 0;

        gameManager.currentGameState
            .Where(state => state == GameState.GAME_PLAYING)
            .Take(1) // 一度だけ
            .Subscribe(_ =>
            {
                ActivateEnemies();
            });

        gameManager.currentGameState
            .Where(state => state == GameState.GAME_OVER || state == GameState.GAME_CLEAR)
            .Take(1) // 一度だけ
            .Subscribe(_ =>
            {
                transform.GetChild(0).gameObject.SetActive(false);
            });
    }

    private void ActivateEnemies()
    {
        transform.GetChild(0).gameObject.SetActive(true);

        foreach (EnemyBase enemy in FindObjectsByType<EnemyBase>(FindObjectsSortMode.None))
        {
            enemies.Add(enemy);

            // OnDestroyされたことを購読してリストから除く
            enemy.onParriedAndEnemyDestroy.Subscribe(e =>
            {
                parriedCount.Value++;
                enemies.Remove(e);
                Destroy(e.gameObject);
            });
        }
    }

}
