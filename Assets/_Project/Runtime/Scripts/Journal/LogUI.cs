using UnityEngine;

namespace DShotgun
{
    public class LogUI : MonoBehaviour
    {
        [SerializeField] private Transform _entryParent;
        [SerializeField] private LogEntry _entryPrefab;

        private bool _visible;

        private void Awake()
        {
            Hide();
            LogSink.OnNewDialogEntry += AddEntry;
        }

        private void OnDestroy()
        {
            LogSink.OnNewDialogEntry -= AddEntry;
        }

        private void AddEntry(DialogEntryData data)
        {
            LogEntry le = Instantiate(_entryPrefab, _entryParent);
            le.Init(data);
        }

        public void ToggleVisible()
        {
            Debug.Log("Toggled");
            if(_visible) {
                Hide();
            }
            else {
                Show();
            }
        }
        

        public void Show()
        {
            _visible = true;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            _visible = false;
            gameObject.SetActive(false);
        }
    }
}
