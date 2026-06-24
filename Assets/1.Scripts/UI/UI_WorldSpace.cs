using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_WorldSpace : UI_Base
{

    public override void Init() { }

    protected void LookAtCamera()
    {
        transform.LookAt(Camera.main.transform);
        transform.Rotate(Vector3.up, -180.0f);
    }

    private void CanvasScaleChange()
    {
        //RectTransform l_hpbarRect = Get<Image>((int)Images.hpBarBackground).GetComponent<RectTransform>();
        //Vector2 l_newCanvasSize = l_hpbarRect.sizeDelta * l_hpbarRect.localScale.x;

        //GetComponent<RectTransform>().sizeDelta = l_newCanvasSize;
    }
}
