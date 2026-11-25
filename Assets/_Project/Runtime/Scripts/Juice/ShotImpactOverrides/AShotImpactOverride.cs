using UnityEngine;

namespace DShotgun
{
    public interface IOverrider
    {
        public void Override(GameObject shotImpact);
    }
    
    public abstract class AShotImpactOverride<T> : MonoBehaviour, IOverrider
    {
        [SerializeField] protected T _ref;

        public abstract void Override(GameObject shotImpact);
    }
}
