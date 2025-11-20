using UnityEngine;

namespace JA
{
    public class PursueTargetState : State
    {
        public CombatStanceState combatStanceState;
        public DeadState deadState;

        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorHandler enemyAnimatorHandler)
        {
            if (enemyStats.isDead)
            {
                return deadState;
            }
            if (enemyManager.isPerformingAction)
            {
                enemyAnimatorHandler.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            }

            Vector3 targetDirection = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);
            //Debug.Log("dist: " + distanceFromTarget);
            float viewableAngle = Vector3.Angle(targetDirection, enemyManager.transform.forward);

            HandleRotateTowardsTarget(enemyManager);          

            if (distanceFromTarget > enemyManager.maximumAttackRange)
            {
                enemyAnimatorHandler.anim.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);

                /*
                //atmen fix - no movement without it
                targetDirection.Normalize();
                targetDirection.y = 0;
                float speed = 3;
                targetDirection *= speed;

                Vector3 projectedVelocity = Vector3.ProjectOnPlane(targetDirection, Vector3.up);

                enemyManager.enemyRigidBody.linearVelocity = projectedVelocity;
                //maybe caused by root motion / lack thereof   
                */
            }

            

            enemyManager.navmeshAgent.transform.localPosition = Vector3.zero;
            enemyManager.navmeshAgent.transform.localRotation = Quaternion.identity;
            //chase target
            //close enough? -> switch to attack stance

            if (distanceFromTarget <= enemyManager.maximumAttackRange)
            {
                return combatStanceState;
            } else
            {
                return this;
            }
            return this;
        }

                
        private void HandleRotateTowardsTarget(EnemyManager enemyManager)
        {
            //manual rotate
            if (enemyManager.isPerformingAction)
            {
                Vector3 direction = enemyManager.currentTarget.transform.position - transform.position;
                direction.y = 0;
                direction.Normalize();

                if (direction == Vector3.zero)
                {
                    direction = transform.forward;
                }

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                enemyManager.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, enemyManager.rotationSpeed / Time.deltaTime);
            }
            //navmash rotate if no action
            else
            {
                Vector3 relativeDirection = transform.InverseTransformDirection(enemyManager.navmeshAgent.desiredVelocity);

                float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);

                //minus needed for change to comatstance
                if (distanceFromTarget > enemyManager.maximumAttackRange - 1)
                {

                    Vector3 direction = enemyManager.currentTarget.transform.position - transform.position;

                    direction.Normalize();
                    direction.y = 0;
                    float speed = 4f;

                    direction *= speed;

                    Vector3 projectedVelocity = Vector3.ProjectOnPlane(direction, Vector3.up);

                    Vector3 targetVelocity = projectedVelocity; // Everything in the IF statement from this line and above this line is new                   

                    //Vector3 targetVelocity = enemyManager.enemyRigidBody.linearVelocity;

                    enemyManager.navmeshAgent.enabled = true;
                    enemyManager.navmeshAgent.SetDestination(enemyManager.currentTarget.transform.position);
                    enemyManager.enemyRigidBody.linearVelocity = targetVelocity;
                    //ENEMYMANAGER.transform!!!
                    enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, enemyManager.navmeshAgent.transform.rotation, enemyManager.rotationSpeed / Time.deltaTime);

                }
                else if (distanceFromTarget < enemyManager.maximumAttackRange)
                {
                    Vector3 targetVelocity = enemyManager.enemyRigidBody.linearVelocity;

                    enemyManager.navmeshAgent.enabled = true;
                    enemyManager.navmeshAgent.SetDestination(enemyManager.currentTarget.transform.position);
                    enemyManager.enemyRigidBody.linearVelocity = targetVelocity;
                    //ENEMYMANAGER.transform!!!
                    enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, enemyManager.navmeshAgent.transform.rotation, enemyManager.rotationSpeed / Time.deltaTime);
                }
            }
            
        }
    }
}