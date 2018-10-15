using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Player {

    [SerializeField] private GameObject[] troopRoster;

	// Use this for initialization
	void Start () {
        GameManager.Instance.GameStateChanged += GameManager_GameStateChanged;
	}

    private void GameManager_GameStateChanged(object sender, GameManager.GameStateChangedArgs e)
    {
        if(e.newState == GameStates.BATTLE_PREP)
        {
            DeployTroops();
        }
        else if (e.newState == GameStates.BATTLING)
        {
            AssignTargetsToTroops();
        }
        else if (e.newState == GameStates.SUMMARY_SCREEN)
        {
            foreach (Troop t in troops)
            {
                t.ResetTroop();
            }
        } else if(e.newState == GameStates.GAME_OVER_SCREEN)
        {
            foreach (Troop t in troops)
            {
                Destroy(t.gameObject);
            }
            troops.Clear();
        }
    }

    public Troop GetTroopFromRoster()
    {
        int index = UnityEngine.Random.Range(0, troopRoster.Length);

        return Instantiate(troopRoster[index]).GetComponent<Troop>();
    }

    public void GrantExperienceToAll(int exp)
    {
        foreach(Troop t in troops)
        {
            t.GiveExperience(exp);
        }
    }

    public void AddTroopToParty(Troop troop)
    {
        troops.Add(troop);
        troop.PlayerControlled = true;
    }
}
