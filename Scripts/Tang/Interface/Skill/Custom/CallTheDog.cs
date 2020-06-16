using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;

namespace Tang
{
    public class CallTheDog : MonoBehaviour, ISkillController
    {
        public GameObject Owner { get; set; }
        public string TeamId { get; set; }

        public void InitSkill(SkillData skillData)
        {
        }

        public void SetIgnoreList(List<int> ignoreList)
        {
            
        }

        public List<int> GetIgnoreList()
        {
            throw new System.NotImplementedException();
        }

        public void Cast()
        {
            GameObject dogGameObject = GameObjectManager.Instance.Create("dog");
            Debug.Assert(dogGameObject !=null, "召唤的角色不存在");
            dogGameObject.transform.parent = Owner.transform.parent;
            dogGameObject.transform.position = Owner.transform.position;

            RoleController roleController = dogGameObject.GetComponent<RoleController>();
            Debug.Assert(roleController != null, "召唤的不是角色");

            RoleController ownerController = Owner.GetComponent<RoleController>();

            SceneManager.Instance.ObjectEnterSceneWithWorldPosition(roleController, ownerController.SceneId,
                Owner.transform.position);
            
            // 播放渐显动画 add by TangJian 2019/3/22 17:43
            roleController.GetComponentInChildren<SkeletonRenderer>().GetComponent<Renderer>().material.SetColor("_Color", new Color(1, 1, 1, 0));
            roleController.GetComponentInChildren<SkeletonRenderer>().GetComponent<Renderer>().material.DOFade(1, 2);
              
            roleController.RoleData.TeamId = TeamId;



//            Renderer renderer = roleController.SkeletonAnimator.GetComponent<Renderer>();
//
//            roleController.gameObject.DOFade(0, 5f);

//            renderer.DOFade(0, 5f);
//            renderer.DOFade(1, 5f);
        }

        public Direction Direction { get; set; }
        public Vector3 Speed { get; set; }
        public void FlyTo(Vector3 pos)
        {
        }

        public void TowardTo(Vector3 pos)
        {
        }
    }
}