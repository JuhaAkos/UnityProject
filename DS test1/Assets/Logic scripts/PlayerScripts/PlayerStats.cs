using UnityEngine;

namespace JA {
    public class PlayerStats : CharacterStats
    {
        [SerializeField] private HealthBar healthbar;
        [SerializeField] private StaminaBar staminabar;

        [SerializeField] private float staminaRegenerationAmount = 25;
        [SerializeField] private float staminaRegenTimer = 0;
        [SerializeField] private int estusCount = 3;

        AnimatorHandler animatorHandler;
        PlayerManager playerManager;
        EstusUICounter estusUICounter;
        DeathScreen deathScreen;
        EstusItem estusItem; 
        EstusHandler estusHandler;    

        public void Awake()
        {
            playerManager = GetComponent<PlayerManager>();
            healthbar = FindFirstObjectByType<HealthBar>();
            staminabar = FindFirstObjectByType<StaminaBar>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();

            estusUICounter = FindFirstObjectByType<EstusUICounter>();
            deathScreen = FindFirstObjectByType<DeathScreen>();
            estusHandler = GetComponent<EstusHandler>();
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
                if (currentStamina < maxStamina && staminaRegenTimer > 1.5f)
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
            estusHandler.SetLightActive(true);
            if (currentHealth + estusHandler.GetHealAmount() > maxHealth)
            {
                currentHealth = maxHealth;
            }
            else
            {
                currentHealth = currentHealth + estusHandler.GetHealAmount();
            }

            healthbar.SetCurrentHealth(currentHealth);
            estusUICounter.ChangeEstusCounterText(estusCount);
            if (estusCount < 1){
                estusUICounter.ChangeCounterIconColor();
            }
        
        }
    
        
    }
}