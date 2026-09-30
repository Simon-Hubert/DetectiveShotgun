using System.Collections.Generic;
using DetectiveShotgun.Conditions;
using UnityEngine;

public class Blackboard : MonoBehaviour
{
    [SerializeField] private BlackboardData _blackboardData;
    private Dictionary<string, bool> _flags = new Dictionary<string, bool>();
    
    public Dictionary<string, bool> Flags => _flags;
    
    private void Awake() 
    {
        if (_blackboardData == null)
        {
            Debug.LogError("BlackboardData is not assigned in the inspector.");
            return;
        }
        foreach (Flag flag in _blackboardData.Flags)
        {
            _flags[flag.Name] = flag.InitializedValue;
        }
    }
    
    public void RaiseFlag(string flagName) {
        _flags[flagName] = true;
    }
}
