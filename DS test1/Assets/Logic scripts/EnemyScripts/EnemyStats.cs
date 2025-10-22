using UnityEngine;

namespace JA {
    public class EnemyStats : CharacterStats
    {
        Animator animator;
        public BossHealthBar bossHealthbar;

        public void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            bossHealthbar = FindObjectOfType<BossHealthBar>();
        }


        void Start()
        {
            //Debug.Log("Enemy loaded 1");
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            //Debug.Log("Bar: " + bossHealthbar);
            bossHealthbar.SetBossMaxHealth(maxHealth);
            bossHealthbar.DisableBossUI();
        }
        
        public void enableBossUIFromStats()
        {
            bossHealthbar.EnableBossUI();
        }

        //health stats =/= health points
        //100vigor in ER =/= 100 hp
        private int SetMaxHealthFromHealthLevel()
        {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        public void TakeDamage(int damage)
        {
            bossHealthbar.EnableBossUI();

            if (isDead)
            {
                return;
            }
            currentHealth = currentHealth - damage;

            bossHealthbar.SetBossCurrentHealth(currentHealth);                  

            animator.Play("Damage_01");

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                //no transition connected to locomotion so won't reset state
                animator.Play("Dead_01");
                //handler death  
                isDead = true;              
            }
        }
    }
}