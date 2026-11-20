using System;
using System.Collections.Generic;
using UnityEngine;

namespace DShotgun
{
    [Serializable]
    public struct DialogEntryData
    {
        public string CharacterName;
        public string Text;

        public DialogEntryData(string name, string text)
        {
            Text = text;
            CharacterName = name;
        }
    }
    
    [Serializable]
    public struct JournalEntryData
    {
        public string Text;

        public JournalEntryData(string text)
        {
            Text = text;
        }
    }

    public static class LogSink
    {
        private static List<DialogEntryData> _dialogEntries = new List<DialogEntryData>();
        private static List<JournalEntryData> _journalEntries = new List<JournalEntryData>();
        
        public static IReadOnlyList<DialogEntryData> DialogEntries => _dialogEntries;
        public static IReadOnlyList<JournalEntryData> JournalEntries => _journalEntries;

        public static event Action<DialogEntryData> OnNewDialogEntry;
        public static event Action<JournalEntryData> OnNewJournalEntry;

        public static void LogDialog(DialogEntryData data)
        {
            _dialogEntries.Add(data);
            OnNewDialogEntry?.Invoke(data);
        }

        public static void LogJournal(JournalEntryData data)
        {
            _journalEntries.Add(data);
            OnNewJournalEntry?.Invoke(data);
        }
    }
}
