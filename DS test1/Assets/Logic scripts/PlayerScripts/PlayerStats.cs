using UnityEngine;

namespace JA {
    public class PlayerStats : CharacterStats
    {
        public HealthBar healthbar;
        public StaminaBar staminabar;

        public float staminaRegenerationAmount = 6;
        public float staminaRegenTimer = 0;

        AnimatorHandler animatorHandler;
        PlayerManager playerManager;

        public void Awake()
        {
            playerManager = GetComponent<PlayerManager>();
            healthbar = FindObjectOfType<HealthBar>();
            staminabar = FindObjectOfType<StaminaBar>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
        }


        void Start()
        {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            healthbar.SetMaxHealth(maxHealth);

            maxStamina = SetMaxStaminaFromStaminaLevel();
            currentStamina = maxStamina;
            staminabar.SetMaxStamina(maxStamina);
        }

        //health stats =/= health points
        //100vigor in ER =/= 100 hp
        private int SetMaxHealthFromHealthLevel()
        {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        private float SetMaxStaminaFromStaminaLevel()
        {
            maxStamina = staminaLevel * 10;
            return maxStamina;
        }

        public void TakeDamage(int damage)
        {
            if (playerManager.isInvulnerable)
            {
                return;
            }
            
            if (isDead)
            {
                return;
            }
            currentHealth = currentHealth - damage;

            healthbar.SetCurrentHealth(currentHealth);

            animatorHandler.PlayTargetAnimation("Damage_01", true);

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                //no transition connected to locomotion so won't reset state
                animatorHandler.PlayTargetAnimation("Dead_01", true);

                isDead = true;              
            }
        }

        public void TakeStaminaDamage(int damage)
        {
            currentStamina = currentStamina - damage;
            staminabar.SetCurrentStamina(currentStamina);
        }

        public void RegenrateStamina()
        {
            if (playerManager.isInteracting)
            {
                staminaRegenTimer = 0;
            } else {
                staminaRegenTimer += Time.deltaTime;
                //+1sec delay
                if (currentStamina < maxStamina && staminaRegenTimer > 2f)
                {
                    currentStamina += staminaRegenerationAmount * Time.deltaTime;
                    staminabar.SetCurrentStamina(Mathf.RoundToInt(currentStamina));
                }
            }            
        }
    }
}