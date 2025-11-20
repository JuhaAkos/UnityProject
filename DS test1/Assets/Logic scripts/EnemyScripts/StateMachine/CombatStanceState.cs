using UnityEngine;

namespace JA
{
    public class CombatStanceState : State
    {
        public AttackState attackState;
        public DeadState deadState;
        public PursueTargetState pursueTargetState;

        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorHandler enemyAnimatorHandler)
        {
            if (enemyStats.isDead)
            {
                return deadState;
            }

            if (enemyManager.isPerformingAction)
            {
                enemyAnimatorHandler.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                //Debug.Log("returned");
                return this;
            }
            //check attack range
            //chance to circle or walk around target
            //ready to attack -> switch
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);

            enemyManager.navmeshAgent.transform.localPosition = Vector3.zero;
            enemyManager.navmeshAgent.transform.localRotation = Quaternion.identity;

            HandleRotateTowardsTarget(enemyManager);
            /*
            if (enemyManager.isPerformingAction)
            {
                //enemyAnimatorHandler.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                return this;
            }
            */

            //Debug.Log("dist: " + distanceFromTarget + " ,max dist: " + enemyManager.maximumAttackRange);
            if (enemyManager.currentRecoveryTime <= 0 && distanceFromTarget <= enemyManager.maximumAttackRange)
            {
                return attackState;
            }
            else if (distanceFromTarget > enemyManager.maximumAttackRange && !enemyManager.isPerformingAction)
            {
                return pursueTargetState;
            }
            else
            {
                //HandleRotateTowardsTarget(enemyManager);
                return this;
            }
        }

        private void HandleRotateTowardsTarget(EnemyManager enemyManager)
        {
            //manual rotate
            if (enemyManager.isPerformingAction)
            {

                //Debug.Log("isperforming");
                Vector3 direction = enemyManager.currentTarget.transform.position - transform.position;
                direction.y = 0;
                direction.Normalize();

                if (direction == Vector3.zero)
                {
                    direction = transform.forward;
                }

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                enemyManager.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, enemyManager.rotationSpeed * Time.deltaTime);


            }
            //navmash rotate if no action
            else
            {
                
                Vector3 relativeDirection = transform.InverseTransformDirection(enemyManager.navmeshAgent.desiredVelocity);

                float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);
                //if (distanceFromTarget > enemyManager.maximumAttackRange)
                {
                    //Debug.Log("not performing");
                    /*
                    Vector3 direction = enemyManager.currentTarget.transform.position - transform.position;

                    direction.Normalize();
                    direction.y = 0;
                    float speed = 3;

                    direction *= speed;

                    Vector3 projectedVelocity = Vector3.ProjectOnPlane(direction, Vector3.up);

                    Vector3 targetVelocity = projectedVelocity; // Everything in the IF statement from this line and above this line is new
                    */

                    Vector3 targetVelocity = enemyManager.enemyRigidBody.linearVelocity;

                    enemyManager.navmeshAgent.enabled = true;
                    enemyManager.navmeshAgent.SetDestination(enemyManager.currentTarget.transform.position);
                    enemyManager.enemyRigidBody.linearVelocity = targetVelocity;
                    //ENEMYMANAGER.transform!!!
                    enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, enemyManager.navmeshAgent.transform.rotation, enemyManager.rotationSpeed / 10000 / Time.deltaTime);

                }
                
            }
            
        }
    }
}