using UnityEngine;
using UnityEngine.AI;

namespace JA
{
    public class EnemyLocomotionManager : MonoBehaviour
    {
        EnemyManager enemyManager;
        EnemyAnimatorHandler enemyAnimatorHandler;

        public CapsuleCollider characterCollider;
        public CapsuleCollider characterCollisionBlockerCollider;


        private void Awake()
        {
            enemyManager = GetComponent<EnemyManager>();
            enemyAnimatorHandler = GetComponentInChildren<EnemyAnimatorHandler>();
        }
        
        private void Start()
        {
             //stop player and enemies from pushing eachother
            Physics.IgnoreCollision(characterCollider, characterCollisionBlockerCollider, true);
        }

        //is player visible
        public void HandleDetection()
        {
            /*
            Collider[] colliders = Physics.OverlapSphere(transform.position, enemyManager.detectionRadius, detectionLayer);

            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterStats characterStats = colliders[i].transform.GetComponent<CharacterStats>();

                if (characterStats != null)
                {
                    //check TEAM ID
                    //Until that -> detects itself if set to player layer
                    Vector3 targetDirection = characterStats.transform.position - transform.position;
                    float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

                    if (viewableAngle > enemyManager.minimumDetectionAngle && viewableAngle < enemyManager.maximumDetectionAngle)
                    {
                        currentTarget = characterStats;
                    }
                }
            }
            */
        }

        public void HandleMoveToTarget()
        {
            /*
            if (enemyManager.isPerformingAction)
            {
                return;
            }

            Vector3 targetDirection = enemyManager.currentTarget.transform.position - transform.position;
            distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, transform.position);
            float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

            if (enemyManager.isPerformingAction)
            {
                //if attacking/action no speed needed
                enemyAnimatorHandler.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                navmeshAgent.enabled = false;
            }
            else
            {
                if (distanceFromTarget > stoppingDistance)
                {
                    enemyAnimatorHandler.anim.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);

                    /*
                    //atmen fix - no movement without it
                    targetDirection.Normalize();
                    targetDirection.y = 0;
                    float speed = 3;
                    targetDirection *= speed;

                    Vector3 projectedVelocity = Vector3.ProjectOnPlane(targetDirection, Vector3.up);

                    enemyRigidBody.linearVelocity = projectedVelocity;
                    //maybe caused by root motion / lack thereof   
                    */
                    /*               
                }
                else if (distanceFromTarget <= stoppingDistance)
                {
                    enemyAnimatorHandler.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                }
            }

            HandleRotateTowardsTarget();

            navmeshAgent.transform.localPosition = Vector3.zero;
            navmeshAgent.transform.localRotation = Quaternion.identity;
            */
        }
    }
}    
