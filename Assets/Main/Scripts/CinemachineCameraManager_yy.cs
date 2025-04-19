using R3;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;

public class CinemachineCameraManager_yy : MonoBehaviour
{
    [Inject]
    private PlayerManager playerManager;

    [SerializeField] private GameObject CinemachineCamera_Move;
    [SerializeField] private GameObject CinemachineCamera_Combat;

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
                Debug.Log("CombatCameraに変更");
                CinemachineCamera_Move.GetComponent<CinemachineCamera>().Priority = 0;
                CinemachineCamera_Combat.GetComponent<CinemachineCamera>().Priority = 1;
            });
    }
}
