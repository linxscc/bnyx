using System;
using Tang;

namespace GameCreator.Scene
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using GameCreator.Core;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    [AddComponentMenu("")]
    public class ActionSceneAddItem : IAction
    {
        public SceneController sceneController;
        public string itemId;
        public int count = 1;

        
        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            sceneController = gameObject.GetComponentInParent<SceneController>();
            for (int i = 0; i < count; i++)
                DropItem();
            return true;
        }

        private async void DropItem()
        {
            GameObject go = await AssetManager.InstantiateDropItem(itemId);
            DropItemController dropItemController = go.GetComponent<DropItemController>();
            SceneManager.Instance.DropItemEnterSceneWithWorldPosition(dropItemController, sceneController.name,
                Tang.Tools.RandomPositionInCube(transform.position, transform.lossyScale) + new Vector3(0, 1, 0));
        }

        public override void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, transform.lossyScale);
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Scene/Add Item";
        private const string NODE_TITLE = "Add Item";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spItemId;
        private SerializedProperty spCount;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE);
        }

        protected override void OnEnableEditorChild ()
        {
            this.spItemId = this.serializedObject.FindProperty("itemId");
            this.spCount = this.serializedObject.FindProperty("count");
        }

        protected override void OnDisableEditorChild ()
        {
            
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();
            EditorGUILayout.PropertyField(this.spItemId);
            EditorGUILayout.PropertyField(this.spCount);
            this.serializedObject.ApplyModifiedProperties();
        }
#endif
    }
}