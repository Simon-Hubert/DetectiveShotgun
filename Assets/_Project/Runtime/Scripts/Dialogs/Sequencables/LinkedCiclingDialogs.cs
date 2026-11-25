using System;
using System.Collections.Generic;
using HierarchySequences;
using UnityEngine;

namespace DShotgun.Dialogs
{
    public class LinkedCiclingDialogs : ASequencable
    {
        private List<ASequencable> _cycle = new List<ASequencable>();

        [SerializeField]
        private string _group;

        private static Dictionary<string, int> _groupToCurrentIndex = new Dictionary<string, int>();
        
        private void Awake() {
            foreach (Transform child in transform) {
                if (!child.gameObject.activeSelf) continue;
                ASequencable sequencable = child.GetComponent<ASequencable>();
                if(sequencable) _cycle.Add(sequencable);
            }

            if (_cycle.Count > 0) _groupToCurrentIndex[_group] = 0;
        }

        protected override async Awaitable OnPlay()
        {
            try
            {
                if (_groupToCurrentIndex[_group] >= 0) {
                    await _cycle[_groupToCurrentIndex[_group]].ExecuteAsync();
                    _groupToCurrentIndex[_group]++;
                    _groupToCurrentIndex[_group] %= _cycle.Count;
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
