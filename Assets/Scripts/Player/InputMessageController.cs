using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for catching incoming Collission Messages from other objects
/// Used for better Server-Client perfomance
/// </summary>
public class InputMessageController : MonoBehaviour
{
    public Action<GameObject> OnHit;

    public void CollissionMessage(GameObject collissionedObject)
    {
        OnHit?.Invoke(collissionedObject);
    }
}
