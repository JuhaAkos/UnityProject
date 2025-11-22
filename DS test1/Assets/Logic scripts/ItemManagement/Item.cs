using UnityEngine;

namespace JA
{
    [CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
    public class Item : ScriptableObject
    {
        [Header("Item Information")]
        public Sprite itemIcon;
        [SerializeField] string itemName;
        //can be empty
        public GameObject modelPrefab;        
    }
}
