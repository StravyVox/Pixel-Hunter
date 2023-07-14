using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMessageController : MonoBehaviour
{
    public Action<GameObject> OnHit;

    public void CollissionMessage(GameObject collissionedObject)
    {
        Debug.Log("We recieved message");
        Debug.Log("OnHit" + OnHit);
        OnHit?.Invoke(collissionedObject);
    }
}
