using System;
using UnityEngine;

namespace DShotgun
{
    public class UIInventory : MonoBehaviour
    {
        [SerializeField] private UIItem[] _inventorySlots = new UIItem[5];

        private void Start()
        {
            for (int i = 0; i < _inventorySlots.Length; i++)
            {
                _inventorySlots[i].SetId(i);
            }
            FindAnyObjectByType<PlayerInventory>().OnSlotChanged += UpdateSlot;
        }

        private void UpdateSlot(int i, ItemData itemSlot)
        {
            _inventorySlots[i].ChangeSprite(itemSlot?.Sprite);
        }
    }
}
