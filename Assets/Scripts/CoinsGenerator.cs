using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class CoinsGenerator : NetworkBehaviour
{

    [SerializeField] int _amountOfSecondsDelay;
    [SerializeField][Range(1, 45)] private int _generationXRange;
    [SerializeField]private CoinsPool _poolOfObjects;
    [SerializeField]private int _maxAmountOfCoins;
    [SerializeField]private GameObject _generationPoint;
    private bool _generate = false;
    private int _generatedCoins = 0;
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
    

    private IEnumerator GenerateCoins()
    {
        while (_generate)
        {
            yield return new WaitForSeconds(_amountOfSecondsDelay);
            GenerateCoin();
        }
    }
    private void GenerateCoin()
    {

        int direction = Random.Range(-1, 2);
        int range = Random.Range(0, _generationXRange + 1);
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
