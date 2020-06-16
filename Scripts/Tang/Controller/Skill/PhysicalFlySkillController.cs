using System.Collections.Generic;
using UnityEngine;

namespace Tang
{
    public class PhysicalFlySkillController : FlySkillController
    {
        public float GrivityAcceleration = 20;
        public bool CanUpdate = true;

        private bool useGrivity = false;


        private Vector3 rotateSpeed = Vector3.zero;
        private SkillOrientationMode SkillOrientationMode;


        private bool IsJoinAnim;
        private string AnimName_Role;
        private string AnimName_Scene;

        
        public override void InitSkill(SkillData skillData)
        {
            base.InitSkill(skillData);

            useGrivity = skillData.useGravity;
            if (useGrivity)
            {
                // 设置重力 add by TangJian 2019/4/20 13:00
                GrivityAcceleration = skillData.gravitationalAcceleration;
                MainRigidbody.useGravity = false;
            
                // 添加恒定外力组件作为重力 add by TangJian 2019/4/19 23:52
                ConstantForce constantForce = MainRigidbody.gameObject.AddComponent<ConstantForce>();
                constantForce.force = new Vector3(0, -MainRigidbody.mass * GrivityAcceleration, 0);
            }
            else
            {
                MainRigidbody.useGravity = false;
            }
            
            // 朝向模式
            SkillOrientationMode = skillData.SkillOrientationMode;
            
            // 旋转 add by TangJian 2019/4/20 13:26
            rotateSpeed = skillData.rotateSpeed;

            IsJoinAnim = skillData.IsJoinAnim;
            AnimName_Role = skillData.AnimEffectName_Role;
            AnimName_Scene = skillData.AnimEffectName_Scene;
        }
        
        public override void InitDamage()
        {
            base.InitDamage();
            var damageData = MainDamageController.damageData;
            damageData.itriggerDelegate = this;
            damageData.force = Vector3.zero;
            damageData.owner = gameObject;
        }

        public override void SetIgnoreList(List<int> ignoreList)
        {
            base.SetIgnoreList(ignoreList);
            MainDamageController.damageData.ignoreObjectList = GetIgnoreList();
        }

        public override void FlyTo(Vector3 pos)
        {
            if (useGrivity)
                Speed = Tools.GetFlyToPosSpeed(new Vector2(1, 1), pos - transform.position, -GrivityAcceleration);
            else
                Speed = Speed.magnitude * (pos - transform.position).normalized;
        }

        public override void TowardTo(Vector3 pos)
        {
            var offset = pos - transform.position;
            offset.y = 0;
            
            // 设置速度的时候会自动修改速度方向, 这里还原回来 add by TangJian 2019/4/23 14:25
            Speed = new Vector3(Mathf.Abs(Speed.x), Speed.y, Speed.z);
            
            Speed = Quaternion.FromToRotation(Vector3.right, offset) * Speed;
        }

        // 击中目标自身消失 add by TangJian 2019/4/19 14:21
        public override bool OnEvent(Event evt)
        {
            switch (evt.Type)
            {
                case EventType.DamageHit:
                    // 击中目标 add by TangJian 2018/01/15 15:05:39
                    MainDamageController.gameObject.SetActive(false);
                    PlayJoinAnim();
                    break;
            }
            return true;
        }

        public override bool OnHit(DamageData damageData)
        {
            PlayJoinAnim(true);
            return base.OnHit(damageData);
        }

        // 触碰地面 add by TangJian 2019/4/19 14:21
        public override void OnTriggerIn(TriggerEvent evt)
        {
            if (Speed.y < 0 && (evt.colider.gameObject.layer == LayerMask.NameToLayer("Ground") || evt.colider.gameObject.tag == "BackWall"))
            {
                MainDamageController.gameObject.SetActive(false);
                CanUpdate = false;
                State = 2;
        
                Speed = Vector3.zero;
                MainRigidbody.isKinematic = true;
                gameObject.GetChild("Collider").GetChild("BoxCollider").SetActive(false);
                PlayJoinAnim();
            }
        }

        private void PlayJoinAnim(bool IsRole = false)
        {
            string AnimName = IsRole ? AnimName_Role : AnimName_Scene;
            if (string.IsNullOrEmpty(AnimName) || !IsJoinAnim) return;
            AnimManager.Instance.PlayAnimEffect(AnimName, transform.position);
            if (IsJoinAnim) Destroy(gameObject);
        }
        
        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            if (CanUpdate)
            {
                
                // 刷新击退方向 add by TangJian 2019/4/19 23:55
                MainDamageData.targetMoveBy = Speed.normalized * MainDamageData.targetMoveBy.magnitude;
            
                // 刷新渲染 add by TangJian 2019/4/19 14:19
                RefreshRenderer(Time.fixedDeltaTime);
            }
        }

        public void RefreshRenderer(float deltaTime)
        {
            if (MainRigidbody.freezeRotation == false)
            {
                switch (SkillOrientationMode)
                {
                    case SkillOrientationMode.Speed:
                        Direction = Direction.Right;

//                        float angleX = Speed.GetAngleX();
                        float angleY = Speed.GetAngleY();
                        float angleZ = Speed.GetAngleZ();

                        if (angleZ > 90 && angleZ <= 180)
                        {
                            angleZ = 180f - angleZ;
                        }
                        else if (angleZ > 180 && angleZ < 270)
                        {
                            angleZ = -(angleZ - 180f);
                        }

                        transform.eulerAngles = new Vector3(0, -angleY, angleZ);
                        break;
                    case SkillOrientationMode.Direction:
                        if (Direction == Direction.Right)
                        {
                            transform.eulerAngles = new Vector3(0, 0, 0);
                        }
                        else
                        {
                            transform.eulerAngles = new Vector3(0, 180, 0);
                        }
                        break;
                    case SkillOrientationMode.Rotation:
                        transform.eulerAngles = (transform.rotation.eulerAngles + new Vector3(rotateSpeed.x, rotateSpeed.y, rotateSpeed.z * DirectionInt) * deltaTime);
                        break;
                }
                
            }
        }
    }
}