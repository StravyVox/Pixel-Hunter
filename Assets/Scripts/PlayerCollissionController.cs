using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollissionController : MonoBehaviour
{
    public Action<int> OnDamageTaken;
    public Action OnCoinTaken;
   public void CheckCollissionedObject(GameObject collissionObject)
    {
        Debug.Log("CheckColObjectActivated");
        switch (collissionObject.tag)
        {

            case "Bullet": OnDamageTaken?.Invoke(-1);
                Debug.Log("itwasBullet");

                break;
            case "Coin": OnCoinTaken?.Invoke(); 
                break;
        }
    }
}
