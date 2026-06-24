using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckDropWeapon : MonoBehaviour
{
    [SerializeField]
    Camera m_cam;

    RaycastHit m_hit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckWeapon();
    }

    void CheckWeapon()
    {
        int weaponMask = LayerMask.GetMask("Weapon");
        if (Physics.Raycast(m_cam.transform.position, m_cam.transform.forward, out m_hit, 5f, weaponMask))
        {
            WeaponManager.m_current.m_weaponInfo.InfoWindowOnOff(true);
            WeaponManager.m_current.m_weaponInfo.ShowGunInfo(m_hit.collider.gameObject);
            // 무기 정보 UI 띄우기
            //m_test.SetActive(true);
            // F 키 누르면 무기 변경
            if (Input.GetKeyDown(KeyCode.F))
            {
                //DropWeaponMove.isMove = true;
                WeaponManager.m_current.DropWeaponChange(m_hit.collider.gameObject);
            }
        }
        else
        {
            WeaponManager.m_current.m_weaponInfo.InfoWindowOnOff(false);
        }
    }
}
