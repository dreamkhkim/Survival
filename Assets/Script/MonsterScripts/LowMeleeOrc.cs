using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;


public class LowMeleeOrc : Orc
{
   
    public override void Init()
    {
        m_data.hp = 10;
        m_data.Attack = 2;
        m_data.attackableRange = 2;
        m_data.defectiveRange = 5;

        m_data.animDead = Animator.StringToHash("MonsterDead");

        behaviourStrategy = BehaviourStrategy.Factory.Create(BehaviourStrategy.TYPE.Melee, this);
    
    }


    public override void Start()
    {
        base.Start();
    }


    public override void Update()
    {
        
        base.Update();
    }
    
    public void OnDrawGizmos()
    {
        
        Gizmos.color = Color.blue; // 플레이어 발견
        //Gizmos.DrawLine(transform.position, playerPos.position);
        
        Gizmos.DrawWireSphere(this.transform.position, m_data.defectiveRange);

        Gizmos.color = Color.red; // 공격 할 범위
        Gizmos.DrawWireSphere(this.transform.position, m_data.attackableRange);

    }


    

}
