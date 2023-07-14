using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Typical Unity pool object class for prefab
/// </summary>
public class CoinsPool : MonoBehaviour
{
    [SerializeField] private GameObject _coinPrefab;
    [SerializeField] private GameObject _generationPoint;
    private List<GameObject> _pool;
    private void Start()
    {
        _pool = new List<GameObject>();
    }
    /// <summary>
    /// Finds inactive but created gameobjects or create them and add to the list
    /// </summary>
    /// <returns>Coin gameobject</returns>
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
