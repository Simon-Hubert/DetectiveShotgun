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
        public void Trigger()
        {
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
