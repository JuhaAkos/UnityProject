using UnityEngine;

namespace JA {
    public class PlayerLocomotion : MonoBehaviour
    {
        Transform cameraObject;
        InputHandler inputHandler;
        Vector3 moveDirection; //which way we are facing

        [HideInInspector]
        public Transform myTransform;
        [HideInInspector]
        public AnimatorHandler animatorHandler;
        PlayerManager playerManager;

        public new Rigidbody rigidbody;
        //public Gameobject normalCamera;

        [Header("Movement Stats")]
        [SerializeField]
        float movementSpeed = 5;
        [SerializeField]
        float sprintSpeed = 7;
        [SerializeField]
        float rotationSpeed = 10;

        
        void Start(){
            playerManager = GetComponentInParent<PlayerManager>();
            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
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
            if (targetDir == Vector3.zero) {
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

        public void HandleMovement(float delta){

            if (inputHandler.rollFlag)
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

            if (inputHandler.sprintFlag)
            {
                speed = sprintSpeed;
                playerManager.isSprinting = true;
                moveDirection *= speed;
            }
            else
            {
                moveDirection *= speed;
            }            


            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            //give the model the velocity we calculated
            rigidbody.linearVelocity = projectedVelocity;

            //to change between the player models animation files (standing -> walking)
            //Debug.Log(inputHandler.moveAmount);
            //0 -> that is for horizontal
            animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount,0, playerManager.isSprinting);
            

            if (animatorHandler.canRotate){
                HandleRotation(delta);
            }
        }

        public void HandleRollingAndSprinting(float delta) {
            ////Debug.Log("RF: " + inputHandler.rollFlag);
            if (animatorHandler.anim.GetBool("isInteracting"))
                return;
            if (inputHandler.rollFlag) {
                moveDirection = cameraObject.forward * inputHandler.vertical;
                moveDirection += cameraObject.right * inputHandler.horizontal;

                if (inputHandler.moveAmount > 0) {      
                    Debug.Log("rolling rolling rolling...");              
                    animatorHandler.PlayTargetAnimation("Rolling", true);
                    moveDirection.y= 0 ;
                    //roll to move direction
                    Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                    myTransform.rotation = rollRotation;
                }
                else {
                    Debug.Log("tried to take a step back...");
                    animatorHandler.PlayTargetAnimation("Backstep",true);
                }
            }
        }


    }

    

    #endregion
}
