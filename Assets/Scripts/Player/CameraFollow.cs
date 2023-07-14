using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Network script, that attached to Player GameObject
/// If player is owner of the object, camera will follow this object by X coordinates
/// </summary>
public class CameraFollow : NetworkBehaviour
{
    [SerializeField] private float _followSpeed = 1;
    private Camera _camera;
    private Vector3 _position;
    private void Start()
    {
        if (!IsOwner)
        {
            CameraFollow script = GetComponent<CameraFollow>();
            script.enabled = false;
            return;
        };
        _camera = Camera.main;
        _position = transform.position;

    }
    private void FixedUpdate()
    {
        if (_camera != null)
        {
            _position = _camera.transform.position;
            _position.x = Mathf.Lerp(_camera.transform.position.x, transform.position.x, Time.fixedDeltaTime * _followSpeed);
            _camera.transform.position = _position;
        }
        else
        {
            _camera = Camera.main;
        }
    }

}
