using HierarchySequences;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DShotgun.Editor
{
    public static class MenuItems
    {
        private const string GO_CREATE_ROOT = "GameObject/";

        [MenuItem(GO_CREATE_ROOT + "Interactable", false, 2)]
        public static void CreateInteractableObject(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("New Interactable");
            Vector3 createPos = default;
            if( SceneView.lastActiveSceneView != null )
                createPos = SceneView.lastActiveSceneView.pivot;
            go.transform.position = createPos;
            go.AddComponent<BoxCollider2D>();
            go.AddComponent<InteractableManager>();
            GameObject onInteract = new GameObject("OnInteract");
            onInteract.transform.position = createPos;
            onInteract.transform.SetParent(go.transform);
            onInteract.AddComponent<Sequence>();
            GameObject onAim = new GameObject("OnAim");
            onAim.transform.position = createPos;
            onAim.transform.SetParent(go.transform);
            onAim.AddComponent<Sequence>();
            GameObject visuals = new GameObject("Visuals");
            visuals.transform.position = createPos;
            visuals.transform.SetParent(go.transform);
            visuals.AddComponent<SpriteRenderer>();
            visuals.AddComponent<SpriteSwitcher>();
            Selection.activeObject = go;
            Undo.RegisterCreatedObjectUndo( go, $"created New Interactable" );
            Place( go, menuCommand.context );
        }
        
        private static void Place( GameObject go, Object context ) {
            Transform parentTransform;
            if( context != null && context is GameObject goCtx )
                parentTransform = goCtx.transform;
            else
                parentTransform = PrefabStageUtility.GetCurrentPrefabStage()?.prefabContentsRoot.transform;
            Transform transform = go.transform;
            Undo.SetTransformParent( transform, parentTransform, "Reparenting" );
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            if( parentTransform != null ) {
                go.layer = parentTransform.gameObject.layer;
                if( parentTransform.GetComponent<RectTransform>() )
                    ObjectFactory.AddComponent<RectTransform>( go );
            }
        }
    }
}
