using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpamgeonUI : MonoBehaviour {

    [SerializeField] private GameObject ui;
    [SerializeField] private PulsingObject pulsingSpaceBar;
    [SerializeField] private Text levelText;

	// Use this for initialization
	void Start () {
        GameManager.Instance.GameStateChanged += GameManager_GameStateChanged;
	}

    private void GameManager_GameStateChanged(object sender, GameManager.GameStateChangedArgs e)
    {
        if(e.newState == GameStates.BATTLE_PREP)
        {
            ui.SetActive(true);
            levelText.text = "Level " + GameManager.Instance.CurrentRound;
        } else if(e.newState == GameStates.BATTLING)
        {
            pulsingSpaceBar.gameObject.SetActive(true);
            pulsingSpaceBar.StartPulse();
        } else if(e.newState == GameStates.BATTLE_ENDED)
        {
            pulsingSpaceBar.StopPulse();
            pulsingSpaceBar.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
