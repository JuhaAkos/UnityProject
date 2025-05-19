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

        public new Rigidbody rigidbody;
        //public Gameobject normalCamera;

        [Header("Stats")]
        [SerializeField]
        float movementSpeed = 5;
        [SerializeField]
        float rotationSpeed = 10;

        void Start(){
            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            cameraObject = Camera.main.transform;
            myTransform = transform;

            animatorHandler.Initialize();
        }

        public void Update()
        {
            //frame update?
            float delta = Time.deltaTime;
            inputHandler.TickInput(delta);

            //our direction = camera direction + player input
            moveDirection = cameraObject.forward * inputHandler.vertical;
            //left and right movement
            moveDirection += cameraObject.right * inputHandler.horizontal;
            //normalize (change length to 1)
            moveDirection.Normalize();
            moveDirection.y = 0;

            float speed = movementSpeed;
            moveDirection *= speed;


            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            //give the model the velocity we calculated
            rigidbody.linearVelocity = projectedVelocity;

            if (animatorHandler.canRotate){
                HandleRotation(delta);
            }
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
    }
    #endregion
}
