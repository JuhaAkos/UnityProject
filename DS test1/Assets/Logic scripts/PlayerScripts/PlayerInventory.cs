using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace JA
{
    public class PlayerInventory : MonoBehaviour
    {
        WeaponSlotManager weaponSlotManager;

        public WeaponItem rightWeapon;

        [SerializeField] WeaponItem[] weaponsInRightHandSlots = new WeaponItem[1];
        [SerializeField] EstusItem estusFlask;

        [SerializeField] int currentRightWeaponIndex = 0;

        public List<WeaponItem> weaponInventory;

        private void Awake()
        {
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        }

        private void Start()
        {
            rightWeapon = weaponsInRightHandSlots[0];      

            weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
        }

        public void SwitchToHealingItem()
        {
           weaponSlotManager.LoadWeaponOnSlot(estusFlask, false);
        }
        
        public void SwitchBackFromHealingItem()
        {
            weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
        }

        public void ChangeRightWeapon()
        {
            currentRightWeaponIndex = currentRightWeaponIndex + 1;

            if (currentRightWeaponIndex > weaponsInRightHandSlots.Length - 1)
            {
                currentRightWeaponIndex = 0;
                rightWeapon = weaponsInRightHandSlots[0];  
                weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
            }
            else if (weaponsInRightHandSlots[currentRightWeaponIndex] != null)
            {
                rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
                weaponSlotManager.LoadWeaponOnSlot(weaponsInRightHandSlots[currentRightWeaponIndex], false);
            }
            else
            {
                currentRightWeaponIndex = currentRightWeaponIndex + 1;
            }

        }

    }
}
