using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
    public Action<float> OnHorizontalMove;
    public Action OnMoveEnd;
    public Action OnJump;
    public Action OnShoot;

    public static InputController Singleton;
    [SerializeField] private Button _shootButton;
    [SerializeField] private Button _jumpButton;
    [SerializeField] private GameObject _joystickObject;
    private FloatingJoystick _joystickHandler;
    private bool _touch = false;

    private void Awake()
    {
        _jumpButton.onClick.AddListener(() => { OnJump?.Invoke(); });
        _shootButton.onClick.AddListener(() => { OnShoot?.Invoke(); });
        if (Singleton == null)
        {
            Singleton = this;
        }
        else if (Singleton == this) { 
            Destroy(gameObject); 
        }
        if (_joystickObject != null)
        {
          _joystickHandler = _joystickObject.GetComponent<FloatingJoystick>();
        }
        if(_joystickHandler != null)
        {
            _joystickHandler.OnPointerDownHandle = () => _touch = true;
            _joystickHandler.OnPointerUpHandle = () =>
            {
                _touch = false;
                OnMoveEnd?.Invoke();
            };
        }
    }
    void Update()
    {
        if (_touch)
        {
            OnHorizontalMove?.Invoke(_joystickHandler.Horizontal);
        }
    }
}
