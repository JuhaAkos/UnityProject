using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using Mathf;

namespace JA {
    //rigidbody interpolation was changed from default
    //explanation video CAMERA

    public class CameraHandler : MonoBehaviour
    {
        InputHandler inputHandler;
        PlayerManager playerManager;

        public Transform targetTransform; //camera follows this object
        public Transform cameraTransform;
        public Transform cameraPivotTransform; //up and down pivot?
        private Transform myTransform;
        private Vector3 cameraTransformPosition; //to also use camera z position changes
        public LayerMask ignoreLayers;

        private Vector3 cameraFollowVelocity = Vector3.zero; //for camspeed

        public static CameraHandler singleton;

        //public float lookSpeed = 0.1f;
        public float followSpeed = 0.08f;
        //public float pivotSpeed = 0.02f;

        //EP23 CUSTOM FIX
        public float lookSpeed = 260f;
        public float pivotSpeed = 100f;


        private float defaultPosition;
        private float lookAngle; //left and right
        private float pivotAngle; //up and down

        public float minimumPivot = -35;
        public float maximumPivot = 35;

        private float targetPosition;
        public float cameraSphereRadius = 0.2f;
        public float cameraCollisionOffset = 0.2f; //how much the camera will bounce back from objects it collides with
        public float minimumCollisionOffset = 0.2f;

        //lockon
        public Transform currentLockOnTarget;
        List<CharacterManager> availableTargets = new List<CharacterManager>();
        public Transform nearestLockOnTarget;
        public Transform leftLockTarget;
        public Transform rightLockTarget;
        public float maximumLockOnDistance = 30;
        //lockon and non-lockon camera height
        public float lockedPivotPosition = 2.25f;
        public float unlockedPivotPosition = 1.65f;
        public LayerMask enviromentLayer;

        private void Awake()
        {
            singleton = this; //camhandler init?
            myTransform = transform;
            defaultPosition = cameraTransform.localPosition.z;

            //defines how camera interacts with surfaces
            ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
            targetTransform = FindObjectOfType<PlayerManager>().transform;
            inputHandler = FindObjectOfType<InputHandler>();
            playerManager = FindObjectOfType<PlayerManager>();
        }

        private void Start()
        {
            enviromentLayer = LayerMask.NameToLayer("Enviroment");
        }

