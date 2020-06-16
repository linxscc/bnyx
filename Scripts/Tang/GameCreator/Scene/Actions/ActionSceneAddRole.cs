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
    public class ActionSceneAddRole : IAction
    {
        public SceneController sceneController;
        public string roleId;
        public int count = 1;
        
        
        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            for (int i = 0; i < count; i++)
            {
                sceneController = gameObject.GetComponentInParent<SceneController>();
                sceneController.AddRole(roleId, Tang.Tools.RandomPositionInCube(transform.position, transform.lossyScale));
            }
            
            return true;
        }

        public override void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, transform.lossyScale);
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Scene/Add Role";
        private const string NODE_TITLE = "Add Role";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spRoleId;
        private SerializedProperty spCount;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE);
        }

        protected override void OnEnableEditorChild ()
        {
            this.spRoleId = this.serializedObject.FindProperty("roleId");
            this.spCount = this.serializedObject.FindProperty("count");
        }

        protected override void OnDisableEditorChild ()
        {
            
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();
            EditorGUILayout.PropertyField(this.spRoleId);
            EditorGUILayout.PropertyField(this.spCount);
            this.serializedObject.ApplyModifiedProperties();
        }
#endif
    }
}