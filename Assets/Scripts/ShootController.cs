using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class ShootController : NetworkBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _spawnPosition;
    [SerializeField] private float _reloadTime;
    private bool _abilityToShoot = true;


    [ServerRpc]
    public void ShootServerRpc(float direction)
    {
        if (!_abilityToShoot) return;
        _abilityToShoot = false;
        GameObject bullet = Instantiate(_bulletPrefab);
        bullet.transform.position = _spawnPosition.position;
        bullet.GetComponent<FixedMoveByX>().Direction = direction;
        StartCoroutine("Reload");
    }
    IEnumerator Reload()
    {
        yield return new WaitForSeconds(_reloadTime);
        _abilityToShoot = true;
    }
}
