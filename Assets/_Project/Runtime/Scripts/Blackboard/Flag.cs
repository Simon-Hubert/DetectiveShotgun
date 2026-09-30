using System;
using UnityEngine;

namespace DetectiveShotgun.Conditions
{
    [Serializable]
    public struct Flag
    {
        [SerializeField] private string _name;
        [SerializeField] private bool _initializedValue;

        public string Name => _name;
        public bool InitializedValue => _initializedValue;
    }
}
