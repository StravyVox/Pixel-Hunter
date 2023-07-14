using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollissionController : MonoBehaviour
{
    public Action<GameObject> OnCollissionTriggered;
    [SerializeField] List<string> _triggerTags;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (string triggerTag in _triggerTags)
        {
            if(triggerTag == collision.gameObject.tag)
            {
                InputMessageController controller;
                collision.gameObject.TryGetComponent<InputMessageController>(out controller);
                if (controller != null)
                {
                    Debug.Log("Sendind message");
                    controller.CollissionMessage(this.gameObject);
                }
                OnCollissionTriggered?.Invoke(collision.gameObject);
            }
        }
    }
}
