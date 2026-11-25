using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using NaughtyAttributes;

namespace DShotgun
{
    public class CameraScenePosition : MonoBehaviour
    {
        [SerializeField] private InteractableController _interactableCtrl;
        [SerializeField] private List<Transform> _tableauCamPosList;
        
        public void ChangeCamPos(int a_index)
        {
            if (a_index < 0 || a_index >= _tableauCamPosList.Count) {
                Debug.LogError($"{name} : Invalid camera position index in tableau - list Lenght : {_tableauCamPosList.Count} - index : {a_index}", gameObject);
                return;
            }
            if (_interactableCtrl == null) {
                Debug.LogError($"{name} : No InteractableController, cannot change camera position", gameObject);
                return;
            }
            if (Camera.main == null) {
                Debug.LogError($"{name} : No main Camera, cannot change camera position", gameObject);
                return;
            }

            Debug.Log($"{name} : move to camera {a_index} at {_tableauCamPosList[a_index].position}, gameObject");
            _interactableCtrl.SetDefaultCamPos = _tableauCamPosList[a_index].position;
        }

        [Button]
        public void AddTableauPosAtCamera()
        {
            if (_tableauCamPosList == null) _tableauCamPosList = new List<Transform>();
            
            Transform pos = Instantiate(new GameObject($"Cam-Pos-{_tableauCamPosList.Count + 1}"), transform).transform;

            pos.position = Camera.main.transform.position;
            _tableauCamPosList.Add(pos);
        }
    }
}
