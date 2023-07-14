using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CoinsPool : MonoBehaviour
{
    [SerializeField] private GameObject _coinPrefab;
    [SerializeField] private GameObject _generationPoint;
    private List<GameObject> _pool;
    private void Start()
    {
        _pool = new List<GameObject>();
       

    }
    public GameObject GetCoin()
    {
        foreach (GameObject coin in _pool)
        {
            if (!coin.activeInHierarchy)
            {
                coin.SetActive(true);
                return coin;
            }
        }
        GameObject generatedCoin = Instantiate(_coinPrefab, _generationPoint.transform);
        _pool.Add(generatedCoin);
        return generatedCoin;
    }
    

}
