using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Controller, thar provides functions for moving gameObject such as
/// 1. Jump function
/// 2. Move function in X direction
/// </summary>
public class MovementController : NetworkBehaviour
{
    public Action OnMove;

    [SerializeField] private float _movementSpeed;
    [SerializeField] private Vector2 _boxSize;
    [SerializeField] private float _maxDistance;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Rigidbody2D _body;
    [SerializeField] private float _jumpForce;
    private bool _ableToJump = true;

    public void Jump()
    {
        if (_ableToJump)
        {
            _ableToJump=false;
            _body.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        }
    }
    /// <summary>
    /// Move function in X direction
    /// also changes rotation info due to 2D sprites of objects
    /// </summary>
    /// <param name="direction"></param>
    public void Move(float direction)
    {
        OnMove?.Invoke();
        Vector3 rotation = new Vector3(0, direction < 0 ? 180 : 0, 0);
        Vector3 moveDirection = new Vector3(0, 0, 0);
        moveDirection.x = direction;
        transform.localRotation = Quaternion.Euler(rotation);
        transform.position += moveDirection*_movementSpeed * Time.deltaTime;
    }
    private void FixedUpdate()
    {
       _ableToJump = CheckGround();
    }
    /// <summary>
    /// Function for checking, if object with these Box paramters on Ground
    /// </summary>
    /// <returns></returns>
    private bool CheckGround()
    {
        return Physics2D.BoxCast(transform.position, _boxSize, 0, -transform.up, _maxDistance, _layerMask);
    }
}
