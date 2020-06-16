using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

namespace Tang.Controller.MonsterSpawn
{
    public class MonsterSpawn : SceneObjectController
    {
        [Serializable]
        public class RoleGroup
        {
            public List<string> RoleIdList;
        }

        public int CurrIndex = 0;
        public List<RoleGroup> RoleGroupList= new List<RoleGroup>();
            
        public float delayTime = 5f;
        private FSM _fsm;

        private int aliveMonsterCount = 0;
        
        public override void Start()
        {
            base.Start();
            
            InitFsm();
        }

        void InitFsm()
        {
            _fsm = new FSM();

            _fsm.SetCurrStateName("Idle");
            
            float remainingTime = 0;
            _fsm.AddState("Idle");
            
            _fsm.AddState("DelayToMonsterSpawn", () =>
            {
                remainingTime = delayTime;
            }, () =>
            {
                remainingTime -= Time.deltaTime;
                if (remainingTime <= 0)
                {
                    _fsm.SendEvent("AllToMonsterSpawn");
                }
            }, () =>
            {
            });
            
            IEnumerator enumerator = null;
            _fsm.AddState("MonsterSpawn", () =>
            {
                int level = 1;
                if (GameManager.Instance != null && GameManager.Instance.Player1 != null && GameManager.Instance.Player1.RoleData != null)
                {
                    level = GameManager.Instance.Player1.RoleData.level;
                }

                enumerator = Spawn(RoleGroupList[(CurrIndex++) % RoleGroupList.Count].RoleIdList, "2", level);
            }, () =>
            {
                bool canNext = enumerator.MoveNext();
                RoleController roleController = enumerator.Current as RoleController;
                if (roleController)
                {
                    aliveMonsterCount++;
                    roleController.OnDying += RoleDying;
                }

                if (!canNext)
                {
                    _fsm.SendEvent("AllToIdle");
                }
            }, () =>
            {
                enumerator = null;
            });

            _fsm.AddEvent("IdleToMonsterSpawn", "Idle", "DelayToMonsterSpawn", () => { return aliveMonsterCount == 0; });
        }

        void RoleDying(RoleController roleController)
        {
            aliveMonsterCount--;
        }

        IEnumerator Spawn(List<string> roleIdList, string teamId, int level)
        {
            // 刷怪前掉落装备给玩家 add by TangJian 2019/3/23 11:59
            List<string> itemIdList = ItemManager.Instance.GetDropItemByLevel(level);
            string dropItemId = itemIdList[Random.Range(0, itemIdList.Count)];

            GameObject dropItemObject = GameObjectManager.Instance.Create(dropItemId);
            SceneObjectController sceneObjectController = dropItemObject.GetComponent<SceneObjectController>();
            if (sceneObjectController != null)
            {
                SceneManager.Instance.DropItemEnterSceneWithWorldPosition(sceneObjectController, SceneId, Tools.RandomPositionInCube(transform.position, transform.lossyScale));
            }

            
            for (int i = 0; i < roleIdList.Count; i++)
            {
                string roleId = roleIdList[i];
                GameObject roleGameObject = GameObjectManager.Instance.Create(roleId);
                RoleController roleController = roleGameObject.GetComponent<RoleController>();
                roleController.RoleData.TeamId = teamId;
                roleController.RoleData.difficultyLevel = level;

                SceneManager.Instance.RoleEnterSceneWithWorldPosition(roleController, SceneId,
                    Tools.RandomPositionInCube(transform.position, transform.lossyScale));
                
                yield return roleController;
            }
            yield return 0;
        }
        
        public override void OnUpdate()
        {
            _fsm.Update();
        }
    }
}