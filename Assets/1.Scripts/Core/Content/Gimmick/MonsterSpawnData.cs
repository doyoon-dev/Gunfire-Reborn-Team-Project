using UnityEngine;

public class MonsterSpawnData : MonoBehaviour
{
    [SerializeField] GameObject m_prefab;

    public GameObject Prefab { get => m_prefab; }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 2.0f);
    }
}
