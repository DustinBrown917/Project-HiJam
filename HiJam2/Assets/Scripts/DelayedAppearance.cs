using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedAppearance : MonoBehaviour {

    [SerializeField] private float waitTime = 2.0f;
    [SerializeField] private Behaviour[] componentsToAppear;

    private void OnEnable()
    {
        for (int i = 0; i < componentsToAppear.Length; i++)
        {
            componentsToAppear[i].enabled = false;
        }
        StartCoroutine(DelayedAppearanceCoroutine());
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private IEnumerator DelayedAppearanceCoroutine()
    {
        yield return new WaitForSeconds(waitTime);
        for(int i = 0; i < componentsToAppear.Length; i++)
        {
            componentsToAppear[i].enabled = true;
        }
    }
}
