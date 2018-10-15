using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public const int MAX_PARTY_SIZE = 5;

    public int CurrentPartySize { get { return troops.Count; } }

    [SerializeField] protected Transform[] deploymentPoints;
    [SerializeField] protected List<Troop> troops;
    [SerializeField] protected Player opposingPlayer;
    [SerializeField] protected bool facingLeft = false;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AssignTargetsToTroops()
    {
        foreach(Troop t in troops)
        {
            t.SetTarget(opposingPlayer.GetLivingTroop());
        }
    }

    public void DeployTroops()
    {
        for(int i = 0; i < troops.Count; i++)
        {
            troops[i].transform.position = deploymentPoints[i].transform.position;
            troops[i].ResetTroop();
            troops[i].KilledTarget += Troop_KilledTarget;
            troops[i].Death += Troop_Death;
            troops[i].UpdateFacing(facingLeft);
        }
    }

    public Troop GetTroop(int index)
    {
        if(index < 0 || index > troops.Count) { return null; }
        return troops[index];
    }

    protected void Troop_Death(object sender, Troop.OnDeathArgs e)
    {
        troops.Remove(e.troop);
        if(troops.Count == 0)
        {
            OnNoTroopsLeft();
        }
    }

    protected void Troop_KilledTarget(object sender,Troop.KilledTargetArgs e)
    {
        Troop t = opposingPlayer.GetLivingTroop();
        if (t != null)
        {
            e.troop.SetTarget(t);
        }
    }

    public Troop GetLivingTroop()
    {
        if(troops.Count == 0) { return null; }
        return troops[UnityEngine.Random.Range(0, troops.Count)];
    }

    public event EventHandler<NoTroopsLeftArgs> NoTroopsLeft;

    public class NoTroopsLeftArgs : EventArgs
    {
        public Player player;

        public NoTroopsLeftArgs(Player player)
        {
            this.player = player;
        }
    }

    protected void OnNoTroopsLeft()
    {
        EventHandler<NoTroopsLeftArgs> handler = NoTroopsLeft;

        if(handler != null)
        {
            handler(this, new NoTroopsLeftArgs(this));
        }
    }
}
