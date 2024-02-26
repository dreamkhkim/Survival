using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManKind : Monster
{
    public Ray ray;
    public RaycastHit raycastHit;


    public Transform barrelPos;
    public TrailRenderer trailBullet;

    public ParticleSystem muzzleFlash;

    public override void Init()
    {
        m_data.hp = 20;
        m_data.Attack = 2;
        m_data.attackableRange = 2;
        m_data.defectiveRange = 13;


        m_data.animDead = Animator.StringToHash("MonsterDead");

        behaviourStrategy = BehaviourStrategy.Factory.Create(BehaviourStrategy.TYPE.Ranged, this);
    }

    public override void Start()
    {
        base.Start();
        

    }

    public override void Update()
    {
        base.Update();
    }


    public void BulletRay(RaycastHit hit)
    {
        raycastHit = hit;

        trailBullet.transform.position = raycastHit.point;
        ray.origin = barrelPos.position;
        


        TrailRenderer bullet = Instantiate(trailBullet, ray.origin, Quaternion.identity);
        bullet.AddPosition(ray.origin);
        bullet.transform.position = raycastHit.point;

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
