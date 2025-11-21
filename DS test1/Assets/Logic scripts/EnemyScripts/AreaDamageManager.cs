using UnityEngine;

namespace JA {
    public class AreaDamageManager : MonoBehaviour, AnimationEventReceiver
    {
        public int damage = 10;
        public int radius = 2;
        private SphereCollider colliderSphere;
        public bool isColliderActive = false;
        public Collider areaCollider;

        private void Awake()
        {
            colliderSphere = GetComponent<SphereCollider>();
            colliderSphere.radius = radius;
            areaCollider.enabled = false;
        }

        public void animEventReceived()
        {
            if (isColliderActive)
            {
                isColliderActive = false;
                areaCollider.enabled = false;
            } else
            {
                isColliderActive = true;
                areaCollider.enabled = true;
            }            
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isColliderActive)
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
}