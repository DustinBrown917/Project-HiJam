using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenu : MonoBehaviour {

    private static GameMenu _instance;
    public static GameMenu Instance { get { return _instance; } }

    [SerializeField] private GameObject gameMenuScreen;
    [SerializeField] private GameObject newPartyMemberScreen;
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject lootScreen;
    [SerializeField] private GameObject gameOverScreen;

    private HumanPlayer humanPlayer;
    private GameObject activeScreen;
    private bool waitingForPlayerToProgress = true;
    public bool GiveNewPartyMember { get; set; }

    private void Awake()
    {
        if(_instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
    }

    // Use this for initialization
    void Start () {
        humanPlayer = (HumanPlayer)GameManager.Instance.HumanPlayer;
        GameManager.Instance.GameStateChanged += GameManager_GameStateChanged;
        lootScreen.GetComponent<LootScreen>().ExperienceEventDone += GameMenu_ExperienceEventDone;
        SetActiveScreen(gameMenuScreen);
	}

    private void GameMenu_ExperienceEventDone(object sender, System.EventArgs e)
    {
        waitingForPlayerToProgress = true;
    }

    private void GameManager_GameStateChanged(object sender, GameManager.GameStateChangedArgs e)
    {
        if(e.newState == GameStates.BATTLE_ENDED)
        {
            if(humanPlayer.CurrentPartySize > 0)
            {
                SetActiveScreen(victoryScreen);
            } else
            {
                SetActiveScreen(gameOverScreen);
            }
            
        } 
    }

    // Update is called once per frame
    void Update () {
        if(!waitingForPlayerToProgress) { return; }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(GameManager.Instance.CurrentState == GameStates.MAIN_MENU || GameManager.Instance.CurrentState == GameStates.SUMMARY_SCREEN)
            {
                if (activeScreen == victoryScreen)
                {
                    SetActiveScreen(lootScreen);
                    waitingForPlayerToProgress = false;
                }
                else if (GiveNewPartyMember)
                {
                    SetActiveScreen(newPartyMemberScreen);
                    GivePlayerNewPartyMember();
                }
                else if(activeScreen == gameOverScreen)
                {
                    GameManager.Instance.ResetRoundCount();
                    SetActiveScreen(gameMenuScreen);
                }
                else
                {
                    GameManager.Instance.ChangeGameState(GameStates.BATTLE_PREP);
                }
            }
        }
	}

    private void GivePlayerNewPartyMember()
    {
        Troop t = humanPlayer.GetTroopFromRoster();
        newPartyMemberScreen.GetComponent<NewPartyMemberScreen>().SetNewPartyMember(t);
        humanPlayer.AddTroopToParty(t);
        GiveNewPartyMember = false;
    }

    private void SetActiveScreen(GameObject screen)
    {
        if(activeScreen != null)
        {
            activeScreen.SetActive(false);
        }


        activeScreen = screen;

        if(activeScreen == gameMenuScreen) { GiveNewPartyMember = true; }

        activeScreen.SetActive(true);
    }
}
