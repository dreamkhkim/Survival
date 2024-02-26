using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    //public enum Weapon
    //{
    //    DEFAULT = 0,
    //    RIFLE
    //};


    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif


    public class ThirdPersonController : MonoBehaviour, IPlayerInteractable
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;
        public float Sensitivity = 1f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;


        private int animDirInputY;
        private int animDirInputX;
        public int animAim;


        //총 스왑 애니메이션들
        public GameObject[] guns;
        [SerializeField]
        public bool gunisSwap = false;
        public bool weaponIsEquip = false;
        public bool isReload = false;
        public bool isShotGun = false;


        //현재 총기 정보 
        [SerializeField]
        public WeaponTypes currentWeapon;

        //라이플 
        [SerializeField]
        public WeaponTypes idleWeapon;
        public Rifle rifle;
        public Pistol pistal;
        public ShotGun shotGun;


        //엄폐 관련된거 
        public RaycastHit coverHitPoint;
        public bool autoMoverActive;
        public Vector3 autoMoverTargetPos;

        public float autoMoverStoppingDistance;

        public bool isSwap = true;
        public bool swapWeapon1 = false;
        public bool swapWeapon2 = false;
        public bool swapWeapon3 = false;

        //[SerializeField]
        //public Weapon myWeapon;


#if ENABLE_INPUT_SYSTEM 
        public PlayerInput _playerInput;
#endif
        public Animator _animator;
        private CharacterController _controller;
        public StarterAssetsInputs _input;
        private GameObject _mainCamera;
        private bool rotateOnMove = true;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
                
#else
				return false;
