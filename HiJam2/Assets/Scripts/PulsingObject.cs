using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulsingObject : MonoBehaviour {

    [SerializeField] private float pulseSpeed = 10.0f;
    [SerializeField] private float pulseFloor = 1.0f;
    [SerializeField] private float pulseDepth = 0.2f;
    [SerializeField] private float pulseOffset = 0.5f;

    private Vector3 baseScale;
    private Coroutine cr_Pulse;

    private void Awake()
    {
        
    }

    private void Start()
    {
    }

    public void StartPulse()
    {
        StopPulse();
        cr_Pulse = StartCoroutine(Pulse());
    }

    public void StopPulse()
    {
        if(cr_Pulse != null)
        {
            StopCoroutine(cr_Pulse);
            cr_Pulse = null;
        }
    }

    private IEnumerator Pulse()
    {
        if(baseScale == Vector3.zero) { baseScale = transform.localScale; }

        while (true)
        {
            transform.localScale = baseScale * (((Mathf.Sin(Time.realtimeSinceStartup * pulseSpeed) + pulseFloor) * pulseDepth) + pulseOffset);
            yield return null;
        }
    }
}
