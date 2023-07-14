using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Main Player script, that connect all logic between scripts on the Player gameObject
/// Also logic is depended by the owner of the gameObject.
/// For OWNER of the object player CharacterController will connect to InputController and will be able to move
/// FOR HOST(SERVER) CharacterController provides logic, that will call Collission functions 
/// </summary>
public class CharacterController : NetworkBehaviour
{
    [SerializeField] private MovementController _movementController;
    [SerializeField] private ShootController _shootController;
    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerCollissionController _playerCollissionController;
    [SerializeField] private InputMessageController _messageController;
    [SerializeField] private HP _healthPointsController;
    private InputController _inputController;
    private NetworkObject _networkObject;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _networkObject = GetComponent<NetworkObject>();
        if (IsOwner) //Only owner of object can move player object
        {
            _inputController = InputController.Singleton;
            _inputController.OnHorizontalMove = (float direction) =>
            {
                if (!IsOwner) return;
                _movementController?.Move(direction);
                _animator?.SetBool("Move", true);  
            };
            _inputController.OnMoveEnd = () => _animator?.SetBool("Move", false);
            _inputController.OnJump = () => _movementController?.Jump();
            _inputController.OnShoot = () => _shootController?.ShootServerRpc(transform.rotation.eulerAngles.y<180?1:-1);
        }
        if (IsHost) //Only host will control collission Logic
        {
            _playerCollissionController.OnDamageTaken = (value) => { ChangeHPClientRpc(value); };
            _playerCollissionController.OnCoinTaken = () => { SessionManager.Singleton.AddScore(_networkObject.OwnerClientId); };
            _messageController.OnHit = _playerCollissionController.CheckCollissionedObject;
        }
        
    }
    /// <summary>
    /// SERVER function, that changes HP of Player on all connected machines
    /// </summary>
    /// <param name="value"></param>
    [ClientRpc]
    private void ChangeHPClientRpc(int value)
    {
        _healthPointsController.HealthPoints += value;
    }
}
