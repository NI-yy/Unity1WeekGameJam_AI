using R3;
using UnityEngine;
using VContainer;

public enum PlayerState
{
    MOVE,
    COMBAT
}

public class PlayerManager : MonoBehaviour
{
    public ReactiveProperty<PlayerState> currentPlayerState = new ReactiveProperty<PlayerState>(PlayerState.MOVE);

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

        // PlayerStateの更新 (CinemChineの切り替えに使用
        playerController.OnCombatState += () => currentPlayerState.Value = PlayerState.COMBAT;
        playerController.OnMoveState += () => currentPlayerState.Value = PlayerState.MOVE;


        // Playerに対するコントローラー操作の管理
        gameManager.currentGameState
            .Where(state => state == GameState.GAME_PLAYING)
            .Take(1) // 一度だけ
            .Subscribe(_ =>
            {
                playerController.inputEnabled = true;
            });

        gameManager.currentGameState
            .Where(state => state == GameState.GAME_OVER || state == GameState.GAME_CLEAR)
            .Take(1) // 一度だけ
            .Subscribe(_ =>
            {
                playerController.inputEnabled = false;
            });
    }
}
