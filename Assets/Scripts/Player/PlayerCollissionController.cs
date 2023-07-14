using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller, that checks, with what objects player collissioned
/// Depending on the answer calls special Actions 
/// </summary>

public class PlayerCollissionController : MonoBehaviour
{
    public Action<int> OnDamageTaken;
    public Action OnCoinTaken;
   public void CheckCollissionedObject(GameObject collissionObject)
    {
        switch (collissionObject.tag)
        {

            case "Bullet": OnDamageTaken?.Invoke(-1);
                break;
            case "Coin": OnCoinTaken?.Invoke(); 
                break;
        }
    }
}
