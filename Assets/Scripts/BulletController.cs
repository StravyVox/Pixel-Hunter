using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BulletController : NetworkBehaviour
{
    [SerializeField] private FixedMoveByX _movementController;
    [SerializeField] private List<string> _triggerTags;
    [SerializeField] private NetworkObject _networkObject;

    private void Start()
    {
        if (!IsOwner) return;
        _networkObject?.Spawn();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collission2D Bullet");
        foreach (string tag in _triggerTags)
        {
            if(collision.gameObject.tag == tag || _networkObject!=null)
            {
                _networkObject.Despawn(); 
                Destroy(gameObject);

            }
        }
    }
}
