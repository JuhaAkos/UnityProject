using UnityEngine;

namespace JA
{
    public class AttackState : State
    {
        public CombatStanceState combatStanceState;
        public DeadState deadState;

        public EnemyAttackAction[] enemyAttacks;
        public EnemyAttackAction currentAttack;

        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorHandler enemyAnimatorHandler)
        {
            if (enemyStats.isDead)
            {
                return deadState;
            }

            enemyManager.navmeshAgent.transform.localPosition = Vector3.zero;
            enemyManager.navmeshAgent.transform.localRotation = Quaternion.identity;

            Vector3 targetDirection = enemyManager.currentTarget.transform.position - transform.position;
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);
            //float viewableAngle = Vector3.Angle(targetDirection, transform.forward);
            
            float viewableAngle = Vector3.SignedAngle(transform.forward, targetDirection, Vector3.up);
            if (viewableAngle < 0)
            {
                viewableAngle += 360;
            }
            

            enemyAnimatorHandler.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            enemyAnimatorHandler.anim.SetFloat("Horizontal", 0, 0.1f, Time.deltaTime);

            HandleRotateTowardsTarget(enemyManager);

            if (enemyManager.isPerformingAction)
            {
                return combatStanceState;
            }

            //Debug.Log("angle " + viewableAngle);
            //Debug.Log("NEW angle " + Vector3.SignedAngle(targetDirection, transform.forward, Vector3.up));
            //Debug.Log("NEW angle " + Vector3.SignedAngle(transform.forward, targetDirection, Vector3.up));
            if (currentAttack != null)
            {
                //if too close for CURRENT ATTACK-> new attack
                if (distanceFromTarget < currentAttack.minimumDistanceNeededToAttack)
                {
                    return this;
                }
                else if (distanceFromTarget < currentAttack.maximumDistanceNeededToAttack)
                {

                    if (currentAttack.maximumAttackAngle < currentAttack.minimumAttackAngle &&
                        (viewableAngle >= currentAttack.minimumAttackAngle || viewableAngle <= currentAttack.maximumAttackAngle)
                        || viewableAngle <= currentAttack.maximumAttackAngle && viewableAngle >= currentAttack.minimumAttackAngle
                        )
                    {
                            if (enemyManager.currentRecoveryTime <= 0 && enemyManager.isPerformingAction == false)
                            {
                                enemyAnimatorHandler.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                                enemyAnimatorHandler.anim.SetFloat("Horizontal", 0, 0.1f, Time.deltaTime);
                                //Debug.Log("start: " + currentAttack.minimumAttackAngle + " end: " + currentAttack.maximumAttackAngle);
                                enemyAnimatorHandler.PlayTargetAnimation(currentAttack.actionAnimation, true);
                                enemyManager.isPerformingAction = true;
                                enemyManager.currentRecoveryTime = currentAttack.recoveryTime;
                                currentAttack = null;
                                return combatStanceState;
                            }
                    }
                }
                currentAttack = null;
            }
            else if (!enemyManager.isPerformingAction && !enemyManager.isOnAttackTimeOut)
            {   
                enemyManager.navmeshAgent.transform.localPosition = Vector3.zero;
                //enemyManager.navmeshAgent.transform.localRotation = Quaternion.identity;
                //Debug.Log("performing: " + enemyManager.isPerformingAction);
                GetNewAttack(enemyManager);
            }

            return combatStanceState;
            

