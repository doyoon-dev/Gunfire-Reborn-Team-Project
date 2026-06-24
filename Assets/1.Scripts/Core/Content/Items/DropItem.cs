using System.Collections;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    [SerializeField] Define.DropItemType m_type;

    float m_heightForce = 5f;
    float m_widthForce = 4f;

    bool m_ismoved = false;

    Rigidbody m_rigid;

    public Define.DropItemType Type => m_type;


    public void Init(Transform  _transform)
    {
        transform.localPosition = new Vector3(0,1.5f,0);
        transform.parent = _transform;
        m_ismoved = false;

        if (m_rigid == null) m_rigid = GetComponent<Rigidbody>();
        Spread();
    }

    public void Spread()//Fisher–Yates shuffle 알고리즘 - 나중에 적용
    {
        m_rigid.velocity = Vector3.zero;
        m_rigid.angularVelocity = Vector3.zero;

        Vector3 randDir = Random.insideUnitSphere.normalized * Random.Range(2f, m_widthForce);
        randDir.y = m_heightForce;
        m_rigid.AddForce(randDir, ForceMode.Impulse);
        if(m_type==Define.DropItemType.Gold) m_rigid.AddTorque(randDir, ForceMode.Impulse);
    }

    IEnumerator MoveToPlayer()
    {
        m_ismoved = true;

        float time = 0;
        Vector3 playerPosition = Managers.Game.Player.transform.position+ Managers.Game.Player.transform.forward;
        playerPosition.y -= 0.5f;
        while (Vector3.Distance(transform.position, playerPosition) >= 1.5f)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, playerPosition, time);
            yield return null;
        }
        PickUpItem();
    }
    void PickUpItem()
    {
        Managers.Game.PickUpItem(this);
        Managers.Game.Item.DeSpawn(this);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == (int)Define.Layer.Player)
        {
            if (m_ismoved) return;
            switch (m_type)
            {
                case Define.DropItemType.Grenade:
                    if (WeaponManager.m_current.m_curGrenade == WeaponManager.m_current.m_maxGrenade) return;
                    break;
                case Define.DropItemType.Bullet_Green:
                case Define.DropItemType.Bullet_Blue:
                case Define.DropItemType.Bullet_Yellow:
                    if (WeaponManager.m_current.IsBulletFull(m_type)) return;
                    break;
            }
            StartCoroutine(MoveToPlayer());
        }
    }
}
