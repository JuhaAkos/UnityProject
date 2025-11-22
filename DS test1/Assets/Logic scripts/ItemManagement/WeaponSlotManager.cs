using UnityEngine;

namespace JA
{
    public class WeaponSlotManager : MonoBehaviour
    {
        PlayerManager playerManager;
        WeaponHolderSlot leftHandSlot;
        WeaponHolderSlot rightHandSlot;

        DamageCollider leftHandDamageCollider;
        DamageCollider rightHandDamageCollider;

        public WeaponItem attackingWeapon;

        Animator animator;
        PlayerStats playerStats;

        private void Awake()
        {
            playerManager = GetComponentInParent<PlayerManager>();
            animator = GetComponent<Animator>();
            playerStats = GetComponentInParent<PlayerStats>();

            WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
            foreach (WeaponHolderSlot weaponSlot in weaponHolderSlots)
            {
                if (weaponSlot.isLeftHandSlot)
                {
                    leftHandSlot = weaponSlot;
                }
                else if (weaponSlot.isRightHandSlot)
                {
                    rightHandSlot = weaponSlot;
                }
            }
        }

        public void LoadWeaponOnSlot(Item item, bool isLeft)
        {
            rightHandSlot.LoadWeaponModel(item);
        }


        public void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft)
        {
            rightHandSlot.LoadWeaponModel(weaponItem);
            LoadWeaponDamageCollider();

            #region Handle weapon idle anim
            if (weaponItem != null)
            {
                animator.CrossFade(weaponItem.right_Hand_Idle, 0.2f);
            }
            else
            {
                animator.CrossFade("Right Arm Empty", 0.2f);
            }
            #endregion
        }

        #region Handle Weapon damage colliders
        //animation events uses these

        private void LoadWeaponDamageCollider()
        {
            //error if empty
            if (rightHandSlot.currentWeaponModel != null)
            {
                rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
            }
        }

        public void OpenDamageCollider()
        {
            rightHandDamageCollider.EnableDamageCollider();
        }

        public void CloseDamageCollider()
        {
            rightHandDamageCollider.DisableDamageCollider();
            //leftHandDamageCollider.DisableDamageCollider();
        }

        #endregion

        #region Handle weapon stamina
        public void DrainStaminaLightAttack()
        {
            playerStats.TakeStaminaDamage(Mathf.RoundToInt(attackingWeapon.baseStamina * attackingWeapon.lightAttackMultiplier));
        }

        public void DrainStaminaHeavyAttack()
        {
            playerStats.TakeStaminaDamage(Mathf.RoundToInt(attackingWeapon.baseStamina * attackingWeapon.heavyAttackMultiplier));
        }
        #endregion
    }
}