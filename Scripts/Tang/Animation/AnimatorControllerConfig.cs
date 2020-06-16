using System.Collections.Generic;
using UnityEngine;


namespace Tang
{
    public class AnimatorTransitionCondition
    {
        public UnityEditor.Animations.AnimatorConditionMode animatorConditionMode;
        public float threshold;
        public string parameter;
    }

    public class 
        AnimatorTransition
    {
        public List<AnimatorTransitionCondition> conditions;

        public List<string> ignoreSourceAnimNames;
        public List<string> ignoreDestinationAnimNames;

        public List<string> containSourceAnimNames;
        public List<string> containDestinationAnimNames;

        public List<string> ignoreSourceAnimTags;
        public List<string> ignoreDestinationAnimTags;

        public List<string> containSourceAnimTags;
        public List<string> containDestinationAnimTags;

        public string sourceAnimName;
        public string destinationAnimName;

        public UnityEditor.Animations.TransitionInterruptionSource interruptionSource = UnityEditor.Animations.TransitionInterruptionSource.Destination;
        
        public bool hasExitTime = false;
        public float exitTime = 1f;

        public bool hasFixedDuration = true;
        public float duration = 0f;
        public float offset = 0;
        
        
    }

    // new UnityEditor.Animations.BlendTree();
    public class ChildMotion
    {
        public float threshold = 0;
        public float timeScale = 1;
        
        public BlendTree blendTree;
    }

    public class BlendTree
    {
        public string blendParameter;
        public bool useAutomaticThresholds = true;
        public float minThreshold = int.MinValue;
        public float maxThreshold = int.MinValue;
        public List<ChildMotion> childMotions;
    }

    public class AnimatorState
    {
        public string name;
        public string tag;
        public string animName;
        public float time = 0;
        public int totalFrame = 1;
        public float Speed = 1;
        public float duration = -1;
        public string script;
        public bool loop;
        public List<AnimatorTransition> transitions = new List<AnimatorTransition>();
        public List<string> behaviours = new List<string>();

        public BlendTree blendTree;
        public float MinSpeed;
        public float NormalMoveSpeed;

        public Vector3 MoveVector = Vector3.right;

        public float beginTime;
        public float endTime;
        

        public RoleAnimState animState;
    }

    public class AnimatorParameter
    {
        public string name;
        public AnimatorControllerParameterType type;
    }

    public class AnimatorTag
    {
        public string tag;
    }

    public class AnimatorControllerConfig
    {
        public List<AnimatorParameter> parameters = new List<AnimatorParameter>();
        public List<AnimatorState> states = new List<AnimatorState>();
        public List<AnimatorTransition> transitions = new List<AnimatorTransition>();
        public List<AnimatorTag> Tags = new List<AnimatorTag>();
    }
    
    
    
    
    
    
    // 角色状态机类
    public class RoleAnimator
    {
        public Dictionary<string, RoleIdleState> idle;
        public Dictionary<string, RoleWalkState> walk;
        public Dictionary<string, RoleRunState> run;
        public Dictionary<string, RoleTurnBackState> turnback;
        public Dictionary<string, RoleActionState> action;
        public Dictionary<string, RoleHurtState> hurt;
        public Dictionary<string, RoleDeathState> death;

        public Dictionary<string, RoleJumpState> jump;
        
        public Dictionary<string, RoleDodgeState> dodge;
    }

    public class RoleIdleState : RoleAnimState { }

    public class RoleWalkState : RoleAnimState { }

    public class RoleActionState : RoleAnimState
    {
        public string AnimName01;
        public int AnimName01_Start;
        public int AnimName01_End;
        public float AnimName01_Speed;
        public int AnimName01_totalFrame;
        public string AnimName02;
        public int AnimName02_Start;
        public int AnimName02_End;
        public float AnimName02_Speed;
        public int AnimName02_totalFrame;
        public string AnimName03;
        public int AnimName03_Start;
        public int AnimName03_End;
        public float AnimName03_Speed;
        public int AnimName03_totalFrame;
        public string AnimName04;
        public int AnimName04_Start;
        public int AnimName04_End;
        public float AnimName04_Speed;
        public int AnimName04_totalFrame;

        public Vector3 MoveVector;
    }
    
    public class RoleHurtState : RoleAnimState
    {
        public string AnimName01;
        public int AnimName01_Start;
        public int AnimName01_End;
        public float AnimName01_Speed;
        public int AnimName01_totalFrame;
        public string AnimName02;
        public int AnimName02_Start;
        public int AnimName02_End;
        public float AnimName02_Speed;
        public int AnimName02_totalFrame;
        public string AnimName03;
        public int AnimName03_Start;
        public int AnimName03_End;
        public float AnimName03_Speed;
        public int AnimName03_totalFrame;
        public float ForceValue;
        public bool IsFront;
    }
    
    public class RoleDeathState : RoleAnimState
    {
        public string AnimName01;
        public int Start01;
        public int End01;
        public float Speed01;
        public string AnimName02;
        public int Start02;
        public int End02;
        public float Speed02;
    }

    public class RoleJumpState : RoleAnimState
    {
        public string AnimName01;
        public int AnimName01_Start;
        public int AnimName01_End;
        public float AnimName01_Speed;
        public string AnimName01_Tag;
        public string AnimName02;
        public int AnimName02_Start;
        public int AnimName02_End;
        public float AnimName02_Speed;
        public string AnimName02_Tag;
        public string AnimName03;
        public int AnimName03_Start;
        public int AnimName03_End;
        public float AnimName03_Speed;
        public string AnimName03_Tag;
        public string AnimName04;
        public int AnimName04_Start;
        public int AnimName04_End;
        public float AnimName04_Speed;
        public string AnimName04_Tag;
        
    }

    public class RoleDodgeState : RoleAnimState
    {
    }
    
    public class RoleAnimState
    {
        public string StateName;
        public string AnimName;
        public bool Loop;
        public float Speed;
        public float moveSpeed;
        public float NormalMoveSpeed;
        public int Start;
        public int End;
        public string Tag;
        public float MinSpeed;
    }
    
    
    
    
    
    
    
    
    
    
    public class RoleRunState : RoleAnimState
    {
        public string AnimName;
        public float MinSpeed;
    }
    public class RoleTurnBackState : RoleAnimState
    {
        
    }
}