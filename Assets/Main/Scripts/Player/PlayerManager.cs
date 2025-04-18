using R3;
using UnityEngine;
using VContainer;

public class PlayerManager : MonoBehaviour
{
    [Inject]
    private GameManager gameManager;

    private PlayerController playerController;

    private void Start()
    {
        playerController = transform.GetChild(0).GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("プレイヤーが見つかりません。PlayerManagerの子にPlayer(PlayerController)を配置してください");
        }

        gameManager.currentGameState
            .Where(state => state == GameState.GAME_PLAYING)
            .Take(1) // 一度だけ
            .Subscribe(_ =>
            {
                playerController.inputEnabled = true;
            });

        gameManager.currentGameState
            .Where(state => state == GameState.GAME_END)
            .Take(1) // 一度だけ
            .Subscribe(_ =>
            {
                playerController.inputEnabled = false;
            });
    }
}
