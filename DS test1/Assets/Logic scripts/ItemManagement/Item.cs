using UnityEngine;

namespace JA
{
    [CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
    public class Item : ScriptableObject
    {
        [Header("Item Information")]
        public Sprite itemIcon;
        public string itemName;
        //can be empty
        public GameObject modelPrefab;        
    }
}
