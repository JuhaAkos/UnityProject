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

        public bool b_Input;
        public float rollInputTimer;
        public bool rollFlag;
        public bool sprintFlag;
        //public bool isInteracting;

        PlayerControls inputActions;
        

        //2 axis values -> vector2
        Vector2 movementInput;
        Vector2 cameraInput;
       

        public void OnEnable(){
            if (inputActions == null) {
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
        }

        //handlemovementinput
        private void MoveInput(float delta){
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
    }
}
