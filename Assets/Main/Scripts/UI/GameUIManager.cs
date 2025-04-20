using UnityEngine;
using R3;
using ObservableCollections;
using VContainer;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System;

public class GameUIManager : MonoBehaviour
{
    [Inject]
    private EnemyManager enemyManager;
    [Inject]
    private GameManager gameManager;

    [SerializeField] private TextMeshProUGUI remainTimeText;
    [SerializeField] private TextMeshProUGUI remainEnemyText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button startButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameCelarPanel;
    [SerializeField] private ClickToStartTextAnimController clickToStartTextAnimController;
    [SerializeField] private ReadyGoTextAnimController readyGoTextAnimController;

    void Start()
    {
        SEManager seManager = SEManager.Instance;


        // テキストUI
        enemyManager.parriedCount
            .Subscribe(count =>
            {
                remainEnemyText.text = $"{count} あいさつ";
            });
        gameManager.time
            .Select(t => Mathf.Min(t, 999).ToString("F1"))
            .Subscribe(str => remainTimeText.text = str)
            .AddTo(this);

        // Start Button
        startButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                seManager.PlaySE_ClickToStart();
                clickToStartTextAnimController.OnClicked();
                TriggerReadyGoAnim_and_GameStart().Forget();
            })
            .AddTo(this);

        // Retry Button
        retryButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            })
            .AddTo(this);


        // ゲームオーバー
        gameManager.currentGameState
            .Where(state => state == GameState.GAME_OVER)
            .Take(1) // 一度だけ
            .Subscribe(_ =>
            {
                remainTimeText.gameObject.SetActive(false);
                remainEnemyText.gameObject.SetActive(false);
                retryButton.gameObject.SetActive(true);
                gameOverPanel.SetActive(true);
            });

        // ゲームクリア
        gameManager.currentGameState
            .Where(state => state == GameState.GAME_CLEAR)
            .Take(1) // 一度だけ
            .Subscribe(_ =>
            {
                remainTimeText.gameObject.SetActive(false);
                remainEnemyText.gameObject.SetActive(false);
                retryButton.gameObject.SetActive(true);
                gameCelarPanel.SetActive(true);

                scoreText.text = $"{Mathf.Min(gameManager.time.Value, 999).ToString("F1")}sec, {enemyManager.parriedCount} あいさつ！";
            });
    }

    private async UniTask TriggerReadyGoAnim_and_GameStart()
    {
        
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: destroyCancellationToken);

        startButton.gameObject.SetActive(false);
        readyGoTextAnimController.gameObject.SetActive(true);
        await readyGoTextAnimController.StartAnimation();
        gameManager.ChangeGameState(GameState.GAME_PLAYING);
    }
}
