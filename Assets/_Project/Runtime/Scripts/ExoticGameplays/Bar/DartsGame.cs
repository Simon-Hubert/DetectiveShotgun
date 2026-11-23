using HierarchySequences;
using UnityEngine;

namespace DShotgun
{
    public class DartsGame : MonoBehaviour
    {
        [Header("Params")]
        [SerializeField, Range(0.1f, 0.6f),
         Tooltip("Deviation max de la flechette (distance en pourcentage du radius de la cible)")]
        private float _maxDeviation = 0.5f;

        [Header("References")] [SerializeField]
        private CircleCollider2D _targetCollider;
        [SerializeField] private DartTrajectoryAnimator _dartAnimator;

        [Header("Sequence")] [SerializeField] private Sequence _winSequence;
        [SerializeField] private Sequence _smallerNumberSequence;
        [SerializeField] private Sequence _greaterNumberSequence;
        [SerializeField] private Sequence _bullSequence;
        [SerializeField] private Sequence _outSequence;

        private Vector2 _hitPoint;

        public void ThrowDart()
        {
            Camera camera = Camera.main;
            if (camera == null) return;

            Vector2 dartThrowPosition = camera.ScreenToWorldPoint(InputReader.PointerPosition);
            Compute(dartThrowPosition);
        }

        private void Compute(Vector2 dartThrowPosition)
        {
            int randomAccuracy = Random.Range(0, 100);

            _hitPoint = ComputeTrajectoire(dartThrowPosition, randomAccuracy);

            if (_dartAnimator != null)
            {
                _dartAnimator.OnEndAnim += Result;
                _dartAnimator.Animate(_hitPoint);
            }
        }

        private void Result()
        {
            _dartAnimator.OnEndAnim -= Result;
            RaycastHit2D hit = Physics2D.Raycast(_hitPoint, Vector2.zero);
            
            if (hit.collider == null)
            {
                _outSequence.Execute();
                return;
            }

            int score = EvaluateScore(_hitPoint);
            if (score == 17)
            {
                _winSequence.Execute();
                return;
            }
            else if (score == 50)
            {
                _bullSequence.Execute();
                return;
            }
            else if (score < 17)
            {
                _smallerNumberSequence.Execute();
            }
            else
            {
                _greaterNumberSequence.Execute();
                return;
            }
        }

        private Vector2 ComputeTrajectoire(Vector2 hitpoint, int accuracy)
        {
            if (_targetCollider == null)
            {
                return hitpoint;
            }

            float normalizedAccuracy = Mathf.Clamp01(accuracy / 100f);
            float maxDeviation = _targetCollider.radius * _maxDeviation;
            float deviation = (1f - normalizedAccuracy) * maxDeviation;

            Vector2 offset = Random.insideUnitCircle.normalized * deviation;
            return hitpoint + offset;
        }

        private int EvaluateScore(Vector2 hitPoint)
        {
            if (_targetCollider == null)
            {
                return 0;
            }

            Vector2 center = _targetCollider.bounds.center;
            float distance = Vector2.Distance(hitPoint, center);
            float radius = _targetCollider.radius;

            if (distance <= radius * 0.13f)
            {
                return 50;
            }

            if (distance <= radius * 0.35f)
            {
                return 17;
            }

            float ratio = Mathf.Clamp01(distance / radius);
            return Mathf.Clamp(Mathf.RoundToInt((1f - ratio) * 20f), 1, 20);
        }
    }
}
