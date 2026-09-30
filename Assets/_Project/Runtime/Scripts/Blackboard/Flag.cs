using System;
using UnityEngine;

namespace DetectiveShotgun.Conditions
{
    /// <summary>
    /// Un flag de l'enquête déclaré dans un
    /// <see cref="BlackboardData"/>. Cette struct ne contient que la définition (nom et valeur initiale) ;
    /// la valeur courante en jeu est tenue par le <see cref="Blackboard"/>.
    [Serializable]
    public struct Flag
    {
        [SerializeField] private string _name;
        [SerializeField] private bool _initializedValue;

        /// <summary>
        /// Nom du flag défini dans le <see cref="BlackboardData"/>
        /// </summary>
        public string Name => _name;
        
        /// <summary>
        /// Valeur initiale du flag défini dans le <see cref="BlackboardData"/>
        /// </summary>
        public bool InitializedValue => _initializedValue;
    }
}
