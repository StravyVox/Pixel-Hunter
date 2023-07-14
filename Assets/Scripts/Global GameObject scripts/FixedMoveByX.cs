using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Script, that move object by X coordinates in selected Direction
//
/// </summary>
public class FixedMoveByX : MonoBehaviour
{
   
    public float Direction { get => _direction; set => ChangeDirection(value); }

    [SerializeField] private float _speed;
    [SerializeField] private float _direction = 0;
    private Vector3 _position;

    private void Start()
    {
        _position = transform.position;
    }
    private void FixedUpdate()
    {
        _position = Vector3.zero;
        _position.x = _speed * _direction * Time.deltaTime;
        transform.position += _position;
    }
    /// <summary>
    /// Checks income value and change direction
    /// Also rotate object because of 2d Sprite Game
    /// </summary>
    /// <param name="value"></param>
    private void ChangeDirection(float value)
    {
        _direction = value < 0 ? -1 : 1;
        Vector3 rotation = Vector3.zero;
        rotation.y = value < 0 ? 180 : 0;
        Quaternion rotationQuaternion = new Quaternion();
        rotationQuaternion.eulerAngles = rotation;
        transform.rotation = rotationQuaternion;
    }
}
