using System;
using UnityEngine;

namespace DetectiveShotgun.Conditions
{
    /// <summary>
    /// Référence vers un <see cref="Flag"/> par son nom. C'est ce que les conditions et les séquences
    /// stockent pour désigner le flag qui les concerne
    /// </summary>
    [Serializable]
    public struct FlagReference
    {
        [SerializeField] private string _flagName;
        
        /// <summary>Nom du flag référencé, tel que déclaré dans le <see cref="BlackboardData"/>.</summary>
        public string FlagName => _flagName;
    }
}
