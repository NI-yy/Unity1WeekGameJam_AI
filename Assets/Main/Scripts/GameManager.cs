using System.Collections.Generic;
using UnityEngine;
using R3;
using ObservableCollections;
using VContainer;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Inject]
    private EnemyManager enemyManager;

    [SerializeField] private TextMeshProUGUI timeLimitText;
    [SerializeField] private TextMeshProUGUI remainEnemyText;


    private void Start()
    {
        if(timeLimitText == null || remainEnemyText == null)
        {
            Debug.LogError("Please Attach UI Text.");
        }

        enemyManager.enemies
            .ObserveCountChanged()
            .Subscribe(count =>
            {
                remainEnemyText.text = $"{count} reamin";
            });
    }
}
