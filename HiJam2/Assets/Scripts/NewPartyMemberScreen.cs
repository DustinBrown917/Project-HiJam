using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewPartyMemberScreen : MonoBehaviour {

    [SerializeField] private Image img;    
    [SerializeField] private Text text;
    private Troop troop;

    private void Awake()
    {
    }

    public void SetNewPartyMember(Troop t)
    {
        img.sprite = t.GetSprite();
        text.text = "A new " + t.TroopName + " has joined your party!";
    }
}
