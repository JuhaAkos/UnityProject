using UnityEngine;

namespace JA {
    public class DamagePlayer : MonoBehaviour
    {
        public int damage = 25;

        private void OnTriggerEnter(Collider other)
        {
            //other as in the other thing that entered the radius
            PlayerStats playerStats = other.GetComponent<PlayerStats>();

            //if the object that entered the radius has the playerstats scripts do this
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
            }
        }
    }
}