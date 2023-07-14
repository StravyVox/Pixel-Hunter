using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

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
        if (IsOwner)
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
        if (IsHost)
        {
            _playerCollissionController.OnDamageTaken = (value) => { ChangeHPClientRpc(value); };
            _playerCollissionController.OnCoinTaken = () => { SessionManager.Singleton.AddScore(_networkObject.OwnerClientId); };
            _messageController.OnHit = _playerCollissionController.CheckCollissionedObject;
        }
        
    }
    [ClientRpc]
    private void ChangeHPClientRpc(int value)
    {
        _healthPointsController.HealthPoints += value;
    }
}
