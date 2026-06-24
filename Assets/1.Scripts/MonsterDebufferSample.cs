using UnityEngine;

public class MonsterDebufferSample : MonoBehaviour
{
    public bool asdfdsf=false;
    Monster monster;
    void Start()
    {
        //태그"MON" 테스트용
        monster = GameObject.FindWithTag("MON").GetComponent<Monster>();
        //monster.ApplyDeBuff(Define.ElementalType.Corrosion, true);
        monster.ApplyDeBuff(true);
    }    
    
    void Update()
    {
        //if(asdfdsf) monster.ApplyDeBuff(Define.ElementalType.Corrosion, true);
        if (asdfdsf) monster.ApplyDeBuff(true);
        //else monster.ApplyDeBuff(Define.ElementalType.Corrosion, false);
        else monster.ApplyDeBuff(false);
    }    
}
