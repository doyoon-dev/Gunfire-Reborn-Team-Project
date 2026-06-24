using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_NumberString : MonoBehaviour
{
    [SerializeField] Define.TextUIType m_type= Define.TextUIType.OutLine;

    [SerializeField] Transform m_numberRoot;
    [SerializeField] float m_stringSize;
    List<RectTransform> m_numberList = new List<RectTransform>();

    private void Start()
    {
        SetStringSize(m_stringSize);
    }

    public void SetStringColor(Color color)
    {
        foreach (var item in m_numberList)
        {
            Image num = Util.FindChild<Image>(item.gameObject);
            num.color = color;
        }
    }
    public void SetStringSize(float size)
    {
        transform.localScale = Vector2.one * size;
    }

    public void ChangeNumber(int num)
    {
        ChangeString(num.ToString());
    }

    public void ChangeString(string str)
    {
        if (str.Length > m_numberList.Count)
        {
            int count = str.Length - m_numberList.Count;
            for (int i = 0; i < count; i++)
            {
                m_numberList.Add(Managers.Resource.Instantiate<RectTransform>(Define.Path.UI_Number, m_numberRoot));
            }
        }

        for (int i = 0; i < m_numberList.Count; i++)
        {
            if (str.Length <= i)
            {
                m_numberList[i].gameObject.SetActive(false);
            }
            else
            {
                m_numberList[i].gameObject.SetActive(true);
                Image image = m_numberList[i].transform.GetChild(0).GetComponent<Image>();
                image.sprite = Managers.Resource.Load<Sprite>(
                    (m_type == Define.TextUIType.OutLine ? Define.Path.TextSprite_Number : Define.Path.TextSprite_Number_NoneOutLine)
                    + (str[i] == '/' ? '-' : str[i]));
                image.SetNativeSize();
            }
        }
    }
}
