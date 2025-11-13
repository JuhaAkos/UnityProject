using UnityEngine;

namespace JA
{
    public class AmbushState : State
    {

        public bool isSleeping;
        float detectionRadius = 10;
        public string wakeAnimation;
        public string sleepAnimation;

        public LayerMask detectionLayer;
        public PursueTargetState pursueTargetState;
        public DeadState deadState;
        public WakeUpState wakeUpState;
        public EnemyStats enemyStats;
        public CharacterStats charactersStats;

        public Vector3 here;

        private void Awake()
        {
            enemyStats = GetComponent<EnemyStats>();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(here, detectionRadius);
        }

        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorHandler enemyAnimatorHandler)
        {
            if (enemyStats.isDead)
            {
                return deadState;
            }

            if (isSleeping)
            {
                enemyAnimatorHandler.PlayTargetAnimation(sleepAnimation, true);
            }

            #region target detection

            Collider[] colliders = Physics.OverlapSphere(enemyManager.transform.position, detectionRadius, detectionLayer);
            here = enemyManager.transform.position;

            for (int i = 0; i < colliders.Length; i++)
            {
                charactersStats = colliders[i].transform.GetComponent<CharacterStats>();

                if (charactersStats != null)
                {
                    Vector3 targetsDirection = charactersStats.transform.position - enemyManager.transform.position;
                    float viewableAngle = Vector3.Angle(targetsDirection, enemyManager.transform.forward);

                    if (viewableAngle > enemyManager.minimumDetectionAngle
                    && viewableAngle < enemyManager.maximumDetectionAngle)
                    {
                        enemyManager.currentTarget = charactersStats;
                        isSleeping = false;
                        enemyAnimatorHandler.PlayTargetAnimation(wakeAnimation, true);
                    }
                }
            }

            #endregion

            #region statechange
            
            if (enemyManager.currentTarget != null)
            {
                enemyStats.enableBossUIFromStats();
                return wakeUpState;

            } else
            {
                return this;
            }
            
            #endregion
        }
    }
}