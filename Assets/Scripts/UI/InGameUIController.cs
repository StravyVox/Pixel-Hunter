using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Controller that provides logic for Game UI items
/// 1. Logic for exit button
/// 2. Setting and showing end info board
/// </summary>

public class InGameUIController : MonoBehaviour
{
    public Action OnExitButton;

    [SerializeField] private TextMeshProUGUI _resultScoreInfo;
    [SerializeField] private GameObject _winScoreBoardUILayout;
   

  
    public void SetWinResults(int player, int score)
    {
        _resultScoreInfo.text = "Player " + player + " won. Collected " + score + " coins";
    }
    
    public void ShowWinScoreBoard()
    {
        _winScoreBoardUILayout.SetActive(true);
    }
    public void Exit()
    {
        OnExitButton?.Invoke();
    }
}
