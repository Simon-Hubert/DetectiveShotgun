using System;
using UnityEngine;
using UnityEngine.Events;

namespace DShotgun
{
    public class UIInventory : MonoBehaviour
    {
        [SerializeField] private UIItem[] _inventorySlots = new UIItem[5];
        [SerializeField] private PlayerInventory _playerInventory;
        [SerializeField, Tooltip("Animator du nuage de fumée (trigger Pouf) : déplacé sur la poche et rejoué quand un objet y arrive")] private Animator _pouf;
        [Header("Events")]
        [SerializeField] private UnityEvent OnLoadItem;
        [SerializeField] private UnityEvent OnUnloadItem;
            
        private static readonly int POUF_TRIGGER = Animator.StringToHash("Pouf");
        private int _loadedSlot;

        // Icône du slot : PickUpItemSequence y fait tomber l'objet ramassé
        public RectTransform GetSlot(int index) => (RectTransform)_inventorySlots[index].transform;

        private void OnEnable()
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

        private void OnDisable()
        {
            for (int i = 0; i < _inventorySlots.Length; i++)
            {
                if (_inventorySlots[i] != null) _inventorySlots[i].OnDropInShotgun -= LoadItem;
            }
            if (_playerInventory != null) _playerInventory.OnSlotChanged -= UpdateSlot;
            ShotgunManager.OnShotgunUnloaded -= UnloadItem;
        }

        private void LoadItem(int id)
        {
            ShotgunManager.LoadItem(_playerInventory.Inventory[id]);
            _loadedSlot = id;
            OnLoadItem?.Invoke();
        }

        private void UpdateSlot(int i, ItemData itemSlot)
        {
            if (itemSlot == null)
            {
                _inventorySlots[i].ChangeSprite(null);
                return;
            }

            _inventorySlots[i].ChangeSprite(itemSlot.Sprite);

            // Objets de départ : pas de POUF au chargement de la scène
            if (_playerInventory.IsInitialized) PlayPouf(i);
        }

        private void PlayPouf(int slot)
        {
            if (_pouf == null) return;

            RectTransform pocket = GetSlot(slot);
            _pouf.transform.position = pocket.TransformPoint(pocket.rect.center);
            _pouf.SetTrigger(POUF_TRIGGER);
        }

        private void UnloadItem()
        {
            if (_inventorySlots[_loadedSlot] != null) {
                _inventorySlots[_loadedSlot].ResetPosition();
                OnUnloadItem?.Invoke();
            }
        }
    }
}
