using System.Collections.Generic;
using HierarchySequences;
using UnityEngine;
using UnityEngine.Events;

namespace DShotgun
{
    public class Counter : MonoBehaviour
    {
        [SerializeField] private int _triggerValue = 1;
        [SerializeField] private Sequence[] _sequences;
        [SerializeField] private UnityEvent _onTrigger;
        [SerializeField] private UnityEvent _onCountReached;
        private int _count = 0;
        private bool _reached = false;
        private List<int> _ids = new List<int>();
        public void Trigger(int id = 0)
        {
            if (id != 0)
            {
                if(_ids.Contains(id)) return;
                _ids.Add(id);
            }
            if(_sequences.Length > 0)
            {
                _sequences[_count].Execute();
            }
            _onTrigger?.Invoke();
            ++_count;
            if (!_reached && _count > _triggerValue)
            {
                _reached = true;
                _onCountReached?.Invoke();
            }
        }
    }
}
