using UnityEngine;

namespace JA
{
    [CreateAssetMenu(menuName = "Items/Estus Item")]
    public class EstusItem : Item
    {
        [SerializeField] GameObject currentItemModel;
        [SerializeField] private int healAmount = 60;

        public int GetHealAmount()
        {
            return healAmount;
        }

        
    }
}