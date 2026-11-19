using System;
using System.Collections.Generic;
using UnityEngine;

namespace DShotgun
{
    [Serializable]
    public struct LogEntryData
    {
        public string CharacterName;
        public string Text;

        public LogEntryData(string name, string text)
        {
            Text = text;
            CharacterName = name;
        }
    }

    public static class LogSink
    {
        private static List<LogEntryData> _entries = new List<LogEntryData>();

        private static LogUI _logUI;
        
        public static IReadOnlyList<LogEntryData> Entries => _entries;

        public static event Action<LogEntryData> OnNewEntry;

        public static void AddEntry(LogEntryData data)
        {
            _entries.Add(data);
            OnNewEntry?.Invoke(data);
        }
    }
    
    public class LogUI : MonoBehaviour
    {
        [SerializeField] private Transform _entryParent;
        [SerializeField] private LogEntry _entryPrefab;

        private bool _visible;

        private void Awake()
        {
            Hide();
            LogSink.OnNewEntry += AddEntry;
        }

        private void OnDestroy()
        {
            LogSink.OnNewEntry -= AddEntry;
        }

        private void AddEntry(LogEntryData data)
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
