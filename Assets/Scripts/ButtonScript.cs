using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    [SerializeField] GameObject _ObjectToOpen;
    [SerializeField] GameObject _ObjectToClose;

    public void ActivateButton()
    {
        _ObjectToOpen?.SetActive(false);
        _ObjectToClose?.SetActive(false);
    }
}
