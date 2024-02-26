using Cinemachine;
using StarterAssets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;




public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField]
    private float normalSensitivity;
    [SerializeField]
    private float aimSensitivity;
    [SerializeField]
    private LayerMask aimColliderMask = new LayerMask();

    [SerializeField]
    private Transform highCoverDetectionPoint;

    [SerializeField]
    private Transform leftCoverDetectionPoint;

    [SerializeField]
    private Transform rightCoverDetectionPoint;
    [SerializeField]
    private bool inHighCover;

    public float max;
    [SerializeField]
    private bool inCover;

    

    Vector3 coverHitPoint;

    public  Vector3 coverSurfaceDirection;


    public ParticleSystem hitEffect;

    public Transform aimPos; // 레이를 향한 총기 방향 
    public float minRayDistance;

   
    //애니메이션 리깅 관련 
    [SerializeField]
    private Rig aimRig; // 애니메이션 리깅 
    private float aimRigWeight;

    public bool isFire;
    public float fireNextTime = 0f;
 

    [SerializeField]
    public ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;



    private void Awake()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<ThirdPersonController>();

        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward.normalized * max, Color.red);

        Debug.DrawRay(highCoverDetectionPoint.position, highCoverDetectionPoint.forward.normalized * max, Color.red);

        Debug.DrawRay(leftCoverDetectionPoint.position, leftCoverDetectionPoint.forward.normalized * max, Color.red);

        Debug.DrawRay(rightCoverDetectionPoint.position, rightCoverDetectionPoint.forward.normalized * max, Color.red);

       

        if (inCover)
        {
            thirdPersonController._animator.SetLayerWeight(3, 1f);
            SetCoverType();
            
            InCoverMovementRestrictor();

        }

        else
            thirdPersonController._animator.SetLayerWeight(3, 0f);

        if (starterAssetsInputs.jump)
        {
            inCover = false;
        }    

        AimRay();

        if (starterAssetsInputs.cover)
        {

            if (IsNearCover())
            {
                
                starterAssetsInputs.cover = false;
                Debug.Log("엄폐하기 ");
                MoveCharacterToCover();
               
            }

            
        }

       
        //CoverTypeDetectorRay();
        aimRig.weight = Mathf.Lerp(aimRig.weight, aimRigWeight, Time.deltaTime * 100f);
    }

    public void AimRay()
    {
        Vector3 mouseWorldPos = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);

        RaycastHit raycastHit;

        // 레이캐스트의 시작 지점에서의 최소 거리 설정
        // 최소 거리를 0.5m로 설정 (필요에 따라 조절 가능)

        Vector3 rayStartPosition = Camera.main.transform.position + Camera.main.transform.forward * minRayDistance;
        Ray ray = new Ray(rayStartPosition, Camera.main.transform.forward);

        

        if (Physics.Raycast(ray, out raycastHit, 999f, aimColliderMask))
        {
            mouseWorldPos = raycastHit.point;

            Vector3 worldAimTarget = mouseWorldPos;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f); 
            aimPos.position = Vector3.Lerp(aimPos.position, raycastHit.point, Time.deltaTime * 20f); // 총기 벽에 다가올 때 오브젝트 회전 방지하기 위한 /

           // Debug.DrawLine(weapon.ray.origin, raycastHit.point, Color.red, 2f);
           // Debug.Log(raycastHit.collider.name + "맞은 정보 ");
        }
        

       
        if (inHighCover)
            aimVirtualCamera.gameObject.SetActive(false);


        if(starterAssetsInputs.aim && thirdPersonController.gunisSwap == true && thirdPersonController.isShotGun)
        {
            this.GetComponent<CharacterController>().height = 2f;
            this.GetComponent<CharacterController>().radius = 0.4f;


            thirdPersonController._animator.SetLayerWeight(3, 0.5f);
            aimVirtualCamera.gameObject.SetActive(true);

            thirdPersonController.isSwap = false;
            thirdPersonController.SetSensitivity(aimSensitivity);
            aimRigWeight = 1f;
            
            isFire = Time.time >= fireNextTime;

            if(starterAssetsInputs.shoot && isFire)
            {

                for(int i = 0; i < 3; i++)
                {

                    if (Physics.Raycast(thirdPersonController.shotGun.barrelPos.position, thirdPersonController.shotGun.GetShootingDirection(), out raycastHit, 50f, aimColliderMask))
                    {
                        thirdPersonController.currentWeapon.weaponData.raycastHit = raycastHit;

                        thirdPersonController.currentWeapon.StartFiring();

                        if (thirdPersonController.currentWeapon.weaponData.isFiring)
                        {
                            thirdPersonController.currentWeapon.UpdateFiring(Time.deltaTime);
                        }

                        fireNextTime = Time.time + 0.1f / thirdPersonController.currentWeapon.weaponData.fireInterval;


                        if (raycastHit.transform.TryGetComponent<IMonsterInteractable>(out IMonsterInteractable monster))
                        {
                            hitEffect.transform.position = raycastHit.point;
                            hitEffect.transform.forward = raycastHit.normal;


                            hitEffect.Emit(1);
                            monster.Interaction(thirdPersonController.currentWeapon.weaponData.weaponDamage);
                        }
                    }  

                }
            }
        }


        else if (starterAssetsInputs.aim && thirdPersonController.gunisSwap == true)
        {
            this.GetComponent<CharacterController>().height = 2f;
            this.GetComponent<CharacterController>().radius = 0.4f;


            thirdPersonController._animator.SetLayerWeight(3, 0.5f);
            aimVirtualCamera.gameObject.SetActive(true);

            thirdPersonController.isSwap = false;
            thirdPersonController.SetSensitivity(aimSensitivity);
            aimRigWeight = 1f;

            isFire = Time.time >= fireNextTime;

            if (starterAssetsInputs.shoot && isFire)
            {
                
                thirdPersonController.currentWeapon.weaponData.raycastHit = raycastHit;
                thirdPersonController.currentWeapon.StartFiring();
               
                if (thirdPersonController.currentWeapon.weaponData.isFiring)
                {
                    thirdPersonController.currentWeapon.UpdateFiring(Time.deltaTime);
                }
                fireNextTime = Time.time + 1f / thirdPersonController.currentWeapon.weaponData.fireInterval;


                if (raycastHit.transform.TryGetComponent<IMonsterInteractable>(out IMonsterInteractable monster))
                {
                    hitEffect.transform.position = raycastHit.point;
                    hitEffect.transform.forward = raycastHit.normal;

                   
                    hitEffect.Emit(1);
                    monster.Interaction(thirdPersonController.currentWeapon.weaponData.weaponDamage);
                }
            }

           
            else
                thirdPersonController.currentWeapon.StopFiring();
               

        }
      
           
        else
        {
            
            thirdPersonController.isSwap = true;
            thirdPersonController.currentWeapon.StopFiring();
            aimRigWeight = 0f;
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSensitivity);
        }

    }

    private void SetCoverType()
    {
        if (Physics.Raycast(highCoverDetectionPoint.transform.position, highCoverDetectionPoint.transform.forward, max, aimColliderMask << 6))
        {

            inHighCover = true;
            starterAssetsInputs.aim = false;
            
            Debug.Log("벽 커버가 높은 위치입니다.");

        }
        else
        {
            Debug.Log("벽 커버가 낮은 위치입니다.");
            inHighCover = false;
        }

    }

    private bool IsNearCover()
    {
        RaycastHit raycastHit;
        Debug.DrawLine(transform.position, transform.forward * 10f,Color.red);

        
        if (Physics.Raycast(transform.position, transform.forward, out raycastHit, 2f, aimColliderMask << 6))
        {
            coverHitPoint = raycastHit.point;

            coverSurfaceDirection = GetCoverSurfaceDirection(raycastHit.normal);
          
            return true;
        }
        else
            return false;

    }



    private void InCoverMovementRestrictor()
    {
        bool didLeftCoverDectorHit = Physics.Raycast(leftCoverDetectionPoint.transform.position, leftCoverDetectionPoint.transform.forward, max, aimColliderMask << 6);
        bool didRightCoverDectorHit = Physics.Raycast(rightCoverDetectionPoint.transform.position, rightCoverDetectionPoint.transform.forward, max, aimColliderMask << 6);

        if (!didLeftCoverDectorHit || !didRightCoverDectorHit)
        {
            if(!didLeftCoverDectorHit)
            {    
                Debug.Log("왼쪽 제한 ");
                Debug.Log("왼쪽 이동하는 값 " + coverSurfaceDirection);
                SetCharacterMoverCoverDirections(coverSurfaceDirection, -coverSurfaceDirection);
            }
            else
            {
                Debug.Log("오른 제한 ");
                Debug.Log("오쪽 이동하는 값 " + coverSurfaceDirection);
                SetCharacterMoverCoverDirections(coverSurfaceDirection, coverSurfaceDirection);
            }

        }
        else
        {
            SetCharacterMoverCoverDirections(coverSurfaceDirection, Vector3.zero);
        }
    }

   
    private Vector3 GetCoverSurfaceDirection(Vector3 hitNormal)
    {
        return Vector3.Cross(hitNormal, Vector3.up).normalized;
    }

    private void SetCharacterMoverCoverDirections(Vector3 moveDir, Vector3 dirToProhibit)
    {
        thirdPersonController.inCoverMoveDirection = moveDir;
        thirdPersonController.inCoverProhibitedDirection = dirToProhibit;
    }




    private void MoveCharacterToCover()
    {
        inCover = true; 
        thirdPersonController.BeginMoveToCover(coverHitPoint);
        
    }


    
}
