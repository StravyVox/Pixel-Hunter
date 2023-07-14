using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Controller, that provides logic for shooting ability
/// IT's network object, so all logic is going to happen on Server side
/// </summary>
public class ShootController : NetworkBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _spawnPosition;
    [SerializeField] private float _reloadTime;
    private bool _abilityToShoot = true;


    /// <summary>
    /// CLIENT method, that calls Server to shoot and spawn bullet
    /// Method instantiates bullet prefab and configures some settings:
    /// 1. Setting the direction of bullet fly
    /// 2. Setting the delegate when TTL ends
    /// 3. Setting the delegate when bullet will face the player
    /// 4. Spawning the bullet itself [ONLY SERVER can spawn, that's why it's ServerRpc]
    /// </summary>
    /// <param name="direction"></param>
    [ServerRpc]
    public void ShootServerRpc(float direction)
    {
        if (!_abilityToShoot) return;
        _abilityToShoot = false;
        GameObject bullet = Instantiate(_bulletPrefab);
        bullet.transform.position = _spawnPosition.position;
        bullet.GetComponent<FixedMoveByX>().Direction = direction;
        bullet.GetComponent<TTL>().OnTTLEnd = () =>
        {
            bullet.GetComponent<NetworkObject>().Despawn();
        };
        bullet.GetComponent<CollissionController>().OnCollissionTriggered = (player) => {
            
            bullet.GetComponent<NetworkObject>().Despawn();
        };
        bullet.GetComponent<NetworkObject>().Spawn();
        StartCoroutine("Reload");
    }
    /// <summary>
    /// Reload coroutine that set abilitytoshoot after the delay
    /// </summary>
    /// <returns></returns>
    IEnumerator Reload()
    {
        yield return new WaitForSeconds(_reloadTime);
        _abilityToShoot = true;
    }
}
