using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP : MonoBehaviour
{
    public Action OnZeroHP;
    public int HealthPoints { get => _healthPoints; set => ChangeHP(value); }
    
    private int _healthPoints;

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
        }
    }
}
