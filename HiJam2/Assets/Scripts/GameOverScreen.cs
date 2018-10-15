using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour {

    [SerializeField] private Text levelText;

    private void OnEnable()
    {
        levelText.text = "Your party made it to level " + GameManager.Instance.CurrentRound + "."; 
    }
}
