using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            Debug.Log("Hp changed" + newHP);
            Debug.Log(OnHPChange);
            OnHPChange?.Invoke(newHP);
        }
    }
}
