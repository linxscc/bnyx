using System;
using GameCreator.Core;
using Tang;
using UnityEditor;
using UnityEngine;

namespace GameCreator.Scene
{
    public class ActionSceneFenceDoor :IAction
    {
        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            var iIntractable = target.GetComponent<IInteractable>();
            SceneEventManager.Instance.ObjStateChange("1F1JoyStick", iIntractable.State);
            return true;
        }
        
        #region EditorMode
        
        public override void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, transform.lossyScale);
        }
        

#if UNITY_EDITOR

        public static new string NAME = "Scene/FenceDoor";
        private const string NODE_TITLE = "FenceDoor";

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE);
        }

        protected override void OnEnableEditorChild ()
        {
            
        }

        protected override void OnDisableEditorChild ()
        {
            
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();
            this.serializedObject.ApplyModifiedProperties();
        }
#endif
        
        #endregion  
  
    }
}