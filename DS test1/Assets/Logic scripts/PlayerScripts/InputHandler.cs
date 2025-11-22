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
        public bool estus_Input;
        public bool b_Input;
        public bool lightA_Input;
        public bool heavyA_Input;

        public bool esc_Input;
        public bool lockOnInput;

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
            cameraHandler = FindFirstObjectByType<CameraHandler>();
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
                inputActions.PlayerActions.LightInput.performed += i => lightA_Input = true;
                inputActions.PlayerActions.HeavyInput.performed += i => heavyA_Input = true;
                inputActions.PlayerActions.Estus.performed += i => estus_Input = true;

                inputActions.PlayerActions.Roll.performed += i => b_Input = true;
                inputActions.PlayerActions.Roll.canceled += inputActions => b_Input = false;

                inputActions.MainMenuUINavigation.ESC.performed += i => esc_Input = true;

                inputActions.PlayerActions.LockOn.performed += i => lockOnInput = true;
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
            if (b_Input)
            {
                rollInputTimer += delta;

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
            //right light attack
            if (lightA_Input)
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

            if (heavyA_Input)
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

            cameraHandler.SetCameraHeight();
        }
    
        private void HandleEstusInput()
        {
            //inevtory -> holderslot
            //stats -> anim  
            if (estus_Input)
            {
                playerStats.UseEstus();
            }
                      
        }
    
        private void HandleEscInput()
        {
            if (esc_Input)
            {
                Debug.Log("Exited early - ESC");
                Cursor.visible = true;
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
    
}
