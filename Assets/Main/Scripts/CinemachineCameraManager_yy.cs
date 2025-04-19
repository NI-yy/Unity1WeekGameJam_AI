using R3;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;

public class CinemachineCameraManager_yy : MonoBehaviour
{
    [Inject]
    private PlayerManager playerManager;
    [Inject]
    private GameManager gameManager;

    [SerializeField] private GameObject CinemachineCamera_Move;
    [SerializeField] private GameObject CinemachineCamera_Combat;
    [SerializeField] private GameObject CinemachineCamera_Move_Middle;

    private void Start()
    {
        playerManager.currentPlayerState
            .Where(state => state == PlayerState.MOVE)
            .Subscribe(_ =>
            {
                Debug.Log("MoveCameraに変更");
                CinemachineCamera_Move.GetComponent<CinemachineCamera>().Priority = 1;
                CinemachineCamera_Combat.GetComponent<CinemachineCamera>().Priority = 0;
            });

        playerManager.currentPlayerState
            .Where(state => state == PlayerState.COMBAT)
            .Subscribe(_ =>
            {
                //Debug.Log("MoveCameraMiddleに変更");
                //CinemachineCamera_Move.GetComponent<CinemachineCamera>().Priority = 0;
                //CinemachineCamera_Combat.GetComponent<CinemachineCamera>().Priority = 1;
            });

        gameManager.currentGameState
            .Where(state => state == GameState.GAME_PLAYING_MIDDLE_BOSS)
            .Subscribe(_ =>
            {
                Debug.Log("MoveCamera_Middleに変更");
                CinemachineCamera_Move.GetComponent<CinemachineCamera>().Priority = 0;
                CinemachineCamera_Move_Middle.GetComponent<CinemachineCamera>().Priority = 1;
            });
    }
}
