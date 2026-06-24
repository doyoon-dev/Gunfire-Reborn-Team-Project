using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagedFloor : MonoBehaviour
{
    List<Monster> Damagelist = new List<Monster>();         // 폭탄 장판 위에 있는 몬스터 리스트
    float timer = 0;
    // Start is called before the first frame update
    void OnEnable()
    {
        CancelInvoke("Die");
        Invoke("Die", 3f);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            timer = 0;
            for (int i = 0; i < Damagelist.Count; i++)
            {
                //Damagelist[i].OnDamage(DamageCalculator.TrueDamageCal(100));
                Damagelist[i].OnDamage(DamageCalculator.GrenadeDamage(Damagelist[i]));
            }
            
        }
        
    }

    void Die()
    {
        Managers.Pool.Push(gameObject);
    }

    
    // 폭탄 터질 때 데미지
    private void OnTriggerEnter(Collider other)
    {
        string layerName = LayerMask.LayerToName(other.gameObject.layer);

        if (layerName == "Monster")
        {
            Monster mon = other.gameObject.GetComponent<Monster>();
            if (mon != null)
            {
                Damagelist.Add(mon);
                //mon.OnDamage(DamageCalculator.TrueDamageCal(100));
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        string layerName = LayerMask.LayerToName(other.gameObject.layer);

        if (layerName == "Monster")
        {
            Monster mon = other.gameObject.GetComponent<Monster>();
            if (mon != null)
            {
                Damagelist.Remove(mon);
            }
        }
    }
}
