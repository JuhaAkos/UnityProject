using UnityEngine;
//using Mathf;

namespace JA {
    //rigidbody interpolation was changed from default
    //explanation video CAMERA

    public class CameraHandler : MonoBehaviour
    {
        public Transform targetTransform; //camera follows this object
        public Transform cameraTransform;
        public Transform cameraPivotTransform; //up and down pivot?
        private Transform myTransform;
        private Vector3 cameraTransformPosition; //to also use camera z position changes
        public LayerMask ignoreLayers;

        private Vector3 cameraFollowVelocity = Vector3.zero; //for camspeed

        public static CameraHandler singleton;

        public float lookSpeed = 0.1f;
        public float followSpeed = 0.08f;
        public float pivotSpeed = 0.02f;

        private float defaultPosition;
        private float lookAngle; //left and right
        private float pivotAngle; //up and down

        public float minimumPivot = -35;
        public float maximumPivot = 35;

        private float targetPosition;
        public float cameraSphereRadius = 0.2f;
        public float cameraCollisionOffset = 0.2f; //how much the camera will bounce back from objects it collides with
        public float minimumCollisionOffset = 0.2f;

        private void Awake()
        {
            singleton = this; //camhandler init?
            myTransform = transform;
            defaultPosition = cameraTransform.localPosition.z;

            //defines how camera interacts with surfaces
            ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
            targetTransform = FindObjectOfType<PlayerManager>().transform;
        }

        public void FollowTarget(float delta) {
            //"lerp" move between the first two argument positions
            //Vector3 targetPosition = Vector3.Lerp(myTransform.position, targetTransform.position, delta / followSpeed);
            
            //changed lerp to SMOOTHDAMP to be less "jittery"
            //updates position to targetTposition
            Vector3 targetPosition = Vector3.SmoothDamp(
            myTransform.position, targetTransform.position, ref cameraFollowVelocity, delta / followSpeed);
            myTransform.position = targetPosition;

            handleCameraCollisions(delta);
        }

        //left and right rotations should have a fix barrier they can't pass through
        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput) {
            lookAngle += (mouseXInput * lookSpeed) / delta;
                       
            pivotAngle -= (mouseYInput * pivotSpeed) / delta;
            //cuts pivotangle value between the boundary 
            pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

            //horizontal rotation
            Vector3 rotation = Vector3.zero;
            rotation.y = lookAngle;
            Quaternion targetRotation = Quaternion.Euler(rotation);
            myTransform.rotation = targetRotation;

            rotation = Vector3.zero;
            rotation.x = pivotAngle;
            targetRotation = Quaternion.Euler(rotation);
            cameraPivotTransform.localRotation = targetRotation;
        }

        private void handleCameraCollisions(float delta) {
            targetPosition = defaultPosition; //reset camera position after a surface brings it too close
            RaycastHit hit;
            Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
            direction.Normalize();

            //SPHERECAST: sphere around the camera and if it collides with anything gives TRUE
            //hit variable: stores information on what we hit
            if (Physics.SphereCast(cameraPivotTransform.position, cameraSphereRadius, direction, out hit, Mathf.Abs(targetPosition), ignoreLayers)) {

                float dis = Vector3.Distance(cameraPivotTransform.position, hit.point);
                targetPosition = -(dis - cameraCollisionOffset);
            }

            if (Mathf.Abs(targetPosition) < minimumCollisionOffset){
                targetPosition = -minimumCollisionOffset;
            }

            cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.2f);

            cameraTransform.localPosition = cameraTransformPosition;
        }

    }
}