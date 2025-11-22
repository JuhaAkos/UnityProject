using UnityEngine;

namespace JA {
    public class DamageCollider : MonoBehaviour
    {
        Collider damageCollider;
        AnimatorHandler animatorHandler;
        public int currentWeaponDamage = 25;

        private void Awake()
        {
            damageCollider = GetComponent<Collider>();
            damageCollider.gameObject.SetActive(true);
            damageCollider.isTrigger = true;
            damageCollider.enabled = false;
            animatorHandler = FindFirstObjectByType<AnimatorHandler>();
        }

        public void EnableDamageCollider()
        {
            damageCollider.enabled = true;
        }

        public void DisableDamageCollider()
        {
            damageCollider.enabled = false;
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.tag == "Player")
            {
                PlayerStats playerStats = collision.GetComponent<PlayerStats>();

                if (playerStats != null)
                {
                    playerStats.TakeDamage(currentWeaponDamage);
                }
            }

            if (collision.tag == "Enemy")
            {
                EnemyStats enemyStats = collision.GetComponent<EnemyStats>();

                if (enemyStats != null)
                {
                    if (animatorHandler.anim.GetBool("isHeavyAttackActive"))
                    {
                        enemyStats.TakeDamage(currentWeaponDamage * 2);
                    } else
                    {
                        enemyStats.TakeDamage(currentWeaponDamage);
                    }                    
                    
                }
            }

        }
    }
}