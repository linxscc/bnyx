using UnityEngine;
using UnityEditor;




namespace Tang
{
    [CustomEditor(typeof(GameObjectExtensions))]
    [CanEditMultipleObjects]
    public class GameObjectExtensionsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Apply", "MiniButtonRight", new GUILayoutOption[0]))
            {
                foreach (var target in targets)
                {
                    GameObjectExtensions gameObjectExtensions = target as GameObjectExtensions;
                    Tools.ModifyPrefab(gameObjectExtensions.gameObject);
                }
            }
        }
    }
}