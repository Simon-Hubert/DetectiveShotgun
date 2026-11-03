using UnityEngine;

namespace DShotgun
{
    public static class ShotgunManager
    {
        private static bool _isLoaded = false;
        private static ItemData _loadedItem;
        
        public static bool IsLoaded => _isLoaded;
        public static ItemData LoadedItem => _loadedItem;

        public static void LoadItem(ItemData item)
        {
            _isLoaded = true;
            _loadedItem = item;
            Debug.Log($"[ShotgunManager] Loaded item: {item.Name}");
        }
        
        public static void UnloadItem()
        {
            if(!_isLoaded) return;
            _isLoaded = false;
            _loadedItem = new ItemData();
            Debug.Log("[ShotgunManager] Unloaded item");
        }
    }
}
