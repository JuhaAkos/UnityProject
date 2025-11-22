using UnityEngine;

namespace JA {
    public class PlayerManager : CharacterManager
    {
        InputHandler inputHandler;
        Animator anim;
        PlayerLocomotion playerLocomotion;
        CameraHandler cameraHandler;
        PlayerStats playerStats;

        [Header("Player Flags")]
        public bool isInteracting;
        public bool isSprinting;
        public bool isInAir;
        public bool isGrounded;
        public bool canDoCombo;
        public bool isUsingRightHand;
        public bool isUsingLeftHand;
        public bool isInvulnerable;


        void Start()
        {
            anim = GetComponentInChildren<Animator>();
            isInteracting = anim.GetBool("isInteracting");

            inputHandler = GetComponent<InputHandler>();
            playerLocomotion = GetComponent<PlayerLocomotion>();

            playerStats = GetComponent<PlayerStats>();


            //START ADDED BECAUSE AWAKE WASN'T BEING CALLED AND CAMERAHANDLER DIDN'T GET INITIALIZED
            //cameraHandler = CameraHandler.singleton;     
            cameraHandler = FindFirstObjectByType<CameraHandler>();

        }

        private void Awake()
        {
            cameraHandler = FindFirstObjectByType<CameraHandler>();
        }

        void Update()
        {
            float delta = Time.deltaTime;

            isInteracting = anim.GetBool("isInteracting");

            canDoCombo = anim.GetBool("canDoCombo");
            isInvulnerable = anim.GetBool("isInvulnerable");
            anim.SetBool("isInAir", isInAir);

            isUsingRightHand = anim.GetBool("isUsingRightHand");
            isUsingLeftHand = anim.GetBool("isUsingLeftHand");

            inputHandler.TickInput(delta);  
            playerLocomotion.HandleRollingAndSprinting(delta);

            playerStats.RegenrateStamina();     
        }

        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;
            playerLocomotion.HandleMovement(delta);            
                             
        }

        private void LateUpdate()
        {
            //reseting most flags
            inputHandler.rollFlag = false;
            inputHandler.lightA_Input = false;
            inputHandler.heavyA_Input = false;
            inputHandler.estus_Input = false;

            inputHandler.esc_Input = false;

            float delta = Time.deltaTime;
            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
            }
        }


}
}