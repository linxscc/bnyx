using System;
using GameCreator.Core;
using Tang;

namespace GameCreator.Scene
{
    using UnityEngine;
    

    [AddComponentMenu("")]
    public class IgniterSceneClean : Igniter 
    {
#if UNITY_EDITOR
        public new static string NAME = "Scene/On Clean";
#endif

        private SceneController sceneController;

        private void Start()
        {
            sceneController = GetComponentInParent<SceneController>();
            sceneController.OnSceneRoleDying += OnSceneRoleDying;
        }

        private void OnDestroy()
        {
            sceneController.OnSceneRoleDying -= OnSceneRoleDying;
        }

        private void OnSceneRoleDying(RoleController roleController)
        {
            if (sceneController.MonsterAliveCount <= 0)
            {
                this.ExecuteTrigger(gameObject);
            }
        }
    }
}