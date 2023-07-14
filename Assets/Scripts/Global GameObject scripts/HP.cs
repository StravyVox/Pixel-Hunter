using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Script, that give an object helathPoints
/// Calls Actions when changed HP and when ZeroHP
/// </summary>
public class HP : MonoBehaviour
{
    public Action OnZeroHP;
    public Action<int> OnHPChange;
    public int HealthPoints { get => _healthPoints; set => ChangeHP(value); }
    
    [SerializeField] private int _healthPoints;

    private void ChangeHP(int newHP)
    {
        if(newHP < 0)
        {
            _healthPoints = 0;
            OnZeroHP?.Invoke();
        }
        else
        {
            _healthPoints = newHP;
            OnHPChange?.Invoke(newHP);
        }
    }
}
