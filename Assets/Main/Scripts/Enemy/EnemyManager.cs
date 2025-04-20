using ObservableCollections;
using UnityEngine;
using R3;
using VContainer;
using DG.Tweening.Core.Easing;

public class EnemyManager : MonoBehaviour
{
    [Inject]
    private GameManager gameManager;

    [Inject]
    private CinemachineCameraManager_yy cinemachineCameraManager_yy;

    public ObservableList<EnemyBase> enemies = new ObservableList<EnemyBase>();
    public ReactiveProperty<int> parriedCount = new ReactiveProperty<int>();

    private SEManager seManager;

    public void Start()
    {
        seManager = SEManager.Instance;
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

            // OnDestroy=パリィされたことを購読してリストから除く
            enemy.onParriedAndEnemyDestroy.Subscribe(e =>
            {
                seManager.PlaySE_Parry();
                cinemachineCameraManager_yy.ImplusebyParry();
                parriedCount.Value++;
                enemies.Remove(e);
                Destroy(e.gameObject);
            });
        }
    }

}
