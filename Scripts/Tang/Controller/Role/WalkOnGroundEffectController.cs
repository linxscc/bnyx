using System.Collections.Generic;
using System.Net.Sockets;
using BehaviorDesigner.Runtime.Tasks.Basic.UnityCapsuleCollider;
using BehaviorDesigner.Runtime.Tasks.Basic.UnityCharacterController;
using GameCreator.Characters;
using Tang.FrameEvent;
using UnityEngine;
using ZS;

namespace Tang
{
    public enum WalkType
    {
        Idle = 1,
        Walk = 2,
        Run = 3,
        Rush = 4,
        Jump = 5,
        Dodge =6,
        Attackmove = 7,
        Landing = 8
    }

    public interface IWalkOnGroundEffectController
    {
        void PlayEffect(WalkType walkType, MatType groundMatType, Transform parent, Vector3 position, 
            Vector3 pos, bool flip = false);
    }

    
    public class WalkOnGroundEffectController : IWalkOnGroundEffectController
    {
        private static Dictionary<string,RoleInteraction> roleInteractionDic;
        
        
//        [RuntimeInitializeOnLoadMethod]
        public static async void LoadRes()
        {
            roleInteractionDic = await AssetManager.LoadJson<Dictionary<string,RoleInteraction>>(Definition.WalkOnGroundEffectFileName);
        }

        private string animName;
        private RoleController selfController; 
        
        public void PlayEffect(WalkType walkType, MatType groundMatType, Transform parent, Vector3 position, 
            Vector3 pos, bool flip = false)
        {
            Vector3 playAnimNameLocation;
            if (flip==false)
            {
                playAnimNameLocation = position + pos;
            }
            else
            {
                playAnimNameLocation = position - pos;
            }

            string animEffectId = GetEffectId(walkType, groundMatType);
            Debug.Assert(string.IsNullOrWhiteSpace(animEffectId) == false, 
                "animEffectId == " + animEffectId == null ? "null" : animEffectId);

//            if (isGround == false)
//            {
//                Debug.Log("stop play animor");
//            }
//            else
//            {
//            
//            }

            AnimManager.Instance.PlayAnimEffect(animEffectId, playAnimNameLocation, 0, flip, 
                Vector3.zero,  parent, 0);
        }

        private string GetEffectId(WalkType walkType, MatType groundMatType)
        {
            string key = walkType.ToString() + "_" + groundMatType.ToString();
            RoleInteraction roleInteraction;    
            if (roleInteractionDic.TryGetValue(key, out roleInteraction))
            {
                Debug.Log("roleInteraction.EffectsKey为" + roleInteraction.hurtEffect);
                return roleInteraction.hurtEffect;
            }
            return null;
        }
    }
}