        public void FollowTarget(float delta)
        {
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
        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
        {
            if (inputHandler.lockOnFlag == false && currentLockOnTarget == null)
            {
                //lookAngle += (mouseXInput * lookSpeed) / delta;                       
                //pivotAngle -= (mouseYInput * pivotSpeed) / delta;

                //EP 23 custom fix
                lookAngle += mouseXInput * lookSpeed * delta;
                pivotAngle -= mouseYInput * pivotSpeed * delta;
                //Debug.Log("lookA: " + lookAngle + ", pivotA: " + pivotAngle);

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
            //force rotation on lockon
            else
            {
                float velocity = 0;

                Vector3 dir = currentLockOnTarget.position - targetTransform.position;
                dir.Normalize();
                dir.y = 0;

                Quaternion targetRotation = Quaternion.LookRotation(dir);
                transform.rotation = targetRotation;

                dir = currentLockOnTarget.position - cameraPivotTransform.position;
                dir.Normalize();

                targetRotation = Quaternion.LookRotation(dir);
                Vector3 eulerAngle = targetRotation.eulerAngles;
                eulerAngle.y = 0;
                cameraPivotTransform.localEulerAngles = eulerAngle;
            }

        }

        private void handleCameraCollisions(float delta)
        {
            targetPosition = defaultPosition; //reset camera position after a surface brings it too close
            RaycastHit hit;
            Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
            direction.Normalize();

            //SPHERECAST: sphere around the camera and if it collides with anything gives TRUE
            //hit variable: stores information on what we hit
            if (Physics.SphereCast(cameraPivotTransform.position, cameraSphereRadius, direction, out hit, Mathf.Abs(targetPosition), ignoreLayers))
            {

                float dis = Vector3.Distance(cameraPivotTransform.position, hit.point);
                targetPosition = -(dis - cameraCollisionOffset);
            }

            if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
            {
                targetPosition = -minimumCollisionOffset;
            }

            cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.2f);

            cameraTransform.localPosition = cameraTransformPosition;
        }

        public void HandleLockOn()
        {
            availableTargets.Clear();
            float shortestDistance = Mathf.Infinity;
            float shortestDistanceOfLeftTarget = Mathf.Infinity;
            float shortestDistanceOfRightTarget = Mathf.Infinity;

            Collider[] colliders = Physics.OverlapSphere(targetTransform.position, 26);

            //create list on lockon targets
            for (int i = 0; i < colliders.Length; i++)
            {

                CharacterManager character = colliders[i].GetComponent<CharacterManager>();

                if (character != null)
                {
                    Vector3 lockTargetDirection = character.transform.position - targetTransform.position;
                    float distanceFromTarget = Vector3.Distance(targetTransform.position, character.transform.position);
                    float viewableAngle = Vector3.Angle(lockTargetDirection, cameraTransform.forward);

                    RaycastHit hit;

                    //do not lock onto yourself, do not lock onto most things unseen
                    if (character.transform.root != targetTransform.transform.root
                        && viewableAngle > -75 && viewableAngle < 75
                        && distanceFromTarget <= maximumLockOnDistance)
                    {
                        //LINECAST: create a raycast line between two points -> good for line of sight checks
                        if (Physics.Linecast(playerManager.lockOnTransform.position, character.lockOnTransform.position, out hit))
                        {
                            Debug.DrawLine(playerManager.lockOnTransform.position, character.lockOnTransform.position);

                            if (hit.transform.gameObject.layer == enviromentLayer)
                            {
                                //no lock on because of coverage
                            }
                            else
                            {
                                availableTargets.Add(character);
                                //Debug.Log("LENGTH: " + availableTargets.Count);
                            }
                        }
                    }
                }
            }

            //find closestlockon
            for (int k = 0; k < availableTargets.Count; k++)
            {
                //Debug.Log("FOR k: " + k);
                float distanceFromTarget = Vector3.Distance(targetTransform.position, availableTargets[k].transform.position);

                if (distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    nearestLockOnTarget = availableTargets[k].lockOnTransform;
                }

                if (inputHandler.lockOnFlag)
                {
                    //if they are on the same x coordinate -> zero value -> not added
                    Vector3 relativeEnemyPosition = currentLockOnTarget.InverseTransformPoint(availableTargets[k].transform.position);
                    //Debug.Log("Relative.x: " + relativeEnemyPosition.x);
                    //Debug.Log("Relative.x calc: " + currentLockOnTarget.InverseTransformPoint(availableTargets[k].transform.position));
                    //Debug.Log("Target: " + currentLockOnTarget);
                    //changing between apllicable targets
                    var distanceFromLeftTarget = currentLockOnTarget.transform.position.x - availableTargets[k].transform.position.x;
                    var distanceFromRightTarget = currentLockOnTarget.transform.position.x + availableTargets[k].transform.position.x;

                    /*
                    Debug.Log("Relative.x: " + relativeEnemyPosition.x + " -> bigger than zero? - " + (relativeEnemyPosition.x > 0.00f) +
                        "\nL (>0) - distance from L: " + distanceFromLeftTarget + ", SHORTEST dist. from L: " + shortestDistanceOfLeftTarget +
                       "\nR (<0) - distance from R: " + distanceFromRightTarget + ", SHORTEST dist. from R: " + shortestDistanceOfRightTarget

                    );
                    */

                    if (relativeEnemyPosition.x > 0.00f && distanceFromLeftTarget < shortestDistanceOfLeftTarget)
                    {
                        //Debug.Log("found better on left");
                        shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                        leftLockTarget = availableTargets[k].lockOnTransform;
                    }

                    if (relativeEnemyPosition.x < 0.00f && distanceFromRightTarget < shortestDistanceOfRightTarget)
                    {
                        //Debug.Log("found better on right");
                        shortestDistanceOfRightTarget = distanceFromRightTarget;
                        rightLockTarget = availableTargets[k].lockOnTransform;
                    }
                }
            }

            
        }

        public void ClearLockOnTargets()
        {
            availableTargets.Clear();
            nearestLockOnTarget = null;
            currentLockOnTarget = null;
        }

        public void SetCameraHeight()
        {
            Vector3 velocity = Vector3.zero;
            Vector3 newLockedPosition = new Vector3(0, lockedPivotPosition);
            Vector3 newUnlockedPosition = new Vector3(0, unlockedPivotPosition);

            if (currentLockOnTarget != null)
            {
                cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newLockedPosition, ref velocity, Time.deltaTime);
            }
            else
            {
                cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newUnlockedPosition, ref velocity, Time.deltaTime);
            }
        }
    }
}