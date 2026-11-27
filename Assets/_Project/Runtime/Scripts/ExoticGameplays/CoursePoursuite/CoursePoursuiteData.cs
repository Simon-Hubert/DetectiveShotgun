using UnityEngine;

namespace DShotgun
{
    [CreateAssetMenu(fileName = "CoursePoursuiteData", menuName = "DetectiveShotgun/CoursePoursuiteData")]
    public class CoursePoursuiteData : ScriptableObject
    {
        [field: SerializeField] public float BackToFrontDuration { get; private set; }
        [field: SerializeField] public AnimationCurve BackToFrontCurve { get; private set; }
    }
}
