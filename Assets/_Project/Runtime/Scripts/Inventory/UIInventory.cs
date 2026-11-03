using System;
using UnityEngine;

namespace DShotgun
{
    public class UIInventory : MonoBehaviour
    {
        [SerializeField] private UIItem[] _inventorySlots = new UIItem[5];
        [SerializeField] private PlayerInventory _playerInventory;
        
        private int _loadedSlot;

        private void Start()
        {
            for (int i = 0; i < _inventorySlots.Length; i++)
            {
                _inventorySlots[i].SetId(i);
                _inventorySlots[i].OnDropInShotgun += LoadItem;
            }
            _playerInventory = FindAnyObjectByType<PlayerInventory>();
            _playerInventory.OnSlotChanged += UpdateSlot;
            ShotgunManager.OnShotgunUnloaded += UnloadItem;
        }

        private void LoadItem(int id)
        {
            ShotgunManager.LoadItem(_playerInventory.Inventory[id]);
            _loadedSlot = id;
        }

        private void UpdateSlot(int i, ItemData itemSlot)
        {
            _inventorySlots[i].ChangeSprite(itemSlot?.Sprite);
        }
        
        private void UnloadItem()
        {
            _inventorySlots[_loadedSlot].ResetPosition();
        }
    }
}
