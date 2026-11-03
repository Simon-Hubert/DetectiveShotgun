using System;
using UnityEngine;

namespace DShotgun
{
    public class ItemSlot
    {
        public Sprite Sprite;
        public string Name;
        public string Description;

        public ItemSlot()
        {
            Clear();
        }
        public ItemSlot(ItemData item)
        {
            Sprite = item.Sprite;
            Name = item.Name;
            Description = item.Description;
        }

        public void Clear()
        {
            Sprite = null;
            Name = null;
            Description = null;
        }
    }
    
    public class PlayerInventory : MonoBehaviour
    {
        private int _maxCapacity = 5;

        private ItemSlot[] _inventory;
        
        public ItemSlot[] Inventory => _inventory;
        
        public event Action<int, ItemData> OnSlotChanged;
        public event Action OnInventoryFull;

        private void Initialize(int capacity)
        {
            _inventory = new ItemSlot[Mathf.Max(1, capacity)];
            Debug.Log($"[Storage] {name} initialized with {capacity} slots.");
        }
        private void Start()
        {
            Initialize(_maxCapacity);
        }

        public bool IsValidIndex(int index) => _inventory != null && index >= 0 && index < _inventory.Length;
        
        public void AddItem(ItemData item)
        {
            for (int i = 0; i < _inventory.Length; i++)
            {
                if (_inventory[i].Name == null)
                {
                    _inventory[i] = new ItemSlot(item);
                    Debug.Log($"[PlayerInventory] Added {item.Name} to slot {i}");
                    OnSlotChanged?.Invoke(i, item);
                    return;
                }
            }
            Debug.LogWarning("[PlayerInventory] Inventory is full. Cannot add item: " + item.Name);
            OnInventoryFull?.Invoke();
        }
        
        public bool Remove(int index)
        {
            if (!IsValidIndex(index)) return false;
            
            _inventory[index].Clear();

            OnSlotChanged?.Invoke(index, null);
            return true;
        }
        
        public void Clear()
        {
            for (int i = 0; i < _inventory.Length; i++)
            {
                if (_inventory[i].Name == null) continue;
                _inventory[i] = new ItemSlot();
                OnSlotChanged?.Invoke(i, null);
            }
        }
    }
}
