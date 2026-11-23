using System;
using UnityEngine;

namespace DShotgun
{
    public static class ShotgunManager
    {
        private static bool _isLoaded = false;
        private static ItemSlot _loadedItem;
        
        public static bool IsLoaded => _isLoaded;
        public static ItemSlot LoadedItem => _loadedItem;
        
        public static event Action OnShotgunLoaded;
        public static event Action OnShotgunUnloaded;

        public static void LoadItem(ItemSlot item)
        {
            _isLoaded = true;
            _loadedItem = item;
            Debug.Log($"[ShotgunManager] Loaded item: {item.Name}");
            OnShotgunLoaded?.Invoke();
        }
        
        public static void UnloadItem()
        {
            if(!_isLoaded) return;
            _isLoaded = false;
            _loadedItem = new ItemSlot();
            Debug.Log("[ShotgunManager] Unloaded item");
            OnShotgunUnloaded?.Invoke();
        }
    }
}
