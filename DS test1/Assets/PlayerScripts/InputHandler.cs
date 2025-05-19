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

        PlayerControls inputActions;
        CameraHandler cameraHandler;

        //2 axis values -> vector2
        Vector2 movementInput;
        Vector2 cameraInput;

        private void Awake(){
            cameraHandler = CameraHandler.singleton;
            //Debug.Log("CM1: " + (cameraHandler!= null));
        }

        //START ADDED BECAUSE AWAKE WASN'T BEING CALLED AND CAMERAHANDLER DIDN'T GET INITIALIZED
        private void Start(){
            cameraHandler = CameraHandler.singleton;
            //Debug.Log("CM1: " + (cameraHandler!= null));
        }

        private void FixedUpdate(){
            float delta = Time.fixedDeltaTime;

            //Debug.Log("CM: " + (cameraHandler!= null));
            if (cameraHandler != null) {
                
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, mouseX, mouseY);

            }
        }

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
    }
}
