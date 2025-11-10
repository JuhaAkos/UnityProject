using UnityEngine;
using UnityEngine.AI;

namespace JA {
    public class EnemyManager : CharacterManager
    {
        EnemyLocomotionManager enemyLocomotionManager;
        EnemyAnimatorHandler enemyAnimatorHandler;
        EnemyStats enemyStats;

        public NavMeshAgent navmeshAgent;
        public State currentState;
        public CharacterStats currentTarget;
        public bool isPerformingAction;
        public bool isInteracting;       
        public float rotationSpeed = 25;
        public float maximumAttackRange = 3.5f;

        /*
        public EnemyAttackAction[] enemyAttacks;
        public EnemyAttackAction currentAttack;
        */

        public float detectionRadius = 20;
        public float maximumDetectionAngle = 50;
        public float minimumDetectionAngle = -50;

        public float viewableAngle;

        public float currentRecoveryTime = 0;

        public Rigidbody enemyRigidBody;

        public bool isOnAttackTimeOut;
        public bool timeOutStarted;
        public float attackTimeOutTimer = 0;


        private void Awake()
        {
            enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();
            enemyAnimatorHandler = GetComponentInChildren<EnemyAnimatorHandler>();
            enemyStats = GetComponent<EnemyStats>();
            navmeshAgent = GetComponentInChildren<NavMeshAgent>();
            navmeshAgent.enabled = false;

            enemyRigidBody = GetComponent<Rigidbody>();
        }
        
        private void Start()
        {
            //navmeshAgent.enabled = false;
            enemyRigidBody.isKinematic = false;
        }

        private void Update()
        {
            HandleRecoveryTime();
            StartTimeOut();
            HandleTimeOutTime();


            isInteracting = enemyAnimatorHandler.anim.GetBool("isInteracting");
        }

        private void FixedUpdate()
        {
            HandleStateMachine();
        }

        private void HandleStateMachine()
        {
            if (currentState != null)
            {
                State nextState = currentState.Tick(this, enemyStats, enemyAnimatorHandler);

                if (nextState != null)
                {
                    SwitchToNextState(nextState);
                }
            }
        }

        private void SwitchToNextState(State state)
        {
            currentState = state;
        }

        private void HandleRecoveryTime()
        {
            if (currentRecoveryTime > 0)
            {
                currentRecoveryTime -= Time.deltaTime;
            }

            if (isPerformingAction)
            {
                if (currentRecoveryTime <= 0)
                {
                    isPerformingAction = false;
                }
            }
        }

        private void StartTimeOut()
        {
            if (timeOutStarted && attackTimeOutTimer <= 0)
            {
                attackTimeOutTimer = Random.Range(1, 4);
                isOnAttackTimeOut = true;
                timeOutStarted = false;
                Debug.Log("On timeout for: " + attackTimeOutTimer); 
            }         
        }

        private void HandleTimeOutTime()
        {
            if (attackTimeOutTimer > 0)
            {
                attackTimeOutTimer -= Time.deltaTime;
            }

            if (isOnAttackTimeOut)
            {
                if (attackTimeOutTimer <= 0)
                {
                    isOnAttackTimeOut = false;
                }
            }
        }

    }
}