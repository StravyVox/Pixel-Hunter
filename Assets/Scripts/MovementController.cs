using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
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
    private bool CheckGround()
    {
        return Physics2D.BoxCast(transform.position, _boxSize, 0, -transform.up, _maxDistance, _layerMask);
    }
}
