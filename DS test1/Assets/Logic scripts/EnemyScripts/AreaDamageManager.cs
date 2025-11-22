using UnityEngine;

namespace JA {
    public class AreaDamageManager : MonoBehaviour, AnimationEventReceiver
    {
        [SerializeField] int damage = 10;
        [SerializeField] int radius = 2;
        private SphereCollider colliderSphere;
        [SerializeField] bool isColliderActive = false;
        [SerializeField] Collider areaCollider;

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