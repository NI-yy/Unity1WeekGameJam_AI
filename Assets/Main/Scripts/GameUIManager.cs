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
    [SerializeField] private GameObject gameEndPanel;

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


        // ゲーム終了後UI
        gameManager.currentGameState
            .Where(state => state == GameState.GAME_END)
            .Take(1) // 一度だけ
            .Subscribe(_ =>
            {
                remainEnemyText.gameObject.SetActive(false);
                remainEnemyText.gameObject.SetActive(false);
                gameEndPanel.SetActive(true);
            });

        // Retry Button
        retryButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            })
            .AddTo(this);
    }

}
