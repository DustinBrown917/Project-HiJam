using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerPlayer : Player {

    [SerializeField] private GameObject[] troopRoster;


	// Use this for initialization
	void Start () {
        GameManager.Instance.GameStateChanged += GameManager_GameStateChanged;
	}

    private void GameManager_GameStateChanged(object sender, GameManager.GameStateChangedArgs e)
    {
        if(e.newState == GameStates.BATTLE_PREP)
        {
            BuildLineUp();
            DeployTroops();
        } else if( e.newState == GameStates.BATTLING)
        {
            AssignTargetsToTroops();
        } else if(e.newState == GameStates.SUMMARY_SCREEN || e.newState == GameStates.GAME_OVER_SCREEN)
        {
            foreach(Troop t in troops)
            {
                Destroy(t.gameObject);
            }
            troops.Clear();
        }
    }


    private void BuildLineUp()
    {
        int numOfTroopsToDeploy;

        if(GameManager.Instance.CurrentRound > 4)
        {
            numOfTroopsToDeploy = UnityEngine.Random.Range(1, 6);
        } else
        {
            numOfTroopsToDeploy = UnityEngine.Random.Range(1, GameManager.Instance.CurrentRound + 1);
        }

        int index;
        for(int i = 0; i < numOfTroopsToDeploy; i++)
        {
            index = UnityEngine.Random.Range(0, troopRoster.Length);
            Troop t = Instantiate(troopRoster[index]).GetComponent<Troop>();
            troops.Add(t);
        }

        int totalExpToDole = (int)(((GameManager.Instance.CurrentRound - 1) * GameManager.Instance.CurrentRound) * 0.5f) * Troop.EXP_PER_LEVEL;
        int expToDole;
        foreach(Troop t in troops)
        {
            expToDole = UnityEngine.Random.Range(0, totalExpToDole);
            t.GiveExperience(expToDole);
            totalExpToDole -= expToDole;
        }

        troops[0].GiveExperience(totalExpToDole);
    }
}
