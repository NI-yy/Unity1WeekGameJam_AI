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
    [SerializeField] private GameObject CinemachineCamera_Move_Final;

    private CinemachineImpulseSource impulseSource;

    private void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();

        playerManager.currentPlayerState
            .Where(state => state == PlayerState.MOVE)
            .Subscribe(_ =>
            {
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
            .Where(state => state == GameState.GAME_PLAYING_MIDDLE_FASE)
            .Subscribe(_ =>
            {
                CinemachineCamera_Move.GetComponent<CinemachineCamera>().Priority = 0;
                CinemachineCamera_Move_Middle.GetComponent<CinemachineCamera>().Priority = 1;
            });

        gameManager.currentGameState
            .Where(state => state == GameState.GAME_PLAYING_MIDDLE_FASE)
            .Subscribe(_ =>
            {
                CinemachineCamera_Move.GetComponent<CinemachineCamera>().Priority = 0;
                CinemachineCamera_Move_Middle.GetComponent<CinemachineCamera>().Priority = 0;
                CinemachineCamera_Move_Final.GetComponent<CinemachineCamera>().Priority = 1;
            });
    }

    public void ImplusebyParry()
    {
        impulseSource.GenerateImpulse();
    }
}
