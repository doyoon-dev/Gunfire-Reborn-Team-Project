using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainToggle : MonoBehaviour
{
    void Start()
    {
     
    }
    public void EnterPointer()
    {                
        GetComponent<UnityEngine.UI.Toggle>().isOn = true;
    }
    public void ExitPointer()
    {
        GetComponent<UnityEngine.UI.Toggle>().isOn = false;
    }
}
