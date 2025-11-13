using UnityEngine;

namespace JA {
    public class DeadState : State
    {
        
        private void Awake()
        {     
        } 


        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorHandler enemyAnimatorHandler)
        {
            return this;
        }
    }
}