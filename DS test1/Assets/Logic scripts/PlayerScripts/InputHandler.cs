using UnityEngine;
using UnityEngine.SceneManagement;
//using Mathf;

namespace JA
{
    public class InputHandler : MonoBehaviour
    {
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;

        //input names based on controller inputs for now
        public bool g_Input;
        public bool b_Input;
        public bool a_Input;
        public bool rb_Input;
        public bool rt_Input;
        public bool jump_Input;

        public bool d_Pad_Up;
        public bool d_Pad_Down;
        public bool d_Pad_Left;
        public bool d_Pad_Right;
        public bool esc_Input;
        public bool lockOnInput;
        //for lockon change
        public bool right_Stick_Right_Input;
        public bool right_Stick_Left_Input;

        public float rollInputTimer;
        public bool rollFlag;
        public bool sprintFlag;
        //public bool isInteracting;
        public bool comboFlag;
        public bool lockOnFlag;

        PlayerControls inputActions;
        PlayerAttacker playerAttacker;
        PlayerInventory playerInventory;
        PlayerManager playerManager;
        PlayerStats playerStats;
        CameraHandler cameraHandler;
        AnimatorHandler animatorHandler;


        //2 axis values -> vector2
        Vector2 movementInput;
        Vector2 cameraInput;

        private void Awake()
        {
            playerAttacker = GetComponent<PlayerAttacker>();
            playerInventory = GetComponent<PlayerInventory>();
            playerManager = GetComponent<PlayerManager>();
            playerStats = GetComponent<PlayerStats>();
            cameraHandler = FindObjectOfType<CameraHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
        }
        public void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerControls();

                //record movement input
                inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
                //record camera angle change?           
                inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();


                //handle attacks
                inputActions.PlayerActions.RB.performed += i => rb_Input = true;
                inputActions.PlayerActions.RT.performed += i => rt_Input = true;
                //interact
                inputActions.PlayerActions.A.performed += i => a_Input = true;
                inputActions.PlayerActions.Estus.performed += i => g_Input = true;

                inputActions.PlayerActions.Roll.performed += i => b_Input = true;
                inputActions.PlayerActions.Roll.canceled += inputActions => b_Input = false;

                inputActions.MainMenuUINavigation.ESC.performed += i => esc_Input = true;

                //jump
                inputActions.PlayerActions.Jump.performed += i => jump_Input = true;

                inputActions.PlayerActions.LockOn.performed += i => lockOnInput = true;
                inputActions.PlayerMovement.LockOnTargetRight.performed += i => right_Stick_Right_Input = true;
                inputActions.PlayerMovement.LockOnTargetLeft.performed += i => right_Stick_Left_Input = true;
            }

            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        public void TickInput(float delta)
        {
            MoveInput(delta);
            HandleRollInput(delta);
            HandleAttackInput(delta);
            HandleQuickSlotsInput();
            //HandlerInteractingButtonInput();
            //HandleJumpInput();
            HandleLockOnInput();
            HandleEstusInput();
            HandleEscInput();
        }

        //handlemovementinput
        private void MoveInput(float delta)
        {
            horizontal = movementInput.x;
            vertical = movementInput.y;
            //clamp -> limit value between 0 and 1
            //make h and v values non negative -> calculate movement amount
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            //no anim manager call? (other playlist)
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }

        private void HandleRollInput(float delta)
        {
            //b_Input = inputActions.PlayerActions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Started;

            //b_Input = inputActions.PlayerActions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
            //sprintFlag = b_Input;

            if (b_Input)
            {
                //Debug.Log("rPhase: " + inputActions.PlayerActions.Roll.triggered);
                //Debug.Log("shift pressed");

                //rollFlag = true;
                rollInputTimer += delta;
                //sprintFlag = true;

                if (playerStats.currentStamina <= 0)
                {
                    b_Input = false;
                    sprintFlag = false;
                }
                
                if (moveAmount > 0.5f && playerStats.currentStamina > 0)
                {
                    sprintFlag = true;
                }
            }
            else
            {
                sprintFlag = false;
                //tap input -> roll
                //hold input -> sprint
                if (rollInputTimer > 0 && rollInputTimer < 0.5f)
                {
                    //sprintFlag = false;
                    rollFlag = true;
                }

                rollInputTimer = 0;
            }

        }

