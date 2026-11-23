using HierarchySequences;
using UnityEngine;

namespace DShotgun
{
    public class LogJournalSequence : ASequencable
    {
        [SerializeField] private JournalEntryData _journalEntry;
        private bool _hasExecutedOnce = false;
        
        protected override async Awaitable OnPlay()
        {
            if (_hasExecutedOnce) return;
            LogSink.LogJournal(_journalEntry);
            _hasExecutedOnce = true;
        }
    }
}
