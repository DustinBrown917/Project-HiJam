using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    [SerializeField] private Player player1;
    public Player HumanPlayer { get { return player1; } }
    [SerializeField] private Player player2;
    [SerializeField] private int _currentRound = 1;
    [SerializeField] private GameStates _currentState = GameStates.MAIN_MENU;
    [SerializeField] private CanvasGroup gameMenu;
    public GameStates CurrentState { get { return _currentState; } }
    public int CurrentRound { get { return _currentRound; } }

    

    private void Awake()
    {
        if(_instance != null)
        {
            Destroy(gameObject);
        } else
        {
            _instance = this;
        }
    }

    // Use this for initialization
    void Start () {
        player1.NoTroopsLeft += Player_NoTroopsLeft;
        player2.NoTroopsLeft += Player_NoTroopsLeft;

	}

    public void ChangeGameState(GameStates state)
    {
        if(state == CurrentState) { return; }

        GameStateChangedArgs args = new GameStateChangedArgs(_currentState, state);

        _currentState = state;

        OnGameStateChanged(args);
    }

    public void ResetRoundCount()
    {
        _currentRound = 1;
    }

    private void Player_NoTroopsLeft(object sender, Player.NoTroopsLeftArgs e)
    {
        ChangeGameState(GameStates.BATTLE_ENDED); 
    }

    private IEnumerator FadeMenuTo(float targetAlpha, float duration)
    {
        if(targetAlpha > 0) { gameMenu.gameObject.SetActive(true); }
        float time = 0;
        float startingAlpha = gameMenu.alpha;
        while(time <= duration)
        {
            time += Time.deltaTime;
            gameMenu.alpha = Mathf.Lerp(startingAlpha, targetAlpha, time/duration);
            yield return null;
        }

        if(targetAlpha <= 0) { gameMenu.gameObject.SetActive(false); }
    }

    public event EventHandler<GameStateChangedArgs> GameStateChanged;

    public class GameStateChangedArgs : EventArgs
    {
        public GameStates oldState;
        public GameStates newState;

        public GameStateChangedArgs()
        {

        }

        public GameStateChangedArgs(GameStates oldState, GameStates newState)
        {
            this.oldState = oldState;
            this.newState = newState;
        }
    }

    private void OnGameStateChanged(GameStateChangedArgs e)
    {
        Debug.Log("State changed to " + e.newState.ToString() + " from " + e.oldState.ToString());

        if(e.newState == GameStates.SUMMARY_SCREEN) { _currentRound++; }

        EventHandler<GameStateChangedArgs> handler = GameStateChanged;

        if(handler != null)
        {
            handler(this, e);
        }

        if(e.newState == GameStates.BATTLE_PREP)
        {
            StartCoroutine(DelayBeforeBattle());
        } else if(e.newState == GameStates.BATTLE_ENDED)
        {
            StartCoroutine(DelayAfterBattle());
        } 
    }

    private IEnumerator DelayBeforeBattle()
    {
        StartCoroutine(FadeMenuTo(0f, 1.0f));
        yield return new WaitForSeconds(2.0f);
        ChangeGameState(GameStates.BATTLING);
    }

    private IEnumerator DelayAfterBattle()
    {
        yield return new WaitForSeconds(2.0f);
        StartCoroutine(FadeMenuTo(1.0f, 1.0f));
        yield return new WaitForSeconds(1.0f);
        ChangeGameState(GameStates.SUMMARY_SCREEN);
    }
}

public enum GameStates
{
    BATTLE_PREP,
    BATTLING,
    BATTLE_ENDED,
    SUMMARY_SCREEN,
    GAME_OVER_SCREEN,
    MAIN_MENU
}

