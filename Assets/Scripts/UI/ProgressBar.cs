using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Progress bar script
/// Provides function to change the filling of progress bar
/// </summary>
public class ProgressBar : MonoBehaviour
{
    [SerializeField] private RectTransform _imageFiller;
    [SerializeField] private float _maxAmount = 0;
    [SerializeField] private float _currentAmount = 0;
    
    public float CurrentAmount { get { return _currentAmount; } set { _currentAmount = value; ChangeValue(); } }

    public float MaxAmount { get => _maxAmount; set => _maxAmount = value; }

    void Start()
    {
        ChangeValue();
    }
    void ChangeValue()
    {
        float scale = _currentAmount / MaxAmount;
        if (scale <= 1)
        {
            _imageFiller.localScale = new Vector3(scale, _imageFiller.localScale.y, _imageFiller.localScale.z);
        }
    }

}
