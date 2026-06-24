using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_NpcTag : UI_WorldSpace
{
    void Update()
    {
        if (gameObject.activeSelf)
        {
            LookAtCamera();
        }
    }
}
