using UnityEngine;

namespace JA
{
    public class AmbushState : State
    {

        public bool isSleeping;
        public float detectionRadius = 2;
        public string wakeAnimation;
        public string sleepAnimation;

        public LayerMask detectionLayer;
        public PursueTargetState pursueTargetState;
        public DeadState deadState;
        public EnemyStats enemyStats;

        private void Awake()
        {
            enemyStats = GetComponent<EnemyStats>();
        }

        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorHandler enemyAnimatorHandler)
        {
            if (enemyStats.isDead)
            {
                return deadState;
            }

            if (isSleeping && enemyManager.isInteracting == false)
            {
                enemyAnimatorHandler.PlayTargetAnimation(sleepAnimation, true);
            }

            #region target detection

            Collider[] colliders = Physics.OverlapSphere(enemyManager.transform.position, detectionRadius, detectionLayer);

            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterStats charactersStats = colliders[i].transform.GetComponent<CharacterStats>();

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
                return pursueTargetState;
            } else
            {
                return this;
            }
            
            #endregion
        }
    }
}