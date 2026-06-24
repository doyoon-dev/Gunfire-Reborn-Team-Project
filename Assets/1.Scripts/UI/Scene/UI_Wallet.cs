using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Wallet : UI_Scene
{
    [SerializeField] GameObject m_numberOrigin;
    [SerializeField] Transform m_goldNumberRoot;
    List<GameObject> m_numberList = new List<GameObject>();

    public override void Init()
    {
        base.Init();
    }

    public void ChangeCount(int value)
    {
        string str = value.ToString();

        if(str.Length > m_numberList.Count)
        {
            int count = str.Length - m_numberList.Count;
            for (int i = 0; i < count; i++)
            {
                m_numberList.Add(Instantiate(m_numberOrigin, m_goldNumberRoot));
            }
        }


        for (int i = 0; i < m_numberList.Count; i++)
        {
            if (str.Length <= i)
            {
                m_numberList[i].SetActive(false);
            }
            else
            {
                m_numberList[i].SetActive(true);
                Image image = m_numberList[i].transform.GetChild(0).GetComponent<Image>();
                image.sprite = Managers.Resource.Load<Sprite>(Define.Path.TextSprite_Number + str[i]);
                image.SetNativeSize();
            }
        }
    }
}
