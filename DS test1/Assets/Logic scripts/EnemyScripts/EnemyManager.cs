using UnityEngine;
using UnityEngine.AI;

namespace JA {
    public class EnemyManager : CharacterManager
    {
        EnemyLocomotionManager enemyLocomotionManager;
        EnemyAnimatorHandler enemyAnimatorHandler;
        EnemyStats enemyStats;

        [SerializeField] Vector3 lookAngle;

        public  NavMeshAgent navmeshAgent;
        [SerializeField] State currentState;
        public  CharacterStats currentTarget;
        public bool isPerformingAction;
        public bool isInteracting;       
        public float rotationSpeed = 25;
        public float maximumAttackRange = 3.5f;

        /*
        public EnemyAttackAction[] enemyAttacks;
        public EnemyAttackAction currentAttack;
        */

        public  float maximumDetectionAngle = 50;
        public  float minimumDetectionAngle = -50;

        [SerializeField] float viewableAngle;

        public  float currentRecoveryTime = 0;

        public Rigidbody enemyRigidBody;

        #region attackstate timeout
        public bool isOnAttackTimeOut;
        public bool timeOutStarted;
        [SerializeField] float attackTimeOutTimer = 0;
        [SerializeField] bool tooMuchTimeOut;
        [SerializeField] float tooMuchTimeOutTimer = 0;
        #endregion

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

            lookAngle = this.transform.forward;

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

        #region attackstate timeout
        private void StartTimeOut()
        {
            if (timeOutStarted && attackTimeOutTimer <= 0 && tooMuchTimeOutTimer <= 0)
            {
                tooMuchTimeOut = true;
                attackTimeOutTimer = Random.Range(2, 5);
                isOnAttackTimeOut = true;
                
                Debug.Log("On timeout for: " + attackTimeOutTimer); 
            }   
            timeOutStarted = false;      
        }

        private void HandleTimeOutTime()
        {
            if (tooMuchTimeOut)
            {
                tooMuchTimeOutTimer = Random.Range(0, 5);
            }

            if (tooMuchTimeOutTimer >= 0)
            {
                tooMuchTimeOutTimer -= Time.deltaTime;
            }

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
        #endregion

    }
}