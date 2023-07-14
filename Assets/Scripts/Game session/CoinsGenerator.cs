using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;


//Main purpose is to generate coins
public class CoinsGenerator : NetworkBehaviour
{


    [SerializeField] int _amountOfSecondsDelay;
    [SerializeField][Range(1, 45)] private float _generationXRange;
    [SerializeField]private CoinsPool _poolOfObjects;
    [SerializeField]private int _maxAmountOfCoins;
    [SerializeField]private GameObject _generationPoint;

    private bool _generate = false;
    private int _generatedCoins = 0;

    /// <summary>
    /// Methods to stop/start generate. Depends on Host. Only host(server) can spawn objects;
    /// </summary>
    public void StartGenerate()
    {
        if (!IsHost) return;    
        _generate = true;
        StartCoroutine("GenerateCoins");
    }
    public void StopGenerate()
    {
        _generate = false;

    }
    private void Start()
    {
        if (!IsHost)
        {
            this.enabled = false;
        }
    }
    //Delay for coin spawn
    private IEnumerator GenerateCoins()
    {
        while (_generate)
        {
            yield return new WaitForSeconds(_amountOfSecondsDelay);
            GenerateCoin();
        }
    }
    /// <summary>
    /// Only host will call this function
    /// Generate Coin by finding random range and getting coin from pool
    /// </summary>
    private void GenerateCoin() 
    {

        int direction = Random.Range(-1, 2);
        float range = Random.Range(0, _generationXRange + 1);
        Vector3 startPosition = _generationPoint.transform.position;
        startPosition.x = direction < 0 ? -1 : 1 * range;
        GameObject generatedCoin = _poolOfObjects.GetCoin();
        generatedCoin.transform.position = startPosition;
        generatedCoin.GetComponent<CollissionController>().OnCollissionTriggered = (obj) =>
        {
            generatedCoin.SetActive(false);
            generatedCoin.GetComponent<NetworkObject>().Despawn(false);
            _generatedCoins--;
        };
        generatedCoin.GetComponent<NetworkObject>().Spawn();
        _generatedCoins++;
    }
}