        private void HandleAttackInput(float delta)
        {
            //MOVED ELSEWHERE FOR BETTER PERFORMANCE
            //inputActions.PlayerActions.RB.performed += inputActions => rb_Input = true;
            //inputActions.PlayerActions.RT.performed += inputActions => rt_Input = true;

            //right light attack
            if (rb_Input)
            {
                if (playerManager.canDoCombo)
                {
                    comboFlag = true;
                    playerAttacker.HandleWeaponCombo(playerInventory.rightWeapon);
                    comboFlag = false;
                }
                else
                {
                    if (playerManager.isInteracting)
                    {
                        return;
                    }
                    if (playerManager.canDoCombo)
                    {
                        return;
                    }

                    animatorHandler.anim.SetBool("isUsingRightHand", true);
                    playerAttacker.HandleLightAttack(playerInventory.rightWeapon);
                }

            }

            if (rt_Input)
            {                
                if (playerManager.canDoCombo)
                {
                    comboFlag = true;
                    playerAttacker.HandleWeaponCombo(playerInventory.rightWeapon);
                    comboFlag = false;
                }
                else
                {
                    if (playerManager.isInteracting)
                    {
                        return;
                    }
                    if (playerManager.canDoCombo)
                    {
                        return;
                    }

                    animatorHandler.anim.SetBool("isUsingRightHand", true);
                    playerAttacker.HandleHeavyAttack(playerInventory.rightWeapon);
                }
            }
        }

        private void HandleQuickSlotsInput()
        {
            inputActions.PlayerInventoryActions.DPadRight.performed += inputActions => d_Pad_Right = true;
            inputActions.PlayerInventoryActions.DPadLeft.performed += inputActions => d_Pad_Left = true;
            if (d_Pad_Right)
            {
                playerInventory.ChangeRightWeapon();
            }
            else if (d_Pad_Left)
            {
                playerInventory.ChangeLeftWeapon();
            }
        }

        private void HandlerInteractingButtonInput()
        {
            //Debug.Log("pressed F to interact!")
            //MOVED ELSEWERE FOR PERFORMANCE
            //inputActions.PlayerActions.A.performed += inputActions => a_Input = true;
            //ui
        }

        private void HandleJumpInput()
        {
            //MOVED ELSEWERE FOR PERFORMANCE
            //inputActions.PlayerActions.Jump.performed += inputActions => jump_Input = true;
        }

        private void HandleLockOnInput()
        {
            //trying to lockon but not locked on
            if (lockOnInput && lockOnFlag == false)
            {
                lockOnInput = false;
                cameraHandler.currentLockOnTarget = cameraHandler.nearestLockOnTarget;
                cameraHandler.HandleLockOn();

                if (cameraHandler.nearestLockOnTarget != null)
                {
                    cameraHandler.currentLockOnTarget = cameraHandler.nearestLockOnTarget;
                    lockOnFlag = true;
                }
            }
            //trying to lock on and already locke on
            else if (lockOnInput && lockOnFlag)
            {
                lockOnFlag = false;
                lockOnInput = false;
                cameraHandler.ClearLockOnTargets();
            }

            if (lockOnFlag && right_Stick_Left_Input)
            {
                right_Stick_Left_Input = false;
                cameraHandler.HandleLockOn();
                Debug.Log("Left T: " + cameraHandler.leftLockTarget);
                if (cameraHandler.leftLockTarget != null)
                {
                    cameraHandler.currentLockOnTarget = cameraHandler.leftLockTarget;
                }
            }
            else if (lockOnFlag && right_Stick_Right_Input)
            {
                right_Stick_Right_Input = false;
                cameraHandler.HandleLockOn();
                Debug.Log("Right T: " + cameraHandler.rightLockTarget);
                if (cameraHandler.rightLockTarget != null)
                {
                    cameraHandler.currentLockOnTarget = cameraHandler.rightLockTarget;
                }
            }

            cameraHandler.SetCameraHeight();
        }
    
        private void HandleEstusInput()
        {
            //inevtory -> holderslot
            //stats -> anim  
            if (g_Input)
            {
                playerStats.UseEstus();
            }
                      
        }
    
        private void HandleEscInput()
        {
            if (esc_Input)
            {
                Debug.Log("Exited early - ESC");
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
    
}
