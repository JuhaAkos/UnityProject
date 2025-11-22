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
    }
}    
