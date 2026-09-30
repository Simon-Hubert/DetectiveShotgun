using System.Collections.Generic;
using DetectiveShotgun.Conditions;
using UnityEngine;

/// <summary>
/// État en jeu des flags de l'enquête. Les flags sont déclarés dans un <see cref="BlackboardData"/>
/// (asset édité par les GD) ; ce composant les charge au démarrage et garde leur valeur courante,
/// que les conditions consultent et que les séquences font évoluer.
/// </summary>
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
    
    /// <summary>
    /// Lève un flag (le passe à true). Sens unique : il n'existe pas d'opération inverse,
    /// un flag ne redescend qu'au prochain lancement.
    /// </summary>
    /// <param name="flagName">Nom du flag, tel que déclaré dans le <see cref="BlackboardData"/>.</param>
    public void RaiseFlag(string flagName) {
        _flags[flagName] = true;
    }
}