            //select attack
            //if selected attack not usable (bad angle/distance)
            //->select new
            //if usable -> stop + attack
            //recovery timer
            //back to combat stance
        }

        private void GetNewAttack(EnemyManager enemyManager)
        {
            Vector3 targetsDirection = enemyManager.currentTarget.transform.position - transform.position;
            //float viewableAngle = Vector3.Angle(targetsDirection, transform.forward);
            float viewableAngle = Vector3.SignedAngle(transform.forward, targetsDirection, Vector3.up);
            if (viewableAngle < 0)
            {
                viewableAngle += 360;
            }

            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, transform.position);

            int maxScore = 8;

            for (int i = 0; i < enemyAttacks.Length; i++)
            {
                EnemyAttackAction enemyAttackAction = enemyAttacks[i];

                if (distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack
                && distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack)
                {
                    //check if attack is directional or frontal
                    //frontal: 360 < x < 180...
                    if (enemyAttackAction.maximumAttackAngle < enemyAttackAction.minimumAttackAngle)
                    {
                        if (viewableAngle >= enemyAttackAction.minimumAttackAngle ||
                         viewableAngle <= enemyAttackAction.maximumAttackAngle)
                        {
                            maxScore += enemyAttackAction.attackScore;
                        }
                    //directional outcome
                    //does not do a full circle
                    } else
                    {
                        if (viewableAngle <= enemyAttackAction.maximumAttackAngle
                        && viewableAngle >= enemyAttackAction.minimumAttackAngle)
                        {
                            maxScore += enemyAttackAction.attackScore;
                        }
                    }
                    
                }
            }

            int randomValue = Random.Range(0, maxScore);
            int temporaryScore = 0;

            for (int i = 0; i < enemyAttacks.Length; i++)
            {
                EnemyAttackAction enemyAttackAction = enemyAttacks[i];

                if (distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack
                && distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack)
                {
                    //non directional/frontal
                    if (enemyAttackAction.maximumAttackAngle < enemyAttackAction.minimumAttackAngle)
                    {
                        if (viewableAngle >= enemyAttackAction.minimumAttackAngle ||
                         viewableAngle <= enemyAttackAction.maximumAttackAngle)
                        {
                            if (currentAttack != null)
                            {
                                HandleRotateTowardsTarget(enemyManager);
                                return;
                            }

                            temporaryScore += enemyAttackAction.attackScore;

                            //Debug.Log("Temp score: " + temporaryScore + " vs maxScore: " + maxScore + " = " + randomValue);
                            if (temporaryScore > randomValue)
                            {
                                currentAttack = enemyAttackAction;
                            }
                        }
                    }
                    //directional
                    else
                    {
                        if (viewableAngle <= enemyAttackAction.maximumAttackAngle
                        && viewableAngle >= enemyAttackAction.minimumAttackAngle)
                        {
                            if (currentAttack != null)
                            {
                                HandleRotateTowardsTarget(enemyManager);
                                return;
                            }

                            temporaryScore += enemyAttackAction.attackScore;

                            if (temporaryScore > randomValue)
                            {
                                currentAttack = enemyAttackAction;
                            }
                        }
                    }

                }
            }
            
            if (currentAttack == null)
            {
                //Debug.Log("Nem találtam!!!");
                enemyManager.timeOutStarted = true;
                HandleRotateTowardsTarget(enemyManager);
                return;
            }

        }
    
        private void HandleRotateTowardsTarget(EnemyManager enemyManager)
        {
            enemyManager.navmeshAgent.transform.localPosition = Vector3.zero;
            enemyManager.navmeshAgent.transform.localRotation = Quaternion.identity;

            Vector3 targetDirection = enemyManager.currentTarget.transform.position - transform.position;
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);
            float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

            //manual rotate
            if (false)
            {       
                Vector3 direction = enemyManager.currentTarget.transform.position - transform.position;
                direction.y = 0;
                direction.Normalize();

                if (direction == Vector3.zero)
                {
                    direction = transform.forward;
                }

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                enemyManager.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, enemyManager.rotationSpeed * Time.deltaTime / 24);             
            }
            
            //navmash rotate if no action
            else
            {
                //Debug.Log("Rotate: attack, NO perform");   
                Vector3 relativeDirection = transform.InverseTransformDirection(enemyManager.navmeshAgent.desiredVelocity);

                distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);
                //if (distanceFromTarget > enemyManager.maximumAttackRange)
                {
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
                    
                    enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, enemyManager.navmeshAgent.transform.rotation, enemyManager.rotationSpeed / 1 / Time.deltaTime);
                    //Debug.Log("Slerp: " + enemyManager.rotationSpeed / 10000 / Time.deltaTime);

                }
            }
            
        }
    }
}