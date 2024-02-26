using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Rifle : WeaponTypes
{

    public ParticleSystem muzzleFlash;
    public ParticleSystem hitEffect;
    public TrailRenderer trailBullet;

    [SerializeField]
    private float accumulateTime;

    //public bool isFiring = false;
    //public bool isReload = false;
    //public int fireRate = 25;
    //public int currentBulletCount = 30;
    //public int bulletCount = 0;
    //public int carryBullet = 120;
    //public int maxBulletCount = 30;

    [SerializeField]
    public Transform barrelPos = null;
    public Transform rayDest = null;



    public override void GunSetOffAnim()
    {
        SoundManager.instance.PlayAudio(SoundManager.instance.handlingLifle, false, 1);
        weaponData.isAnim._animator.SetTrigger("SwapCant");
    }
    public override void GunSetOnAnim()
    {
        SoundManager.instance.PlayAudio(SoundManager.instance.handlingLifle, false, 1);
        weaponData.isAnim._animator.SetTrigger("Swap");
    }

   

    public override void InitSetting()
    {
        weaponData.isFiring = false;
        weaponData.isReload = false;

        weaponData.fireRate = 25;
        weaponData.currentBulletCount = 30;
        weaponData.bulletCount = 0;
        weaponData.carryBullet = 120;
        weaponData.maxBulletCount = 30;

        weaponData.weaponDamage = 9;

        weaponData.muzzleFlash = this.muzzleFlash;
        weaponData.hitEffect = this.hitEffect;
        weaponData.trailBullet = this.trailBullet;

        weaponData.fireInterval = 6f;


        weaponData.accumulateTime = 0;

        weaponData.barrelPos = this.barrelPos;
        weaponData.rayDest = this.rayDest;
        

    }

    public override void Awake()
    {
        base.Awake();
    }

  
    private void Update()
    {
        AimAnim();
        if(weaponData.isAnim.isReload == true)
        {
            weaponData.isAnim._animator.SetLayerWeight(1, 0f);
            

        }
    }

    public override IEnumerator Reload()
    {
        Debug.Log("코루틴 시작 ");

        SoundManager.instance.PlayAudio(SoundManager.instance.rifleReload, false, 1);
        weaponData.isAnim.isReload = true;
        weaponData.isAnim._animator.SetTrigger("Reload");
        
        yield return new WaitForSeconds(2f);
        
        while (weaponData.isReload)
        {
            
            if (weaponData.bulletCount <= 0)
            {
                weaponData.currentBulletCount += weaponData.maxBulletCount;
                weaponData.carryBullet -= weaponData.maxBulletCount;

            }

            if (weaponData.carryBullet > weaponData.maxBulletCount)
            {
                weaponData.currentBulletCount = weaponData.bulletCount;
                weaponData.carryBullet -= weaponData.currentBulletCount;

                weaponData.currentBulletCount = weaponData.maxBulletCount;
                weaponData.bulletCount = 0;

            }

            else if (weaponData.bulletCount > weaponData.carryBullet)
            {
                weaponData.bulletCount = 0;
                weaponData.currentBulletCount += weaponData.carryBullet;
                weaponData.carryBullet = 0;
            }
            else
            {
                weaponData.currentBulletCount = weaponData.bulletCount;
                weaponData.carryBullet -= weaponData.currentBulletCount;
                weaponData.currentBulletCount = weaponData.maxBulletCount;

                weaponData.bulletCount = 0;

            }

            weaponData.isAnim._animator.SetLayerWeight(1, 0f);
            weaponData.isReload = false;

            if(weaponData.isReload == false)
            {
                weaponData.isAnim._animator.SetLayerWeight(1, 1f);
            }

            yield return null;
        }
        
        weaponData.isAnim.isReload = false;

        
        yield return null;
    }

    public override void StartFiring()
    {
        weaponData.isFiring = true;
        weaponData.currentBulletCount--;
        weaponData.bulletCount++;

        if (weaponData.currentBulletCount <= 0 && weaponData.carryBullet != 0)
        {
            weaponData.isReload = true;
            weaponData.bulletCount = 0;
            StartCoroutine(Reload());
        }

        weaponData.accumulateTime = 0.0f;
        FireBullet();
    }

    public override void UpdateFiring(float deltaTime)
    {
        weaponData.accumulateTime += deltaTime;
        float fireInterval = 1.0f / weaponData.fireRate;

        while (weaponData.accumulateTime >= 0.0f)
        {
            FireBullet();
            weaponData.accumulateTime -= fireInterval;
        }
    }

    public override void FireBullet()
    {
        weaponData.muzzleFlash.Emit(1);
        SoundManager.instance.PlayAudio(SoundManager.instance.rifleSound, false, 1);
        weaponData.ray.origin = weaponData.barrelPos.position;
        weaponData.ray.direction = weaponData.rayDest.position - weaponData.barrelPos.position;

        TrailRenderer bullet = Instantiate(weaponData.trailBullet, weaponData.ray.origin, Quaternion.identity);
        bullet.AddPosition(weaponData.ray.origin);
        bullet.transform.position = weaponData.raycastHit.point;


        weaponData.hitEffect.transform.position = weaponData.raycastHit.point;
        weaponData.hitEffect.transform.forward = weaponData.raycastHit.normal;
        weaponData.hitEffect.Emit(1);
    }

    public override void StopFiring()
    {
        weaponData.isFiring = false;
    }

    

    public override void AimAnim()
    {

        if (weaponData.isAnim._input.aim && weaponData.isAnim.gunisSwap == true)
        {
            weaponData.isAnim._animator.SetBool(weaponData.isAnim.animAim, true);
            weaponData.isAnim._animator.SetLayerWeight(1, 0f);
            weaponData.isAnim._animator.SetLayerWeight(2, 0f);
        }

        else
        {
            weaponData.isAnim._animator.SetBool(weaponData.isAnim.animAim, false);
            weaponData.isAnim._animator.SetLayerWeight(1, 1f);
            weaponData.isAnim._animator.SetLayerWeight(2, 0f);
        }
    }
}
