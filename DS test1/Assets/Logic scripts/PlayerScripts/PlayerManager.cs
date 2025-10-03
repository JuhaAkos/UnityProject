using UnityEngine;

namespace JA {
    public class PlayerManager : CharacterManager
    {
        InputHandler inputHandler;
        Animator anim;
        PlayerLocomotion playerLocomotion;
        CameraHandler cameraHandler;

        [Header("Player Flags")]
        public bool isInteracting;
        public bool isSprinting;
        public bool isInAir;
        public bool isGrounded;
        public bool canDoCombo;


        void Start()
        {
            anim = GetComponentInChildren<Animator>();
            isInteracting = anim.GetBool("isInteracting");

            inputHandler = GetComponent<InputHandler>();
            playerLocomotion = GetComponent<PlayerLocomotion>();


            //START ADDED BECAUSE AWAKE WASN'T BEING CALLED AND CAMERAHANDLER DIDN'T GET INITIALIZED
            //cameraHandler = CameraHandler.singleton;     
            cameraHandler = FindObjectOfType<CameraHandler>();

        }

        private void Awake()
        {
            //cameraHandler = CameraHandler.singleton;
            cameraHandler = FindObjectOfType<CameraHandler>();
            //Debug.Log("CM1: " + (cameraHandler!= null));
        }

        void Update()
        {
            //frame update?
            //what is delta?
            float delta = Time.deltaTime;

            isInteracting = anim.GetBool("isInteracting");
            //Debug.Log("Interact? - " + inputHandler.isInteracting);     

            canDoCombo = anim.GetBool("canDoCombo");
            anim.SetBool("isInAir", isInAir);

            inputHandler.TickInput(delta);  
            //FROM LOCOMOTION   
            playerLocomotion.HandleJumping(); 
            playerLocomotion.HandleRollingAndSprinting(delta);        

            CheckForInteractableObject();        
        }

        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;

            /* MOVED
            //Debug.Log("CM: " + (cameraHandler!= null));
            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
            }
            */    

            playerLocomotion.HandleMovement(delta);            
            playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);
            //Debug.Log("isInAir: " + isInAir);
                             
        }


        private void LateUpdate()
        {
            //reseting most flags
            inputHandler.rollFlag = false;
            //inputHandler.sprintFlag = false; //EP23
            inputHandler.rb_Input = false;
            inputHandler.rt_Input = false;
            //isSprinting = inputHandler.b_Input; //bugged, no need
            inputHandler.d_Pad_Up = false;
            inputHandler.d_Pad_Down = false;
            inputHandler.d_Pad_Left = false;
            inputHandler.d_Pad_Right = false;
            inputHandler.a_Input = false;
            inputHandler.jump_Input = false;

            float delta = Time.deltaTime;
            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
            }

            if (isInAir)
            {
                playerLocomotion.inAirTimer = playerLocomotion.inAirTimer + Time.deltaTime;
            }
        }

        public void CheckForInteractableObject()
        {
            RaycastHit hit;

            if (Physics.SphereCast(transform.position, 0.3f, transform.forward, out hit, 1f, cameraHandler.ignoreLayers))
            {
                if (hit.collider.tag == "Interactable")
                {
                    //Debug.Log("HIT");
                    Interactable interactableObject = hit.collider.GetComponent<Interactable>();
                    if (interactableObject != null)
                    {
                        string interactableText = interactableObject.interactableText;
                        //set ui text to interact obj's
                        //ui pop-up

                        if (inputHandler.a_Input)
                        {
                            hit.collider.GetComponent<Interactable>().Interact(this);
                        }
                    }
                }
            }
        }

}
}