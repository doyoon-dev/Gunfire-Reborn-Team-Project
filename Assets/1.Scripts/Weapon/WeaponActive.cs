using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponActive : MonoBehaviour
{
    [SerializeField]
    GameObject m_dropEffect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DropWeaponActive();
    }
    void DropWeaponActive()
    {
        if (transform.parent == null)
        {
            m_dropEffect.SetActive(true);
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
            transform.GetChild(2).gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
        }
        else
        {
            if (transform.parent.name == "Hand")
            {
                m_dropEffect.SetActive(false);
            }
            else
            {
                m_dropEffect.SetActive(true);
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(1).gameObject.SetActive(true);
                transform.GetChild(2).gameObject.SetActive(false);
                transform.rotation = Quaternion.identity;
            }
        }
    }
}
