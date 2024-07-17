using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.AI;


[System.Serializable]
public class MonsterData
{
    public int hp;
    public int defectiveRange;
    public int attackableRange;

    public int animDead;
    public int Attack;
}


public abstract class BehaviourStrategy 
{
    public enum TYPE
    {
        Melee, Ranged
    }

    protected Monster monster;
  
    protected BehaviourStrategy(Monster monster)
    {
        this.monster = monster;
    }

    public static class Factory
    {
        public static BehaviourStrategy Create(TYPE type, Monster monster)
        {
            switch (type)
            {
                case TYPE.Melee:
                    return new MeleeBehaviourStrategy(monster);
                case TYPE.Ranged:
                    return new RangedBehaviourStrategy(monster);
                default:
                    return null;
            }
        }
    }

    public abstract void Behaviour();

}


public class MeleeBehaviourStrategy : BehaviourStrategy
{
    public MeleeBehaviourStrategy(Monster monster) : base(monster)
    {
        this.monster = monster;
        
    }

    public override void Behaviour()
    {
        //Debug.Log("근거리 공격타입");
        monster.Anim.SetBool("MeleeAttack", true);


        MeleeAttack();

    }

    
    public void MeleeAttack()
    { 
        monster.nav.SetDestination(monster.player.pos.position);
        monster.Anim.SetBool("MeleeRun", true);


        if (Vector3.Distance(monster.transform.position, monster.player.pos.position) <= monster.m_data.attackableRange)
        {
           // Debug.Log("공격 ");

            
            if (monster.currentAttackCoolDoon <= 0)
            {
                SoundManager.instance.PlayAudio(SoundManager.instance.meleeMonster, false, this.monster.transform.position, 0.5f);
                monster.player.Interaction(monster.m_data.Attack);
                monster.Anim.SetBool("MeleeAttack", true);

                monster.ResetAttackCoolDown();
            }
        }
        
        else
        {
            monster.Anim.SetBool("MeleeAttack", false);
        }
    }
}

public class RangedBehaviourStrategy : BehaviourStrategy
{

    public ManKind manMonster;
    public bool isFire;
    
    public bool isFireCheck = true;
    public float fireNextTime = 0f;

    public float fireInterval = 3f;


    public RangedBehaviourStrategy(Monster monster) : base(monster)
    {
        manMonster = (ManKind)this.monster;
        
    }

    public override void Behaviour()
    {

        if (manMonster.m_data.hp <= 0)
            isFireCheck = false;

        if (Vector3.Distance(monster.transform.position, monster.player.pos.position) <= monster.m_data.defectiveRange)
        {
            
            RangedAttack();
        }

        Debug.Log("원거리 공격타입");
    }

    public void RangedAttack()
    {
        
        if(isFireCheck)
            monster.attackTimeCheck += Time.deltaTime;

        Vector3 direction = GetDirection();
        monster.transform.LookAt(monster.player.pos.position);

       
        isFire = Time.time >= fireNextTime;

        

        if (monster.attackTimeCheck >= monster.bulletIntervalcheck)
        {
            isFireCheck = false;
            monster.Anim.SetBool("Aiming", false);
            monster.StartCoroutine(EnemyCover());

        }


        if (isFireCheck != false)
        {
            
            if (isFire && monster.attackTimeCheck <= monster.bulletIntervalcheck &&
                Physics.Raycast(manMonster.transform.position, direction, out RaycastHit hit, 10f, 1 << 3))
            {

                SoundManager.instance.PlayAudio(SoundManager.instance.rifleSound, false, this.monster.transform.position, 0.5f);
                fireNextTime = Time.time + 1f / fireInterval;
                monster.Anim.SetBool("Aiming", true);
                manMonster.muzzleFlash.Emit(1);


                Debug.DrawRay(hit.transform.position, hit.transform.forward, Color.blue);
                hit.point = direction;

                manMonster.BulletRay(hit);
                

                if (hit.point != monster.player.pos.position)
                {
                    monster.player.Interaction(monster.m_data.Attack);
                    Debug.Log("맞았다 ");
                }

            }
        }
        else
        {
            manMonster.muzzleFlash.Emit(0);
        }
       
    }

