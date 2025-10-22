using UnityEngine;

namespace JA
{
    [CreateAssetMenu(menuName = "Items/Estus Item")]
    public class EstusItem : Item
    {
        public GameObject currentItemModel;
        private int healAmount = 20;

        public int GetHealAmount()
        {
            return healAmount;
        }
    }
}