#endif
            }
        }

        
        public Transform pos
        {
            get => this.GetComponent<Transform>();
           
        }

        [SerializeField]
        private int hp;
        public int PlayerHp
        {
            get => hp;
            set
            {
                hp = value;

                if (hp <= 0)
                    Debug.Log("플레이어 죽음 ");
            }
        }


        public ThirdPersonController Owner
        {
            get => this;
        }

        [SerializeField]
        private bool cover;
        public bool inCover
        {
            get => cover;
            set => cover = value;
        }


        
        public Vector3 inCoverMoveDirection
        {
            get;
            set;
        }

        
        public Vector3 inCoverProhibitedDirection
        {
            get;
            set;
        }

        private void Awake()
        {
            
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }

            

        }

        WeaponTypes WeaponTypes()
        {
            if (swapWeapon1)
                return rifle;
            else if (swapWeapon2)
                return shotGun;
            else if (swapWeapon3)
                return pistal;
            else
                return null;
        }

        private void Start()
        {
            pistal.gameObject.SetActive(false);
            rifle.gameObject.SetActive(false);
            shotGun.gameObject.SetActive(false);
            
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }


        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            Debug.Log(_input.move.y + "xbox 왼쪽 스틱 ");
            GroundedCheck();
            Move();
            SwapWeapon();
            ReloadWeapon();
            GetInput();
            Cover();

            if(isReload == true)
            {
                _input.shoot = false;
                _input.reload = false;
            }

            if (gunisSwap == false)
            {
                _animator.SetLayerWeight(1, 0f);
            }

            if (_input.jump)
                inCover = false;

            if (inCover && !autoMoverActive)
            {
                InCoverMove();

                this.GetComponent<CharacterController>().height = 1f;
                this.GetComponent<CharacterController>().radius = 0.001f;
               
            }
            else
            {
                this.GetComponent<CharacterController>().height = 2f;
                this.GetComponent<CharacterController>().radius = 0.4f;
                
            }

            if (currentWeapon == shotGun)
            {
                isShotGun = true;
            }
            else
                isShotGun = false;



        }


        private bool GetInput()
        {
            swapWeapon1 = _input.swap.x > 0;
            swapWeapon2 = _input.swap.x < 0;
            swapWeapon3 = _input.swap.y < 0;

            if (isSwap)
            {
                if (swapWeapon1)
                {
                    return true;

                }
                else if (swapWeapon2)
                    return true;
                else if (swapWeapon3)
                    return true;

            }
            return false;
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");

            animDirInputY = Animator.StringToHash("YInput");
            animDirInputX = Animator.StringToHash("XInput");
            animAim = Animator.StringToHash("Aim");



        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);


            #region 
            // update animator if using character
            //if (_hasAnimator)
            //{
            //    _animator.SetBool(_animIDGrounded, Grounded);
            //}
            #endregion
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier * Sensitivity;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier * Sensitivity;


            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);


        }

        private void Move()
        {

            if (cover != true)
            {


                // set target speed based on move speed, sprint speed and if sprint is pressed
                float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

                // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

                // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
                // if there is no input, set the target speed to 0
                if (_input.move == Vector2.zero)
                {
                    //_animator.SetFloat("Speed", 0);
                    _animator.SetFloat(animDirInputY, 0);
                    _animator.SetFloat(animDirInputX, 0);

                    targetSpeed = 0.0f;
                }
              
                // a reference to the players current horizontal velocity
                float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

                float speedOffset = 0.1f;
                float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

                // accelerate or decelerate to target speed
                if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                    currentHorizontalSpeed > targetSpeed + speedOffset)
                {

                    // creates curved result rather than a linear one giving a more organic speed change
                    // note T in Lerp is clamped, so we don't need to clamp our speed
                    _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                        Time.deltaTime * SpeedChangeRate);

                    // round speed to 3 decimal places
                    _speed = Mathf.Round(_speed * 1000f) / 1000f;
                }
                else
                {
                    _speed = targetSpeed;
                }

                #region InputAniDir
                if (_input.move.y < 0)
                {
                    _animator.SetFloat(animDirInputX, 0);
                    _animator.SetFloat(animDirInputY, -1);


                    if (_input.move.y < 0 && _input.move.x < 0)
                    {
                        _animator.SetFloat(animDirInputX, -1);
                        _animator.SetFloat(animDirInputY, -1);

                    }

                    else if (_input.move.y < 0 && _input.move.x > 0)
                    {
                        _animator.SetFloat(animDirInputX, 1);
                        _animator.SetFloat(animDirInputY, -1);
                    }

                }
                else if (_input.move.y > 0)
                {
                    _animator.SetFloat(animDirInputX, 0);
                    _animator.SetFloat(animDirInputY, 1);



                    if (_input.move.y > 0 && _input.move.x < 0)
                    {
                        _animator.SetFloat(animDirInputX, -1);
                        _animator.SetFloat(animDirInputY, 1);
                    }


                    else if (_input.sprint != false)
                    {
                        _input.jump = false;
                        _input.cover = false;
                        _animator.SetFloat(animDirInputY, 2);
                        _animator.SetFloat(animDirInputX, 0);

                    }

                    else if (_input.move.y > 0 && _input.move.x > 0)
                    {
                        _animator.SetFloat(animDirInputX, 1);
                        _animator.SetFloat(animDirInputY, 1);
                    }
                }

                if (_input.move.x < 0)
                {
                    _animator.SetFloat(animDirInputX, -1);
                    _animator.SetBool("LeftRight", true);

                    if (_input.sprint != false)
                    {
                        _input.cover = false;
                        _input.jump = false;
                        _animator.SetFloat(animDirInputY, 2);
                        _animator.SetFloat(animDirInputX, 0);

                    }
                }

                else if (_input.move.x > 0)
                {
                    _animator.SetFloat(animDirInputX, 1);
                    _animator.SetBool("LeftRight", false);
                    if (_input.sprint != false)
                    {
                        _input.jump = false;
                        _animator.SetFloat(animDirInputY, 2);
                        _animator.SetFloat(animDirInputX, 0);

                    }
                }
                #endregion

                _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
                if (_animationBlend < 0.01f) _animationBlend = 0f;

                // normalise input direction
                Vector3 inputDirection = new Vector3(_input.move.x, 0, _input.move.y).normalized;

                // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
                // if there is a move input rotate player when the player is moving
                if (_input.move != Vector2.zero)
                {
                    _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                       _mainCamera.transform.eulerAngles.y;

                    #region Rotate Them
                    //float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    //    RotationSmoothTime);

                    //else if (transform.rotation.y < 90)
                    //    return;


                    // rotate to face input direction relative to camera positio
                    //transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                    //if (rotateOnMove)
                    //{
                    //    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                    //}
                    #endregion
                }


                Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

                // move the player
                _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                                 new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);




                // update animator if using character
                //if (_hasAnimator)
                //{
                //    //_animator.SetFloat(_animIDSpeed, _animationBlend);
                //    //_animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
                //}

                //JumpRoll
                if (_input.move.y > 0 && _input.jump)
                {
                    Debug.Log("구르기 누름 ");
                    _input.jump = false;
                    cover = false;


                    _controller.Move(Vector3.forward.normalized + targetDirection.normalized * 1f);

                    _animator.SetTrigger("Roll");
                }
                else if (_input.move.y < 0 && _input.jump)
                {
                    _input.jump = false;
                    cover = false;
                    _controller.Move(Vector3.back.normalized + targetDirection.normalized * 1f);
                    _animator.SetTrigger("Roll");

                }
                else if (_input.move.x > 0 && _input.jump)
                {
                    _input.jump = false;
                    cover = false;
                    _controller.Move(Vector3.right.normalized + targetDirection.normalized * 1f);
                    _animator.SetTrigger("Roll");

                }
                else if (_input.move.x < 0 && _input.jump)
                {
                    _input.jump = false;
                    cover = false;
                    _controller.Move(Vector3.left.normalized + targetDirection.normalized * 1f);
                    _animator.SetTrigger("Roll");

                }
            }

        }


        IEnumerator JumpRoll()
        {
            float curTime = 0f;
            curTime += Time.deltaTime;
            _animator.SetTrigger("Roll");
         
            yield return null;
            
            
        }


        #region JumpAndGravity
        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }
        #endregion


        private void Cover()
        {
            if (inCover && autoMoverActive)
            {
                MoveToCover();
            }
        }

        private void MoveToCover()
        {
            Vector3 moveDir = (autoMoverTargetPos - transform.position).normalized;

            if(Vector3.Distance(transform.position, autoMoverTargetPos) > autoMoverStoppingDistance)
            {
                _controller.Move(moveDir * SprintSpeed * Time.deltaTime);
                
            }
            else
            {
                autoMoverActive = false;
                autoMoverTargetPos = Vector3.zero;

            }

        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
        
        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        public void SetSensitivity(float newSensitivity)
        {
            Sensitivity = newSensitivity;
        }

        public void SetRotateOnMove(bool newRotateOnMove)
        {
            rotateOnMove = newRotateOnMove;
        }

        private void SwapWeapon()
        {
            if (GetInput() &&  WeaponTypes() != currentWeapon)
            {
                weaponIsEquip = true;
                gunisSwap = true;
                PutWeaponOn();
                
               
            }
        }

        private void PutWeaponOn()
        {
            if(currentWeapon != null)
            {
                if (currentWeapon == idleWeapon)
                    currentWeapon.GunSetOffAnim();
                else if (currentWeapon == rifle)
                    currentWeapon.GunSetOffAnim();

                currentWeapon.gameObject.SetActive(false);
            }

            if (currentWeapon == pistal)
                idleWeapon.gameObject.SetActive(true); // 캐릭터 뒤에 총기 부착하는 idle
            else if (currentWeapon == shotGun)
                idleWeapon.gameObject.SetActive(true);

            currentWeapon = WeaponTypes();
            currentWeapon.GunSetOnAnim();

           
            currentWeapon.gameObject.SetActive(true);
        }

        private void ReloadWeapon()
        {
            if (currentWeapon.weaponData.currentBulletCount == currentWeapon.weaponData.maxBulletCount)
            {
                _input.reload = false;
            }

            if (currentWeapon.weaponData.currentBulletCount == 0 && currentWeapon.weaponData.carryBullet == 0)
            {
                currentWeapon.weaponData.bulletCount = 0;
                _input.shoot = false;
                _input.reload = false;

                 currentWeapon.weaponData.isReload = false;

            }

            if (_input.reload && gunisSwap == true && isReload != true && currentWeapon.weaponData.currentBulletCount <= currentWeapon.weaponData.maxBulletCount
                && currentWeapon.weaponData.carryBullet > 0)
            {
                Debug.Log(currentWeapon + "너 정체가 뭐야 + 장전하는놈 누구  ");
                _input.reload = false;
                isReload = true;
                currentWeapon.weaponData.isReload = true;

                
                StartCoroutine(currentWeapon.Reload());

            }


        }

        private void InCoverMove()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            if (_input.move.x == 0)
            {
                //_animator.SetFloat("Speed", 0);
                _animator.SetFloat(animDirInputY, 0);
                _animator.SetFloat(animDirInputX, 0);

                targetSpeed = 0.0f;
            }

          
            Vector3 perpDirection = Vector3.Cross(inCoverMoveDirection, Vector3.up);

            

            transform.TransformDirection(perpDirection);

            Vector3 moveDirection = inCoverMoveDirection.normalized * _input.move.x;



            // move the player
            if (moveDirection != inCoverProhibitedDirection.normalized)
            {
                _input.coverRestrict = false;
                _controller.Move(moveDirection * (targetSpeed * Time.deltaTime)
                    + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            }
            else
            {
                
                targetSpeed = 0;
            }


            if (_input.move.x < 0)
            {
                _animator.SetFloat(animDirInputX, -1);
                _animator.SetBool("LeftRight", true);

                if (_input.sprint != false)
                {
                    _input.cover = false;
                    _input.jump = false;
                    _animator.SetFloat(animDirInputY, 2);
                    _animator.SetFloat(animDirInputX, 0);

                }
            }

            else if (_input.move.x > 0)
            {
                _animator.SetFloat(animDirInputX, 1);
                _animator.SetBool("LeftRight", false);
                if (_input.sprint != false)
                {
                    _input.jump = false;
                    _animator.SetFloat(animDirInputY, 2);
                    _animator.SetFloat(animDirInputX, 0);

                }
            }


            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, 0).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {

                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

          

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            



        }


        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }



        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }

        private void OnYes(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0f)
            {
                guns[0].SetActive(false);
                //guns[1].SetActive(true);

            }
        }

        private void OnNo(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0f)
            {
                guns[0].SetActive(true);
                //guns[1].SetActive(false);
            }
        }

        private void Reload(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0f)
            {
                guns[2].SetActive(true);
                guns[3].SetActive(false);
            }
            
        }


        private void ReloadExit(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                guns[2].SetActive(false);
                guns[3].SetActive(true);
            }
        }

        public void Interaction(int value)
        {
            hp -= value;

            if (hp <= 0)
            {
                Time.timeScale = 0;
                GameManager.instance.PlayerDeadScene();

            }
        }

        public void BeginMoveToCover(Vector3 targetPos)
        {
            cover = true;
            autoMoverActive = true;
            autoMoverTargetPos = targetPos;

        }



        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IInteractObject>(out IInteractObject player))
            {
                Debug.Log("플레이어 문 상호작용 성공했다 ");
                player.ObjectInteraction();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            
            if(other.TryGetComponent<IInteractObject>(out IInteractObject player))
            {
                player.ObjectInteraction();
            }
        }

      
    }

}