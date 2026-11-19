using System;
using HierarchySequences;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace DShotgun
{
    /// <summary>
    /// Sequence pour pick up un item via le shotgun, l'ajouter a l'inventaire et jouer les animations
    /// </summary>
    public class PickUpItemSequence : ASequencable
    {
        [SerializeField] private ItemData _item;

        [Header("Trajectoire (parabole)")]
        [SerializeField] private float _jumpHeight = 2.5f;
        [SerializeField] private float _gravity = 30f;

        [Header("Rotation et taille")]
        [SerializeField, Tooltip("Nombre de tours")] private int _spinTurns = 6;
        [SerializeField, Tooltip("Avancée de la rotation sur la durée du vol : 0 = départ, 1 = tous les tours faits")]
        private AnimationCurve _spin = new AnimationCurve(new Keyframe(0f, 0f, 2f, 2f), new Keyframe(1f, 1f, 0.4f, 0f));
        [SerializeField, Tooltip("Passage de la taille de l'objet (0) à la taille du slot (1) sur la durée du vol")] //a voir si on garde
        private AnimationCurve _size = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.6f, 0.15f, 0.6f, 0.6f), new Keyframe(1f, 1f, 3f, 0f));

        public override bool RequirementsValidated() => _item != null; //merci simon

        protected override string GetName()
        {
            if (_item == null) return "Pick Up";
            return "Pick Up " + _item.Name;
        }

        protected override async Awaitable OnPlay()
        {
            PlayerInventory inventory = FindAnyObjectByType<PlayerInventory>();
            UIInventory ui = FindAnyObjectByType<UIInventory>();
            InteractableManager source = GetComponentInParent<InteractableManager>(true);
            int slot = inventory.FirstFreeSlot();

            // Inventaire plein, pas d'UI ou pas d'objet de départ : ajout classique
            if (slot < 0 || ui == null || source == null)
            {
                inventory.AddItem(_item);
                return;
            }

            Camera worldCamera = Camera.main;
            RectTransform pocket = ui.GetSlot(slot);
            float canvasScale = pocket.GetComponentInParent<Canvas>().rootCanvas.scaleFactor;

            Bounds bounds = source.GetComponent<Collider2D>().bounds;
            Vector2 start = worldCamera.WorldToScreenPoint(bounds.center);
            Vector2 target = pocket.position;
            Vector2 startSize = (worldCamera.WorldToScreenPoint(bounds.max) - worldCamera.WorldToScreenPoint(bounds.min)) / canvasScale;
            float pixelsPerUnit = worldCamera.WorldToScreenPoint(bounds.center + Vector3.up).y - start.y;

            float gravity = _gravity * pixelsPerUnit;
            float apex = Mathf.Max(start.y + _jumpHeight * pixelsPerUnit, target.y);
            float riseSpeed = Mathf.Sqrt(2f * gravity * (apex - start.y));
            float duration = riseSpeed / gravity + Mathf.Sqrt(2f * (apex - target.y) / gravity);
            float sideSpeed = (target.x - start.x) / duration;

            Image pocketImage = pocket.GetComponent<Image>();
            Image flying = new GameObject(source.name + " (ramassage)", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
            flying.rectTransform.SetParent(pocket.parent, false);
            flying.rectTransform.pivot = pocket.pivot;
            flying.sprite = _item.Sprite;
            SpriteRenderer visual = source.GetComponentInChildren<SpriteRenderer>();
            if (visual != null && visual.sprite != null) flying.sprite = visual.sprite;
            flying.color = pocketImage.color;
            flying.preserveAspect = pocketImage.preserveAspect;
            flying.raycastTarget = false;

            source.gameObject.SetActive(false);
            float spinDirection = 1f;
            if (Random.value < 0.5f) spinDirection = -1f;
            try
            {
                for (float time = 0f; time < duration; time += Mathf.Min(Time.deltaTime, 1f / 30f))
                {
                    float t = time / duration;
                    flying.rectTransform.position = new Vector2(start.x + sideSpeed * time, start.y + riseSpeed * time - 0.5f * gravity * time * time);
                    flying.rectTransform.rotation = Quaternion.Euler(0f, 0f, spinDirection * _spin.Evaluate(t) * _spinTurns * 360f);
                    flying.rectTransform.sizeDelta = Vector2.LerpUnclamped(startSize, pocket.rect.size, _size.Evaluate(t));
                    await Awaitable.NextFrameAsync(destroyCancellationToken);
                }

                inventory.AddItem(_item);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                Destroy(flying.gameObject);
            }
        }
    }
}
