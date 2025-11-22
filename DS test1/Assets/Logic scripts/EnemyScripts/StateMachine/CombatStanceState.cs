using UnityEngine;

namespace JA
{
    public class CombatStanceState : State
    {
        [SerializeField] AttackState attackState;
        [SerializeField] DeadState deadState;
        [SerializeField] PursueTargetState pursueTargetState;

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

            
            /*
            if (enemyManager.isPerformingAction)
            {
                //enemyAnimatorHandler.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                return this;
            }
            */

            //Debug.Log("dist: " + distanceFromTarget + " ,max dist: " + enemyManager.maximumAttackRange);
            if (enemyManager.currentRecoveryTime <= 0 && distanceFromTarget <= enemyManager.maximumAttackRange && !enemyManager.isOnAttackTimeOut)
            {
                return attackState;
            }
            else if (distanceFromTarget > enemyManager.maximumAttackRange && !enemyManager.isPerformingAction)
            {
                return pursueTargetState;
            }
            else
            {
                HandleRotateTowardsTarget(enemyManager, enemyAnimatorHandler);
                //HandleRotateTowardsTarget(enemyManager);
                return this;
            }
        }

        private void HandleRotateTowardsTarget(EnemyManager enemyManager, EnemyAnimatorHandler enemyAnimatorHandler)
        {
            //manual rotate
            Vector3 direction = enemyManager.currentTarget.transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();

            if (direction == Vector3.zero)
            {
                direction = transform.forward;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemyAnimatorHandler.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            enemyManager.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, enemyManager.rotationSpeed * Time.deltaTime / 24);            
        }
    }
}