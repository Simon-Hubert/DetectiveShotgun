using System;
using UnityEngine;

namespace DetectiveShotgun.Conditions
{
    [Serializable]
    public struct FlagReference
    {
        [SerializeField] private string _flagName;
        public string FlagName => _flagName;
    }
}
