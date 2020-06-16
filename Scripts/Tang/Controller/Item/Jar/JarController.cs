using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

namespace Tang
{
    public class JarController : SceneObjectController, ITriggerDelegate
    {
        Rigidbody mainRigidbody;
        Animator animator;
        public GameObject GetGameObject() { return gameObject; }

        public List<string> dropItems = new List<string>();

        public override void Start()
        {
            base.Start();
            
            mainRigidbody = GetComponent<Rigidbody>();
            animator = GetComponentInChildren<Animator>();
        }

        void updateAnimatorState()
        {
            animator.GetInteger("state");
        }
        public async void playEffectAnim(string animName, Vector3 position, float rotation = 0.0f)
        {
            GameObject ins =
                await AssetManager.LoadAssetAsync<GameObject>(
                    "Aeests/Resources_moved/Prefabs/Effect/RoleEffectAnim.prefab");
            var roleEffectAnim = Instantiate(ins);
            var roleEffectSkeletonAnimation = roleEffectAnim.GetComponent<SkeletonAnimation>();
            roleEffectSkeletonAnimation.skeleton.SetToSetupPose();

            roleEffectSkeletonAnimation.state.SetAnimation(0, animName, false);

            Destroy(roleEffectAnim, 0.5f);

            // 旋转 add by TangJian 2017/08/25 17:50:30
            roleEffectAnim.transform.eulerAngles = new Vector3(0, 0, rotation);

            roleEffectSkeletonAnimation.transform.parent = transform.parent;
            roleEffectSkeletonAnimation.transform.position = position;
        }

        public void playDamageEffectAnim(DamageEffectType damageEffectType, Vector3 position, float rotation = 0.0f)
        {
            switch (damageEffectType)
            {
                case DamageEffectType.Slash:
                {
                    playEffectAnim("qiege2", position, rotation);
                }
                    break;
                case DamageEffectType.Strike:
                {
                    playEffectAnim("daji1", position, rotation);
                }
                    break;
            }
        }
        bool OnHurt(DamageData damageData)
        {
            switch (animator.GetInteger("state"))
            {
                case 0:
                    // playDamageEffectAnim(damageData.damageEffectType, damageData.collideBounds.center, MathUtils.SpeedToDirection(damageData.force));
                    playEffectAnim("daji1", damageData.collideBounds.center, MathUtils.SpeedToDirection(damageData.force));
                    animator.SetInteger("state", 1);
                    drop();
                    break;
                case 1:
                    break;
            }
            return true;
        }

        void drop()
        {
            foreach (var item in dropItems)
            {
                GameObject go = GameObjectManager.Instance.Create(item);
                if (go != null)
                {
                    DropItemController dropItemController = go.GetComponent<DropItemController>();
                    SceneManager.Instance.DropItemEnterSceneWithWorldPosition(dropItemController, SceneId, gameObject.transform.position + new Vector3(0, 1, 0));
                    Rigidbody rigidbody = go.GetComponent<Rigidbody>();
                    if (rigidbody != null)
                    {
                        float min = 100;
                        float max = 200;
                        rigidbody.AddForce(new Vector3(Random.Range(-max, max), Random.Range(min, max), Random.Range(-max, max)));
                    }
                }
            }
            Destroy(gameObject, 3);
        }

        public void OnTriggerIn(TriggerEvent evt)
        { }

        public void OnTriggerOut(TriggerEvent evt)
        { }
        public void OnTriggerKeep(TriggerEvent evt)
        {

        }
        public bool OnEvent(Event evt)
        {
            switch (evt.Type)
            {
                case EventType.DamageHurt:
                    OnHurt(evt.Data as DamageData);
                    break;
                case EventType.DamageHit:
                    break;
            }
            return true;
        }
    }
}