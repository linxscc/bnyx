using UnityEngine;
using Spine.Unity;
using System.Collections.Generic;
namespace Tang
{
    public class BoomController : FlySkillController
    {
        List<RoleController> mubiaolist = new List<RoleController>();
        public override void Awake()
        {
            base.Awake();
            SkeletonAnimator  skeletonAnimator= MainAnimator.gameObject.GetComponent<SkeletonAnimator>();
            skeletonAnimator.initialSkinName = "qiu";
            skeletonAnimator.Initialize(true);
            MainRigidbody.useGravity = true;
            MainRigidbody.freezeRotation = true;
            foreach(RoleController roleController in SceneManager.Instance.CurrScene.RoleControllerList)
            {
                if (roleController.RoleData.TeamId != TeamId)
                {
                    mubiaolist.Add(roleController);
                }
            }
            mubiaolist.Sort((RoleController a, RoleController b) =>
            {
                if ((gameObject.transform.position-a.transform.position).magnitude>(gameObject.transform.position - b.transform.position).magnitude)
                {
                    return 1;
                }
                else if((gameObject.transform.position - a.transform.position).magnitude < (gameObject.transform.position - b.transform.position).magnitude)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            });
            // MainDamageController.gameObject.SetActive(false);
        }
        public override void InitDamage()
        {
            // MainDamageController = gameObject.GetChild("Damage").GetComponent<DamageController>();

        }

        public override Vector3 Speed
        {
            set
            {
                MainRigidbody.velocity = value * 3;
                if (value.magnitude != 0)
                    gameObject.transform.localEulerAngles = new Vector3(0, Tools.GetDegree(MainRigidbody.velocity.x, MainRigidbody.velocity.y), 0);
            }
            get
            {
                return MainRigidbody.velocity;
            }
        }

        public override void OnFinished()
        {
            Destroy(gameObject);
        }

        public override void OnTriggerIn(TriggerEvent evt)
        {
            if (evt.otherTriggerController != null&& evt.otherTriggerController.gameObject.tag=="DamageTarget")
            {
                OnHit();
            }
        }
        public override void OnHit()
        {
            State = 2;
            MainRigidbody.freezeRotation = true;
            Speed = Vector3.zero;
            MainRigidbody.isKinematic = true;
        }

        public override bool OnHit(DamageData damageData)
        {
            return base.OnHit(damageData);
        }

        void OnCollisionEnter(Collision other)
        {
            // if (other.gameObject.layer == LayerMask.NameToLayer("DamageTarget"))
            if (other.gameObject.layer != LayerMask.NameToLayer("Ground") && other.gameObject.layer != LayerMask.NameToLayer("SceneComponent"))
            {
                Debug.Log("设置状态!!!!!!");
                State = 2;
                // 冻结z方向旋转 add by TangJian 2018/05/08 20:04:58
                MainRigidbody.freezeRotation = true;
                transform.eulerAngles = Vector3.zero;
                Speed = Vector3.zero;
                MainRigidbody.isKinematic = true;


            }
        }

        public override void OnUpdate()
        {
            //transform.eulerAngles = new Vector3(0, 0, MathUtils.SpeedToDirection(Speed));
            //SceneManager.Instance.CurrSceneController.RoleControllerList
            GameObject player1;
            if (mubiaolist.Count != 0)
            {
                player1 = mubiaolist[0].gameObject;
            }
            else
            {
                player1 = GameObject.Find("Player1");
            }
            

            var addForce = (player1.transform.position - gameObject.transform.position).normalized * 10;
            addForce = new Vector3(addForce.x, 0, addForce.z);
            MainRigidbody.AddForce(addForce);
        }
    }
}

