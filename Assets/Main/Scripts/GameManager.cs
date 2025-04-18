using UnityEngine;
using R3;

public enum GameState
{
    BEFORE_START,
    GAME_PLAYING,
    GAME_STOP,
    GAME_END
}

public class GameManager : MonoBehaviour
{
    

    public ReactiveProperty<float> time = new ReactiveProperty<float>();
    public ReactiveProperty<GameState> currentGameState = new ReactiveProperty<GameState>(GameState.BEFORE_START);

    [SerializeField] private float timeLimit = 10f;


    private bool stateEnter = true;

    public void ChangeGameState(GameState state)
    {
        currentGameState.Value = state;
        stateEnter = true;
    }


    private void Start()
    {
        time.Value = timeLimit;
    }

    private void Update()
    {
        switch (currentGameState.Value)
        {
            case GameState.BEFORE_START:
                break;
            case GameState.GAME_PLAYING:
                if (stateEnter)
                { 
                    stateEnter = false;
                    GameStart();
                    return; 
                }
                UpdateTime();
                break;
            case GameState.GAME_STOP:
                break;
            case GameState.GAME_END:
                if (stateEnter)
                {
                    stateEnter = false;
                    GameEnd();
                    return;
                }
                break;
        }
    }

    private void UpdateTime()
    {
        if (time.Value > 0)
        {
            time.Value -= Time.deltaTime;
        }
        else
        {
            ChangeGameState(GameState.GAME_END);
        }
    }

    public void GameStart()
    {
        Debug.Log("Game Start");
    }

    private void GameEnd()
    {
        Debug.Log("Game End");
    }
}
