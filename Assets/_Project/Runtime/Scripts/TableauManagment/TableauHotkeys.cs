using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace DShotgun
{
    public class TableauHotkeys : MonoBehaviour
    {
        [Serializable]
        private struct Entry
        {
            public string Lieu;
            public int State;
        }

        private struct Tableau
        {
            public string Name;
            public int LieuIndex;
            public int StateIndex;
        }

        private const int MAX_TABLEAUX = 10;
        private static TableauHotkeys _instance;

        [SerializeField] private List<Entry> _tableaux = new List<Entry>
        {
            new Entry { Lieu = "House", State = 1 },
            new Entry { Lieu = "Parking", State = 1 },
            new Entry { Lieu = "Damas", State = 1 },
            new Entry { Lieu = "PoliceStation", State = 1 },
            new Entry { Lieu = "House", State = 2 },
            new Entry { Lieu = "House", State = 3 },
        };

        private readonly List<Tableau> _resolved = new List<Tableau>();

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);

            Resolve();
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null || TableauManager.IsLoading) return;

            for (int i = 0; i < _resolved.Count; i++)
            {
                if (keyboard[DigitKey(i)].wasPressedThisFrame || keyboard[NumpadKey(i)].wasPressedThisFrame)
                {
                    Load(_resolved[i]);
                    return;
                }
            }
        }

        private void Resolve()
        {
            Dictionary<string, int> buildIndexes = new Dictionary<string, int>();
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                buildIndexes.TryAdd(Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)), i);
            }

            if (_tableaux.Count > MAX_TABLEAUX)
            {
                Debug.LogWarning($"[TableauHotkeys] {_tableaux.Count} tableaux, seuls les {MAX_TABLEAUX} premiers ont une touche");
            }

            for (int i = 0; i < _tableaux.Count && i < MAX_TABLEAUX; i++)
            {
                Entry entry = _tableaux[i];
                string stateScene = $"{entry.Lieu}-State-{entry.State}";
                if (!buildIndexes.TryGetValue(entry.Lieu, out int lieuIndex) || !buildIndexes.TryGetValue(stateScene, out int stateIndex))
                {
                    Debug.LogWarning($"[TableauHotkeys] Touche {DigitKey(i)} ignorée : scène \"{entry.Lieu}\" ou \"{stateScene}\" absente des Build Settings");
                    _resolved.Add(default);
                    continue;
                }

                _resolved.Add(new Tableau { Name = $"{entry.Lieu} - State {entry.State}", LieuIndex = lieuIndex, StateIndex = stateIndex });
                Debug.Log($"[TableauHotkeys] Touche {DigitKey(i)} : {_resolved[i].Name}");
            }
        }

        private static void Load(Tableau tableau)
        {
            if (tableau.Name == null) return;
            TableauManager.LoadTableau(tableau.LieuIndex, tableau.StateIndex);
        }

        private static Key DigitKey(int index) => index < 9 ? Key.Digit1 + index : Key.Digit0;

        private static Key NumpadKey(int index) => index < 9 ? Key.Numpad1 + index : Key.Numpad0;
    }
}
