using UnityEngine;
using R3;
using ObservableCollections;
using VContainer;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    [Inject]
    private EnemyManager enemyManager;
    [Inject]
    private GameManager gameManager;

    [SerializeField] private TextMeshProUGUI remainTimeText;
    [SerializeField] private TextMeshProUGUI remainEnemyText;
    [SerializeField] private Button startButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameCelarPanel;

    void Start()
    {
        // テキストUI
        enemyManager.enemies
            .ObserveCountChanged()
            .Subscribe(count =>
            {
                remainEnemyText.text = $"{count} reamin";
            });
        gameManager.time
            .Select(t => Mathf.Max(t, 0).ToString("F1"))
            .Subscribe(str => remainTimeText.text = str)
            .AddTo(this);

        // Start Button
        startButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                gameManager.ChangeGameState(GameState.GAME_PLAYING);
                startButton.gameObject.SetActive(false);
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
            });
    }

}
