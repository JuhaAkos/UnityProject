using UnityEngine;

namespace JA {
    public class PlayerStats : CharacterStats
    {
        public HealthBar healthbar;
        public StaminaBar staminabar;

        public float staminaRegenerationAmount = 6;
        public float staminaRegenTimer = 0;
        public int estusCount = 3;

        AnimatorHandler animatorHandler;
        PlayerManager playerManager;
        EstusUICounter estusUICounter;
        DeathScreen deathScreen;
        public EstusItem estusItem;

        public void Awake()
        {
            playerManager = GetComponent<PlayerManager>();
            healthbar = FindObjectOfType<HealthBar>();
            staminabar = FindObjectOfType<StaminaBar>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();

            estusUICounter = FindObjectOfType<EstusUICounter>();
            deathScreen = FindObjectOfType<DeathScreen>();
        }


        void Start()
        {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            healthbar.SetMaxHealth(maxHealth);

            maxStamina = SetMaxStaminaFromStaminaLevel();
            currentStamina = maxStamina;
            staminabar.SetMaxStamina(maxStamina);

            estusUICounter.ChangeEstusCounterText(estusCount);
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

                //later call from anim event
                deathScreen.DeathScreenFadeIn();
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
            }
            else
            {
                staminaRegenTimer += Time.deltaTime;
                //+1sec delay
                if (currentStamina < maxStamina && staminaRegenTimer > 2f)
                {
                    currentStamina += staminaRegenerationAmount * Time.deltaTime;
                    staminabar.SetCurrentStamina(Mathf.RoundToInt(currentStamina));
                }
            }
        }
    
        public void UseEstus()
        {
            if (estusCount < 1)
            {
                return;
            }
            if (playerManager.isInteracting)
            {
                return;
            }

            estusCount -= 1;
            //CALL PLAYERINV.SwitchToHealingItem() based on anim event
            animatorHandler.PlayTargetAnimation("Heal", true);
            if (currentHealth + estusItem.GetHealAmount() > maxHealth)
            {
                currentHealth = maxHealth;
            }
            else
            {
                currentHealth = currentHealth + estusItem.GetHealAmount();
            }

            healthbar.SetCurrentHealth(currentHealth);
            estusUICounter.ChangeEstusCounterText(estusCount);
            
        }
    }
}