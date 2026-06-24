using System.Collections;
using UnityEngine;

public class Chest : InteractiveObject
{
    [SerializeField] Transform m_itemPos;
    [SerializeField] GameObject Item;

    Animator anim;
    [SerializeField] bool m_isLock = true;
    bool m_isOpen = false;


    public void UnLock() { m_isLock = false; }

    void Start()
    {
        m_interactiveString = "열기";
        base.Init();
        anim = GetComponent<Animator>();
        StartCoroutine(OpenChest());
    }

    private void Update()
    {
        if(!m_isLock)
        {
            OnUpdate();
        }
    }

    public virtual IEnumerator OpenChest()
    {
        yield return new WaitUntil(() => m_isOpen);
        anim.SetTrigger("IsOpen");
        yield return new WaitForSeconds(1.5f);
        Managers.Game.Item.DropAllBullet(m_itemPos);
        if(Item!=null)
        {
            Managers.Game.Item.SpawnItem(Item, m_itemPos);
        }
        yield return new WaitForSeconds(0.1f);
        gameObject.GetComponent<Collider>().enabled = false;
        enabled = false;
    }

    public override void Interactive()
    {
        if(!m_isLock)
        {
            m_isOpen = true;
            base.Clear();
        }
    }
}
