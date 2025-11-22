using UnityEngine;

namespace JA {
    public class WakeUpState : State
    {
        [SerializeField] PursueTargetState pursueTargetState;
        [SerializeField] DeadState deadState;
        [SerializeField] LayerMask detectionLayer;  
        
        [SerializeField] float wakeUpTimer = 0;

       private void Awake()
        {
            //handled in inspector
            //pursueTargetState = GetComponentInChildren<PursueTargetState>();            
        }


        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorHandler enemyAnimatorHandler)
        {
            if (enemyStats.isDead)
            {
                return deadState;
            }

            wakeUpTimer += Time.deltaTime;

            if (wakeUpTimer > 2)
            {
                HandleRotateTowardsTarget(enemyManager);
            }

            #region switch state
            if (wakeUpTimer > 4)
            {
                return pursueTargetState;
            }
            else
            {
                return this;
            }
            #endregion
        }

        private void HandleRotateTowardsTarget(EnemyManager enemyManager)
        {
            enemyManager.navmeshAgent.transform.localPosition = Vector3.zero;

            Vector3 targetDirection = enemyManager.currentTarget.transform.position - transform.position;
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);
            float viewableAngle = Vector3.Angle(targetDirection, transform.forward);


            Vector3 relativeDirection = transform.InverseTransformDirection(enemyManager.navmeshAgent.desiredVelocity);

            distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);
            {
                Vector3 targetVelocity = enemyManager.enemyRigidBody.linearVelocity;

                enemyManager.navmeshAgent.enabled = true;
                enemyManager.navmeshAgent.SetDestination(enemyManager.currentTarget.transform.position);
                enemyManager.enemyRigidBody.linearVelocity = targetVelocity;
                //ENEMYMANAGER.transform!!!
                enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, enemyManager.navmeshAgent.transform.rotation, enemyManager.rotationSpeed / 20000 / Time.deltaTime);

            }

        }
    }
}