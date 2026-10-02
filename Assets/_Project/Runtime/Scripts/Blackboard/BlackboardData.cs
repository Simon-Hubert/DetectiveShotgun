using System;
using System.Collections.Generic;
using UnityEngine;

namespace DShotgun
{
    [CreateAssetMenu(menuName = "DetectiveShotgun/BlackboardData")]
    public class BlackboardData : ScriptableObject
    {
        [SerializeField] private List<Flag> _flags = new List<Flag>();
        
        public IReadOnlyList<Flag> Flags => _flags;


        private void OnValidate()
        {
            for (int i = 0; i < _flags.Count; i++)
            {
                for (int j = i + 1; j < _flags.Count; j++)
                {
                    if (_flags[i].Name == _flags[j].Name)
                    {
                        Debug.LogError($"Dans l'objet {name} deux flags ont le meme nom : {_flags[i].Name}");
                    }
                }
            }
        }
    }
}
