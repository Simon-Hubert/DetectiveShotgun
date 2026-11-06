using UnityEngine;

namespace DShotgun
{
    public abstract class AView : MonoBehaviour
    {
        public abstract CameraConfiguration GetConfiguration();

        private void OnDrawGizmosSelected() {
            GetConfiguration().DrawGizmo(Color.green);
        }
    }
}

