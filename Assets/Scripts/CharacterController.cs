using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterController : NetworkBehaviour
{
    [SerializeField] private MovementController _movementController;
    [SerializeField] private ShootController _shootController;
    [SerializeField] private Animator _animator;
    private InputController _inputController;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) return;
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
}
