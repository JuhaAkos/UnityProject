using UnityEngine;
//using Mathf;

namespace JA {
    public class InputHandler : MonoBehaviour
    {
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;

        //input names based on controller inputs for now
        public bool b_Input;
        public bool a_Input;
        public bool rb_Input;
        public bool rt_Input;

        public bool d_Pad_Up;
        public bool d_Pad_Down;
        public bool d_Pad_Left;
        public bool d_Pad_Right;

        public float rollInputTimer;
        public bool rollFlag;
        public bool sprintFlag;
        //public bool isInteracting;
        public bool comboFlag;

        PlayerControls inputActions;
        PlayerAttacker playerAttacker;
        PlayerInventory playerInventory;
        PlayerManager playerManager;


        //2 axis values -> vector2
        Vector2 movementInput;
        Vector2 cameraInput;

        private void Awake()
        {
            playerAttacker = GetComponent<PlayerAttacker>();
            playerInventory = GetComponent<PlayerInventory>();
            playerManager = GetComponent<PlayerManager>();
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
            HandlerInteractingButtonInput();
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
            b_Input = inputActions.PlayerActions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Performed;

            /*
            //Debug.Log("ezitt: " + UnityEngine.InputSystem.InputActionPhase.Started);
            //started vs performed
            //b_Input = inputActions.PlayerActions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Started;
            b_Input = inputActions.PlayerActions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
            //b_Input = inputActions.PlayerActions.Roll.triggered; 
            Debug.Log("rPhase: " + inputActions.PlayerActions.Roll.phase);
            */

            if (b_Input)
            {
                //Debug.Log("rPhase: " + inputActions.PlayerActions.Roll.triggered);
                //Debug.Log("shift pressed");

                //rollFlag = true;
                rollInputTimer += delta;
                sprintFlag = true;
            }
            else
            {
                //tap input -> roll
                //hold input -> sprint
                if (rollInputTimer > 0 && rollInputTimer < 0.5f)
                {
                    sprintFlag = false;
                    rollFlag = true;
                }

                rollInputTimer = 0;
            }

        }

        private void HandleAttackInput(float delta)
        {
            inputActions.PlayerActions.RB.performed += inputActions => rb_Input = true;
            inputActions.PlayerActions.RT.performed += inputActions => rt_Input = true;

            //right light attack
            if (rb_Input)
            {
                //Debug.Log(playerManager.canDoCombo);
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
                    playerAttacker.HandleLightAttack(playerInventory.rightWeapon);
                }

            }

            if (rt_Input)
            {
                playerAttacker.HandleHeavyAttack(playerInventory.rightWeapon);
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
            inputActions.PlayerActions.A.performed += inputActions => a_Input = true;
            //ui
        }
    }
}
