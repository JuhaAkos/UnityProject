using UnityEngine;

namespace JA
{
    [CreateAssetMenu(menuName = "Items/Weapon Item")]
    public class WeaponItem : Item
    {
        
        public bool isUnarmed;

        [Header("Idle Animations")]
        public string right_Hand_Idle;
        public string left_Hand_Idle;


        [Header("One Handed Attack Animations")]
        public string OH_Light_Attack_1;
        public string OH_Light_Attack_2;
        public string OH_Light_Attack_3;
        public string OH_Heavy_Attack_1;
        public string OH_Heavy_Attack_2;
        //OH as in one-handed

        [Header("Stamina Costs")]
        public int baseStamina;
        public float lightAttackMultiplier;
        public float heavyAttackMultiplier;
    }
}