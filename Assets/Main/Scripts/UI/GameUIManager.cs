using UnityEngine;
using R3;
using ObservableCollections;
using VContainer;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System;
using unityroom.Api;
using UnityEngine.EventSystems;

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
    [SerializeField] private Button reloadButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameCelarPanel;
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private Sprite setting_img;
    [SerializeField] private Sprite seting_close_img;
    [SerializeField] private ClickToStartTextAnimController clickToStartTextAnimController;
    [SerializeField] private ClickToStartTextAnimController retryButtonAnimController;
    [SerializeField] private ClickToStartTextAnimController settingButtonAnimController;
    [SerializeField] private ReadyGoTextAnimController readyGoTextAnimController;

    private bool setting_panel_close = true;

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
            .Subscribe(str =>
            {
                remainTimeText.text = str;
                //Debug.Log(str);
            })
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
                seManager.PlaySE_ClickToStart();
                retryButtonAnimController.OnClicked();
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            })
            .AddTo(this);

        // Reload Button
        reloadButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                seManager.PlaySE_ClickToStart();
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            })
            .AddTo(this);

        // Setting Button
        settingButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                seManager.PlaySE_ClickToStart();
                settingButtonAnimController.OnClicked();

                if (setting_panel_close)
                {
                    startButton.gameObject.SetActive(false);

                    settingPanel.SetActive(true);
                    settingButton.gameObject.GetComponent<Image>().sprite = seting_close_img;
                    setting_panel_close = false;
                }
                else
                {
                    startButton.gameObject.SetActive(true);

                    settingPanel.SetActive(false);
                    settingButton.gameObject.GetComponent<Image>().sprite = setting_img;
                    setting_panel_close = true;
                }
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
                reloadButton.gameObject.SetActive(false);

                retryButton.gameObject.SetActive(true);
                gameCelarPanel.SetActive(true);

                scoreText.text = $"{Mathf.Min(gameManager.time.Value, 999).ToString("F1")}sec, {enemyManager.parriedCount} あいさつ！";

                if(enemyManager.parriedCount.Value == 51)
                {
                    UnityroomApiClient.Instance.SendScore(1, gameManager.time.Value, ScoreboardWriteMode.HighScoreAsc);
                }
            });
    }

    private async UniTask TriggerReadyGoAnim_and_GameStart()
    {
        
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: destroyCancellationToken);

        startButton.gameObject.SetActive(false);
        readyGoTextAnimController.gameObject.SetActive(true);
        settingButton.gameObject.SetActive(false);
        reloadButton.gameObject.SetActive(true);
        await readyGoTextAnimController.StartAnimation();
        gameManager.ChangeGameState(GameState.GAME_PLAYING);
    }
}
