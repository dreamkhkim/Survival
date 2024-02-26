using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class WeaponManager : MonoBehaviour
{
    public bool isFiring = false;
    public bool isReload = false;
    public int fireRate = 25;
    public int currentBulletCount = 30;
    public int bulletCount = 0;
    public int carryBullet = 120;
    public int maxBulletCount = 30;

    public ParticleSystem muzzleFlash;
    public ParticleSystem hitEffect;
    public TrailRenderer trailBullet;

    [SerializeField]
    private float accumulateTime;

    [SerializeField]
    private StarterAssetsInputs input;

    [SerializeField]
    private ThirdPersonShooterController third;
    [SerializeField]
    private ThirdPersonController isAnim;
    [SerializeField]
    public Transform barrelPos;
    public Transform rayDest;



    public Ray ray;
    public RaycastHit raycastHit;

    private void Awake()
    {
        third = GetComponentInParent<ThirdPersonShooterController>();
        isAnim = GetComponentInParent<ThirdPersonController>();
        input = GetComponentInParent<StarterAssetsInputs>();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }


    public IEnumerator Reload()
    {
        Debug.Log("코루틴 시작 ");

        isAnim._animator.SetTrigger("Reload");
        isAnim.isReload = true;

        yield return new WaitForSeconds(2f);

        while (isReload)
        {
            if(bulletCount <= 0)
            {
                currentBulletCount += maxBulletCount;
                carryBullet -= maxBulletCount;

            }       

            if(carryBullet > maxBulletCount)
            {
                currentBulletCount = bulletCount;
                carryBullet -= currentBulletCount;

                currentBulletCount = maxBulletCount;
                bulletCount = 0;

            }

            else if(bulletCount > carryBullet)
            {
                bulletCount = 0;
                currentBulletCount += carryBullet;
                carryBullet = 0;
            }
            else
            {
                currentBulletCount = bulletCount;
                carryBullet -= currentBulletCount;
                currentBulletCount = maxBulletCount;

                bulletCount = 0;
                
            }
            

            isReload = false;

            yield return null;
        }

        isAnim.isReload = false;
        yield return null;
    }


    public void StartFiring()
    {
        isFiring = true;
        currentBulletCount--;
        bulletCount++;

        if (currentBulletCount <= 0 && carryBullet != 0)
        {
            isReload = true;
            bulletCount = 0;
            StartCoroutine(Reload());
        }
      
        accumulateTime = 0.0f;
        FireBullet();
        
    }



    public void UpdateFiring(float deltaTime)
    {
        accumulateTime += deltaTime;
        float fireInterval = 1.0f / fireRate;

        while(accumulateTime >= 0.0f)
        {
            FireBullet();
            accumulateTime -= fireInterval;
        }
    }

    private void FireBullet()
    {
        muzzleFlash.Emit(1);

        ray.origin = barrelPos.position;
        ray.direction = rayDest.position - barrelPos.position;

        TrailRenderer bullet = Instantiate(trailBullet, ray.origin, Quaternion.identity);
        bullet.AddPosition(ray.origin);
        bullet.transform.position = raycastHit.point;


        hitEffect.transform.position = raycastHit.point;
        hitEffect.transform.forward = raycastHit.normal;
        hitEffect.Emit(1);

    }

    public void StopFiring()
    {
        isFiring = false;
    }

    

  
    //[SerializeField]
    //private float fireRate;
    //[SerializeField]
    //private float fireRateTimer;
    //[SerializeField]
    //private bool semiAuto;

    //[SerializeField]
    //private GameObject bullet;
    //[SerializeField]
    //private Transform barrelPos;
    //[SerializeField]
    //private float bulletVelocity;
    //[SerializeField]
    //private int bulletsPerShot;


    


    //private StarterAssetsInputs starterAssetsInputs;

  

    //// Update is called once per frame
    //void Update()
    //{
    //    if (ShouldFire())
    //        Fire();
    //}


    //private bool ShouldFire()
    //{
    //    fireRateTimer += Time.deltaTime;

    //    if (fireRateTimer < fireRate)
    //        return false;
    //    if (semiAuto && starterAssetsInputs.shoot)
    //        return true;
    //    if (!semiAuto && starterAssetsInputs.shoot)
    //        return true;

    //    return false;

    //}

    //void Fire()
    //{
    //    fireRateTimer = 0;
    //    barrelPos.LookAt(third.aimPos);

    //    for(int i = 0; i < bulletsPerShot; i++)
    //    {
    //        GameObject currentBullet = Instantiate(bullet, barrelPos.position, barrelPos.rotation);
    //        Rigidbody rb = currentBullet.GetComponent<Rigidbody>();
    //        rb.AddForce(barrelPos.forward * bulletVelocity, ForceMode.Impulse);

            
    //    }
    //    Debug.Log("Fire");
    //}
}
