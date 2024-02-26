using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;


[System.Serializable]
public class WeaponData
{
    
    public bool isFiring;
    public bool isReload;
    public int fireRate;
    public float fireInterval;
    [SerializeField]
    public int currentBulletCount;
    public int bulletCount;
    public int carryBullet;
    public int maxBulletCount;
    public float accumulateTime;

    public int weaponDamage;

    public ParticleSystem muzzleFlash;
    public ParticleSystem hitEffect;
    public TrailRenderer trailBullet;

    public Transform barrelPos;
    public Transform rayDest;

    public Ray ray;
    public RaycastHit raycastHit;

    public ThirdPersonController isAnim;


}


public abstract class WeaponTypes : MonoBehaviour
{
    public WeaponData weaponData;

    public virtual void Awake()
    {
        weaponData = new WeaponData();
        weaponData.isAnim = GetComponentInParent<ThirdPersonController>();
        InitSetting();
    }

    public void Start()
    {
        
        //InitSetting();
        //GunSetOnAnim();
    }

    public abstract void InitSetting();
    public abstract void GunSetOnAnim();
    public abstract void GunSetOffAnim();
    public abstract void AimAnim();
    public abstract IEnumerator Reload();
   

    public virtual void StartFiring()
    {
        
    }

    public virtual void UpdateFiring(float deltaTime)
    {
       
    }

    public virtual void FireBullet()
    {
        
    }

    public virtual void StopFiring()
    {
    }

    public virtual void AimRay()
    {

    } 

}
