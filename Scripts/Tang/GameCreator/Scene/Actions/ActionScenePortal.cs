using GameCreator.Core;
using Tang;
using UnityEngine;

namespace GameCreator.Scene
{
    public class ActionScenePortal : IAction
    {
        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            var portalController = target.GetComponent<PortalController>();

            if (portalController.GetCurrentAnim())
            {
                portalController.SetColliderState(1);
                SceneEventManager.Instance.ObjStateChange(portalController.name, 1);
            }
            else
            {
                portalController.SetColliderState(0);
                SceneEventManager.Instance.ObjStateChange(portalController.name, 0);
            }
            
            
            return true;
        }
        
        #region EditorMode
        
        public override void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, transform.lossyScale);
        }
        

#if UNITY_EDITOR

        public static new string NAME = "Scene/Portal";
        private const string NODE_TITLE = "Portal";

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE);
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