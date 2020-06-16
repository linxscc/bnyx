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
    public class ActionScenePlayAnimEffect : IAction
    {
        public string animEffectId;
        public int count = 1;
        
        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 randomPosition = Tang.Tools.RandomPositionInCube(transform.position, transform.lossyScale);
                AnimManager.Instance.PlayAnimEffect(animEffectId, randomPosition, 0, false, Vector3.zero, transform);
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

        public static new string NAME = "Scene/播放动画特效";
        private const string NODE_TITLE = "播放动画特效";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spAnimEffectId;
        private SerializedProperty spCount;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE);
        }

        protected override void OnEnableEditorChild ()
        {
            this.spAnimEffectId = this.serializedObject.FindProperty("animEffectId");
            this.spCount = this.serializedObject.FindProperty("count");
        }

        protected override void OnDisableEditorChild ()
        {
            
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();
            EditorGUILayout.PropertyField(this.spAnimEffectId);
            EditorGUILayout.PropertyField(this.spCount);
            this.serializedObject.ApplyModifiedProperties();
        }
#endif
    }
}