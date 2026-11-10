using UnityEngine;

namespace DShotgun
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "DetectiveShotgun/ItemData")]
    public class ItemData : ScriptableObject
    {
        public Sprite Sprite;
        public string Name;
        public string Description;
    }
}
