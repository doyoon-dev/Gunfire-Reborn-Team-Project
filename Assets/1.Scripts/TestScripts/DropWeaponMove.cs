using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropWeaponMove : MonoBehaviour
{
    [SerializeField]
    Camera m_cam;

    public static bool isMove = false;

    Rigidbody m_rigid;

    float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        m_rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isMove)
        {
            DropMove();
        }
        
    }

    void DropMove()
    {
        m_rigid.isKinematic = false;
        timer += Time.deltaTime;
        if (timer < 0.5f)
        {
            m_rigid.velocity = -transform.up * 3f;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        string layerName = LayerMask.LayerToName(collision.gameObject.layer);

        if (layerName == "Ground")
        {
            isMove = false;
            m_rigid.velocity = Vector3.zero;
        }
    }
}
