using System.Collections.Generic;
using System.IO;
using System.Linq;
using DShotgun.Dialogs;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DShotgun.Editor
{
    [CustomEditor(typeof(DialogFrame))]
    public class DialogFrameCustomInspector : UnityEditor.Editor
    {
        private static readonly string[] REFS = { "_leftRef", "_middleRef", "_rightRef" };

        private CharactersDatabase _database;
        private string[] _names;

        public bool _showAdditionalSettings = false;

        public override void OnInspectorGUI() {
            serializedObject.Update();

            _database = CharactersDatabase.Load();
            if (_database == null) {
                EditorGUILayout.HelpBox($"Il faut un asset « {CharactersDatabase.RESOURCE_PATH} » (CharactersDatabase) dans un dossier Resources.", MessageType.Error);
                return;
            }
            _names = _database.Characters.Select(e => e.Name).Prepend("None").ToArray();

            GUILayout.BeginHorizontal();
            for (int i = 0; i < REFS.Length; i++) CharacterPopup(i);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            SerializedProperty moveNext = serializedObject.FindProperty("_moveNextAutomatically");
            moveNext.boolValue = GUILayout.Toggle(moveNext.boolValue, "Move to next Frame immediately after");

            _showAdditionalSettings = EditorGUILayout.Foldout(_showAdditionalSettings, "Show Additional Settings");
            if (_showAdditionalSettings) DrawAdditionalSettings();

            GUILayout.Space(10);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_text"));
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawAdditionalSettings() {
            SerializedProperty speaker = serializedObject.FindProperty("_speakingCharacterID");
            if (GUILayout.Toggle(speaker.intValue == -1, "Don't Show Speaker")) speaker.intValue = -1;

            SerializedProperty useCustomName = serializedObject.FindProperty("_useCustomName");
            useCustomName.boolValue = GUILayout.Toggle(useCustomName.boolValue, "Use Custom Name");
            if (useCustomName.boolValue) {
                SerializedProperty customName = serializedObject.FindProperty("_name");
                customName.stringValue = GUILayout.TextField(customName.stringValue);
            }

            SerializedProperty useCustomProfile = serializedObject.FindProperty("_useCustomDialogProfile");
            useCustomProfile.boolValue = GUILayout.Toggle(useCustomProfile.boolValue, "Use Custom Dialog Profile");
            if (useCustomProfile.boolValue) {
                SerializedProperty profile = serializedObject.FindProperty("_characterDialogProfile");
                profile.objectReferenceValue = EditorGUILayout.ObjectField(profile.objectReferenceValue, typeof(SOCharacterDialogProfile), false);
            }
        }

        private void CharacterPopup(int position) {
            SerializedProperty slot = serializedObject.FindProperty(REFS[position]);
            SerializedProperty id = slot.FindPropertyRelative("_characterId");
            SerializedProperty spriteId = slot.FindPropertyRelative("_spriteId");

            GUILayout.BeginVertical();
            int selected = _database.TryGetCharacter(id.intValue, out CharacterData current) ? IndexOf(current.Id) + 1 : 0;   // 0 = None
            int newSelected = EditorGUILayout.Popup(selected, _names);
            if (newSelected != selected) {
                selected = newSelected;
                id.intValue = selected == 0 ? 0 : _database.Characters[selected - 1].Id;
                spriteId.intValue = 0;
            }

            if (selected > 0) {
                IReadOnlyList<NamedSprite> sprites = _database.Characters[selected - 1].SpriteList;
                int selectedSprite = 0;
                for (int i = 0; i < sprites.Count; i++) {
                    if (sprites[i].Id == spriteId.intValue) selectedSprite = i;
                }

                selectedSprite = EditorGUILayout.Popup(selectedSprite, sprites.Select(e => e.Name).ToArray());
                if (sprites.Count > 0) spriteId.intValue = sprites[selectedSprite].Id;
            }

            SerializedProperty speaker = serializedObject.FindProperty("_speakingCharacterID");
            if (GUILayout.Toggle(position == speaker.intValue, "Speaking")) speaker.intValue = position;
            GUILayout.EndVertical();
        }

        // Réécrit les prefabs et scènes qui contiennent des DialogFrame au format actuel (retire les anciens champs)
        [MenuItem("Tools/Dialogues/Réécrire les DialogFrame au format actuel")]
        private static void ReserializeDialogFrames() {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            string guid = AssetDatabase.FindAssets("DialogFrame t:MonoScript")
                .First(g => AssetDatabase.LoadAssetAtPath<MonoScript>(AssetDatabase.GUIDToAssetPath(g)).GetClass() == typeof(DialogFrame));
            List<string> paths = AssetDatabase.FindAssets("t:Prefab t:Scene", new[] { "Assets/_Project" })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => File.ReadAllText(path).Contains(guid))
                .ToList();

            AssetDatabase.ForceReserializeAssets(paths);
            Debug.Log($"{paths.Count} prefabs et scènes réécrits :\n{string.Join("\n", paths)}");
        }

        private int IndexOf(int id) {
            for (int i = 0; i < _database.Characters.Count; i++) {
                if (_database.Characters[i].Id == id) return i;
            }
            return -1;
        }
    }
}
