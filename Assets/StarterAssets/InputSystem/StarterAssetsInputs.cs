using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    public class StarterAssetsInputs : MonoBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;
        

        public bool dir;
        public bool jump;
        public bool sprint;
        public bool aim;
        public bool shoot;
        public Vector2 swap;
        public float swapNumber;
        public bool reload;
        public bool cover;
        public bool coverRestrict;

        [Header("Movement Settings")]
        public bool analogMovement;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());

            if(coverRestrict != false)
            {
                move.x = 0f;
            }
            //if(coverRestrict != false)
            //{
            //    move.y = 0f;
            //}
        }

        public void OnLook(InputValue value)
        {
            if (cursorInputForLook)
            {
                LookInput(value.Get<Vector2>());
            }
        }

        public void OnRollJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            SprintInput(value.isPressed);
        }

        public void OnAim(InputValue value)
        {
            AimInput(value.isPressed);
        }

        public void OnSwap(InputValue value)
        {
            SwapInput(value.Get<Vector2>());
           
        }

        public void OnShoot(InputValue value)
        {
            ShootInput(value.isPressed);
        }

        public void OnReload(InputValue value)
        {
            ReloadInput(value.isPressed);
        }

        public void OnTakeCover(InputValue value)
        {
            CoverInput(value.isPressed);
        }


#endif


        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }

        public void MoveInputDir(bool newDirState)
        {
            dir = newDirState;
        }


        public void AimInput(bool newAimState)
        {
            aim = newAimState;
        }

        public void SwapInput(Vector2 newSwapState)
        {
            swap = newSwapState;

        }

       
        public void ShootInput(bool newShootState)
        {
            shoot = newShootState;
        }

        public void ReloadInput(bool newRealodState)
        {
            reload = newRealodState;
        }

        public void CoverInput(bool newCoverState)
        {
            cover = newCoverState;
            

        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
            
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }

}