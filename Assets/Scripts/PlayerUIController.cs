using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] ProgressBar _progressBar;
    [SerializeField] List<Color> _playerColors;

    private void Start()
    {
        HP _healthPoints = GetComponentInParent<HP>();
        if (_healthPoints != null)
        {
            _progressBar.MaxAmount = _healthPoints.HealthPoints;
            _progressBar.CurrentAmount = _healthPoints.HealthPoints;
            _healthPoints.OnHPChange += (newHP) => { UpdateProgressBar(newHP); };
        }
        NetworkObject networkObject = GetComponentInParent<NetworkObject>();
        if (networkObject != null)
        {
            SetColors((int)networkObject.OwnerClientId);
        }
    }
    private void SetColors(int value)
    {
        SpriteRenderer spriteRenderer = GetComponentInParent<SpriteRenderer>();
        while (value > _playerColors.Count)
        {
            value -= _playerColors.Count;
        }
        spriteRenderer.color = _playerColors[value];
        
    }
    public void UpdateProgressBar(int value)
    {
        if (_progressBar != null)
        {
            _progressBar.CurrentAmount = value;
        }
    }
}
