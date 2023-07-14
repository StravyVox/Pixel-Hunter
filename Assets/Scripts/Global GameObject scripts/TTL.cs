using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Time to live script, that calls Action, when TTL is over
/// </summary>
public class TTL : MonoBehaviour
{
    public Action OnTTLEnd;
    [SerializeField] private float _timeToLive;

    private void Start()
    {
        StartCoroutine("Delay");
    }
    IEnumerator Delay()
    {
        yield return new WaitForSeconds(_timeToLive);
        OnTTLEnd?.Invoke();
    }
}
