using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GimmickTrigger : MonoBehaviour
{
    Room m_room;
    bool m_isWork = false;

    BoxCollider m_collider;

    void Start()
    {
        if (m_collider == null) m_collider = GetComponent<BoxCollider>();
        m_collider.isTrigger = true;

        m_room = transform.parent.GetComponent<Room>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Managers.Game?.IsPlay==true &&
            other.gameObject.layer == (int)Define.Layer.Player &&
            m_isWork == false)
        {
            m_isWork = true;
            if (m_room != null) m_room.FirstEnterRoom();
            else Debug.LogError(name + ":null room");
        }
    }

    private void OnDrawGizmos()
    {
        if (m_collider == null) m_collider = GetComponent<BoxCollider>();
        if (m_collider)
        {
            Gizmos.color = Color.blue;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(m_collider.center, m_collider.size);
        }
    }
}
