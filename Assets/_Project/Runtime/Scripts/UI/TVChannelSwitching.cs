using System;
using UnityEngine;

namespace DShotgun
{
    [Serializable]
    public struct TVChannel
    {
        [SerializeField] private string _label;
        public string Label { get => _label; }

        [SerializeField] private GameObject[] _associatedGameobjects;
        public GameObject[] AssociatedGameobjects { get => _associatedGameobjects; }
    }
    
    public class TVChannelSwitching : MonoBehaviour
    {
        
    }
}
