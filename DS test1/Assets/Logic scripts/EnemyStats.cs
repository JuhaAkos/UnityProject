using UnityEngine;

namespace JA {
    public class EnemyStats : MonoBehaviour
    {
        public int healthLevel = 10;
        public int maxHealth;
        public int currentHealth;

        Animator animator;

        public void Awake()
        {
            animator = GetComponentInChildren<Animator>();
        }


        void Start()
        {
             Debug.Log("Enemy loaded 1");
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;            
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
            currentHealth = currentHealth - damage;            

            animator.Play("Damage_01");

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                //no transition connected to locomotion so won't reset state
                animator.Play("Dead_01");
                //handler death                
            }
        }
    }
}