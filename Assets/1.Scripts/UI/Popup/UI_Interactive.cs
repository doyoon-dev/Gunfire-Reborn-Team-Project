using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_Interactive : MonoBehaviour
{
    [SerializeField] Text m_interactiveText;

    public void SetText(string text)
    {
        m_interactiveText.text = text;
    }
}
