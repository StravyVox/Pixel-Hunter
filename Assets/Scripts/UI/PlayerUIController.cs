using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Controller, that controlls progress bar and color of player
/// </summary>
public class PlayerUIController : MonoBehaviour
{
    [SerializeField] ProgressBar _progressBar;
    [SerializeField] List<Color> _playerColors;

    /// <summary>
    /// Gets info about HP and sets main properties for progress bar
    /// On start gets networkObject of Player and sets color depending on player ID
    /// </summary>
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
    /// <summary>
    /// Setting color for 2d Sprite renderer depending on id of player
    /// </summary>
    /// <param name="value"></param>
    private void SetColors(int value)
    {
        SpriteRenderer spriteRenderer = GetComponentInParent<SpriteRenderer>();
        while (value > _playerColors.Count)
        {
            value -= _playerColors.Count;
        }
        spriteRenderer.color = _playerColors[value];
        
    }
    /// <summary>
    /// Changes filling parameter of progress bar
    /// </summary>
    /// <param name="value"></param>
    public void UpdateProgressBar(int value)
    {
        if (_progressBar != null)
        {
            _progressBar.CurrentAmount = value;
        }
    }
}
