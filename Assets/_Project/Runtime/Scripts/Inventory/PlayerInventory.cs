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
            _inventory = new ItemSlot[capacity];
            for(int i = 0; i < capacity; i++)
            {
                _inventory[i] = new ItemSlot();
            }
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
                if (string.IsNullOrEmpty(_inventory[i]?.Name))
                {
                    _inventory[i] = new ItemSlot(item);
                    OnSlotChanged?.Invoke(i, item);
                    Debug.Log($"[Storage] Added {item.Name} to slot {i}.");
                    return;
                }
            }
            Debug.LogWarning("[PlayerInventory] Inventory is full. Cannot add item: " + item.Name);
            OnInventoryFull?.Invoke();
        }
        
        public void ConsumeItem(ItemSlot item)
        {
            for (int i = 0; i < _inventory.Length; i++)
            {
                if (_inventory[i].Name == item.Name)
                {
                    _inventory[i] = new ItemSlot();
                    OnSlotChanged?.Invoke(i, null);
                    Debug.Log($"[Storage] Consumed {item.Name} from slot {i}.");
                    return;
                }
            }
            Debug.LogWarning("[PlayerInventory] Item not found in inventory: " + item.Name);
        }

        public void ConsumeItemLoaded()
        {
            ConsumeItem(ShotgunManager.LoadedItem);
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
