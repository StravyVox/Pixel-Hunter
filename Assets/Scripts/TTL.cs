using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
