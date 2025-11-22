using UnityEngine;

namespace JA {
    public class PlayerLocomotion : MonoBehaviour
    {
        Transform cameraObject;
        InputHandler inputHandler;
        [HideInInspector]
        public Transform myTransform;
        [HideInInspector]
        [SerializeField] AnimatorHandler animatorHandler;
        PlayerManager playerManager;
        PlayerStats playerStats;
        [SerializeField] CameraHandler cameraHandler;

        public Vector3 moveDirection; //which way we are facing
        //public for fall movement

        [SerializeField] new Rigidbody rigidbody;
        //public Gameobject normalCamera;

        [Header("Movement Stats")]
        [SerializeField]
        float movementSpeed = 3;
        [SerializeField]
        float sprintSpeed = 6;
        [SerializeField]
        float rotationSpeed = 10;

        //seri used instead of public for inspector
        [Header("Stamina Costs")]
        [SerializeField] int rollStaminaCost = 6;
        [SerializeField] int backstepStaminaCost = 12;
        [SerializeField] int sprintStaminaCost = 1;

        private void Awake()
        {
            playerManager = GetComponentInParent<PlayerManager>();
            playerStats = GetComponent<PlayerStats>();
            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            cameraHandler = FindFirstObjectByType<CameraHandler>();
        }

        void Start()
        {
            cameraObject = Camera.main.transform;
            myTransform = transform;

            animatorHandler.Initialize();
        }


        #region Movement
        Vector3 normalVector;
        Vector3 targetPosition;

        private void HandleRotation(float delta)
        {
            //starts out as all zeroes
            Vector3 targetDir = Vector3.zero;
            float moveOverride = inputHandler.moveAmount;

            //roation direction calculation then normalize
            targetDir = cameraObject.forward * inputHandler.vertical;
            targetDir += cameraObject.right * inputHandler.horizontal;

            targetDir.Normalize();
            //just horizontaly
            targetDir.y = 0;

            //no movement -> keep the direction we look into straight
            if (targetDir == Vector3.zero)
            {
                targetDir = myTransform.forward;
            }

            float rs = rotationSpeed;

            //Quaternion is used for rotations in Unity
            //look into direction
            Quaternion tr = Quaternion.LookRotation(targetDir);
            //slerp -> turn from A point to B with speed of C
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);

            myTransform.rotation = targetRotation;
        }

        public void HandleMovement(float delta)
        {

            if (inputHandler.rollFlag)
            {
                return;
            }

            if (playerManager.isInteracting)
            {
                return;
            }

            //our direction = camera direction + player input
            moveDirection = cameraObject.forward * inputHandler.vertical;
            //left and right movement
            moveDirection += cameraObject.right * inputHandler.horizontal;
            //normalize (change length to 1)
            moveDirection.Normalize();
            moveDirection.y = 0;

            float speed = movementSpeed;

            if (inputHandler.sprintFlag && inputHandler.moveAmount > 0.5)
            {
                speed = sprintSpeed;
                playerManager.isSprinting = true;
                moveDirection *= speed;
                playerStats.TakeStaminaDamage(sprintStaminaCost);
            }
            else
            {
                if (inputHandler.moveAmount < 0.5)
                {
                    //moveDirection *= walkingSpeed;
                    moveDirection *= movementSpeed;
                    playerManager.isSprinting = false;
                }
                else
                {
                    moveDirection *= speed;
                    playerManager.isSprinting = false;
                }
            }


            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            //give the model the velocity we calculated
            rigidbody.linearVelocity = projectedVelocity;


            //LOCKON
            if (inputHandler.lockOnFlag && inputHandler.sprintFlag == false)
            {
                //blendtree with both horizontal and vertical values
                animatorHandler.UpdateAnimatorValues(inputHandler.vertical, inputHandler.horizontal, playerManager.isSprinting);
            }
            else
            {
                animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0, playerManager.isSprinting);
            }


            //to change between the player models animation files (standing -> walking)
            //Debug.Log(inputHandler.moveAmount);
            //0 -> that is for horizontal
            animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0, playerManager.isSprinting);

            if (animatorHandler.canRotate)
            {
                HandleRotation(delta);
            }
        }

        public void HandleRollingAndSprinting(float delta)
        {
            if (animatorHandler.anim.GetBool("isInteracting"))
                return;

            if (playerStats.currentStamina <= 0)
            {
                return;
            }


            if (inputHandler.rollFlag)
            {
                moveDirection = cameraObject.forward * inputHandler.vertical;
                moveDirection += cameraObject.right * inputHandler.horizontal;

                if (inputHandler.moveAmount > 0)
                {
                    animatorHandler.PlayTargetAnimation("Rolling", true);
                    moveDirection.y = 0;
                    //roll to move direction
                    Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                    myTransform.rotation = rollRotation;
                    playerStats.TakeStaminaDamage(rollStaminaCost);
                }
                else
                {
                    animatorHandler.PlayTargetAnimation("Backstep", true);
                    playerStats.TakeStaminaDamage(backstepStaminaCost);
                }
            }
        }

        public void LockOnRotation()
        {
            Vector3 dir = cameraHandler.currentLockOnTarget.position - myTransform.position;
            //Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, cameraHandler.currentLockOnTarget.rotation, rotationSpeed / Time.deltaTime);
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, Quaternion.LookRotation(dir), rotationSpeed / Time.deltaTime);
            targetRotation.x = 0;
            targetRotation.z = 0;

            myTransform.rotation = targetRotation;
        }

    }

    

    #endregion
}
