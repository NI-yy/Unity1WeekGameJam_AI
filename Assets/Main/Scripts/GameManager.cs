using System.Collections.Generic;
using UnityEngine;
using R3;
using ObservableCollections;
using VContainer;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Inject]
    private EnemyManager enemyManager;

    [SerializeField] private TextMeshProUGUI remainTimeText;
    [SerializeField] private TextMeshProUGUI remainEnemyText;
    [SerializeField] private Button startButton;
    [SerializeField] private float timeLimit = 10f;

    private ReactiveProperty<float> time;
    private bool is_gameStarted = false;

    private void Start()
    {
        TestAttach();

        startButton.onClick.AddListener(GameStart);

        enemyManager.enemies
            .ObserveCountChanged()
            .Subscribe(count =>
            {
                remainEnemyText.text = $"{count} reamin";
            });

        time = new ReactiveProperty<float>(timeLimit);
        time
            .Select(t => Mathf.Max(t, 0).ToString("F1"))
            .Subscribe(str => remainTimeText.text = str)
            .AddTo(this);
        Observable.EveryUpdate()
            .Subscribe(_ =>
            {
                UpdateTime();
            })
            .AddTo(this);
    }

    private void UpdateTime()
    {
        if (is_gameStarted)
        {
            if (time.Value > 0)
            {
                time.Value -= Time.deltaTime;
            }
            else
            {
                GameEnd();
            }
        }
    }

    private void GameStart()
    {
        is_gameStarted = true;
    }

    private void GameEnd()
    {
        if (is_gameStarted)
        {
            is_gameStarted = false;
            Debug.Log("Game End");
        }
    }

    private void TestAttach()
    {
        if(remainTimeText == null)
        {
            Debug.LogError("remainTimeTextをアタッチしてください");
        }
        else if (remainEnemyText == null)
        {
            Debug.LogError("remainEnemyTextをアタッチしてください");
        }
        else if (startButton == null)
        {
            Debug.LogError("startButtonをアタッチしてください");
        }
    }
}
