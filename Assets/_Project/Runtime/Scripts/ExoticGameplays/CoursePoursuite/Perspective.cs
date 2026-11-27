using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace DShotgun
{
    public class Perspective : MonoBehaviour
    {
        [SerializeField, Foldout("Perspective")] public Vector2 P0;
        [SerializeField, Foldout("Perspective")] public Vector2 P1;
        [SerializeField, Foldout("Perspective")] public Vector2 P2;
        [SerializeField, Foldout("Perspective")] public Vector2 P3;

        private void OnDrawGizmosSelected() {
            Gizmos.DrawSphere(P0,0.15f);
            Gizmos.DrawSphere(P1,0.15f);
            Gizmos.DrawSphere(P2,0.15f);
            Gizmos.DrawSphere(P3,0.15f);
            Gizmos.DrawLine(P0,P1);
            Gizmos.DrawLine(P2,P3);
        }

    }
}