    IEnumerator EnemyCover()
    {
        monster.Anim.SetTrigger("Cover");
        monster.attackTimeCheck = 0;
        isFireCheck = false;

        
        yield return new WaitForSeconds(3f);

        isFireCheck = true;
        yield return null;
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = monster.transform.forward;

        direction += new Vector3(Random.Range(-monster.bulletSpread.x, monster.bulletSpread.x),
            Random.Range(-monster.bulletSpread.y, monster.bulletSpread.y),
            Random.Range(-monster.bulletSpread.z, monster.bulletSpread.z));

        return direction;

    }

   

    

}




public abstract class Monster : MonoBehaviour, IMonsterInteractable
{
   
    public IPlayerInteractable player;
    public NavMeshAgent nav;


    public EnemyPool enemyPool;

    public int random;

    public float currentAttackCoolDoon = 0f;
    private float attackCooldown = 1f; // 공격 쿨다운 시간 (초)

    public float attackTimeCheck = 0f;
    public float bulletIntervalcheck;

    public float currentRangedAttackCoolDoon = 0f;

    private float rangedAttackCooldown = 3f; // 공격 쿨다운 시간 (초)

    public Vector3 bulletSpread = new Vector3(0.1f, 0.1f, 0.1f);


    public Transform monsterPos;

    [SerializeField]
    private Animator anim;
    public Animator Anim
    {
        get => anim;
    }


  
    public BehaviourStrategy behaviourStrategy;
    public MonsterData m_data;

    public virtual void Start()
    {
        monsterPos = GetComponent<Transform>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        enemyPool = GetComponent<EnemyPool>();

        bulletIntervalcheck = Random.Range(7, 13);
        
        m_data = new MonsterData();

        Init();
        behaviourStrategy.Behaviour();
    }

    public virtual void Update()
    {
        
        if (currentAttackCoolDoon > 0)
            currentAttackCoolDoon -= Time.deltaTime;

        behaviourStrategy.Behaviour();
    }

    public void ResetAttackCoolDown()
    {
        currentAttackCoolDoon = attackCooldown;
    }

    public void ResetRangedAttackCoolDown()
    {
        currentAttackCoolDoon = rangedAttackCooldown;
    }

    

    public abstract void Init();



    public void Interaction(int value)
    {
        Debug.Log("몬스터랑 상호작용 성공 ");
        m_data.hp -= value;

        if (m_data.hp <= 0)
        {
            random = Random.Range(0, 5);
            this.nav.isStopped = true;
            this.nav.velocity = Vector3.zero;
            gameObject.GetComponent<CapsuleCollider>().enabled = false;

            if (random == 0)
            {
                Debug.Log("라이플 탄약 ");
                GameObject rifle = EnemyObjectPoolManager.instance.GetPrafab("RifleAmmo");
                rifle.transform.position = monsterPos.position;
                rifle.transform.rotation = this.transform.rotation;

            }
           
            else if (random == 1)
            {

                Debug.Log("피스톨 탄약 ");
                GameObject pistol = EnemyObjectPoolManager.instance.GetPrafab("PistolAmmo");
                pistol.transform.position = monsterPos.position;
                pistol.transform.rotation = this.transform.rotation;

            }
            else if (random == 2)
            {
                Debug.Log("피스톨 탄약 ");
                GameObject shotGun = EnemyObjectPoolManager.instance.GetPrafab("ShotGunAmmo");
                shotGun.transform.position = monsterPos.position;
                shotGun.transform.rotation = this.transform.rotation;

            }
            else if (random == 3)
            {
                Debug.Log("플레이어 체력");

                GameObject health = EnemyObjectPoolManager.instance.GetPrafab("Health");
                health.transform.position = monsterPos.position;
                health.transform.rotation = this.transform.rotation;
            }

            else
            {
                StartCoroutine(ObjectReturnPool());
            }
                
                
            Anim.SetTrigger(m_data.animDead);

            StartCoroutine(ObjectReturnPool());
            
            Init();
        }
    }

    protected IEnumerator ObjectReturnPool()
    {
        float curTime = 0f;

        curTime += Time.deltaTime;
        

        while(curTime < 2f)
        {
            yield return new WaitForSeconds(2f);
            
            gameObject.GetComponent<CapsuleCollider>().enabled = true;
            enemyPool.ReleaseObject();
           
            yield return null;
        }

        yield return null;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IInteractObject>(out IInteractObject monster))
        {
            Debug.Log("플레이어 문 상호작용 성공했다 ");
            monster.ObjectInteraction();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<IInteractObject>(out IInteractObject monster))
        {
            monster.ObjectInteraction();
        }
    }
}

