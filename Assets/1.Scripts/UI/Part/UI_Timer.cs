using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Timer : MonoBehaviour
{
    Text m_text;

    void Start()
    {
        m_text = transform.GetChild(0).GetComponent<Text>();
    }
    void Update()
    {
        SetTimer();
    }

    private void SetTimer()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(Managers.Game.GetGameTime);
        string answer = string.Format("{0:D1}m {1:D1}s",
                      timeSpan.Minutes,
                      timeSpan.Seconds);
        m_text.text = answer;
    }
}
