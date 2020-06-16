using UnityEngine;

namespace Tang.Editor
{
    public static class EditorExtend
    {
        public static UnityEditor.Animations.AnimatorState AddStateEx(this UnityEditor.Animations.AnimatorStateMachine target, string name)
        {
            foreach (var state in target.states)
            {
                if (state.state.name == name)
                {
                    return state.state;
                }
            }
            return target.AddState(name);
        }

        public static UnityEditor.Animations.AnimatorState GetState(this UnityEditor.Animations.AnimatorStateMachine target, string name)
        {
            foreach (var state in target.states)
            {
                if (state.state.name == name)
                {
                    return state.state;
                }
            }
            return null;
        }
        public static T AddStateBehaviour<T>(this UnityEditor.Animations.AnimatorState target) where T : UnityEngine.StateMachineBehaviour
        {
            T FD = null;
            bool onoff = false;
            System.Collections.Generic.List<UnityEngine.StateMachineBehaviour> sdf = new System.Collections.Generic.List<UnityEngine.StateMachineBehaviour>();
            foreach (var item in target.behaviours)
            {
                if (item is T)
                {
                    onoff = true;
                }
                else
                {
                    sdf.Add(item);
                }
            }
            if (onoff)
            {
                target.behaviours = sdf.ToArray();
            }
            else
            {

            }
            FD = target.AddStateMachineBehaviour<T>();
            return FD;
        }

        public static UnityEditor.Animations.AnimatorStateTransition AddTransitionEx(this UnityEditor.Animations.AnimatorState target, UnityEditor.Animations.AnimatorState toState, AnimatorTransition transitionConfig)
        {
            if (
                (transitionConfig.ignoreSourceAnimNames != null
                 && transitionConfig.ignoreSourceAnimNames.Contains(target.name))

                ||

                (transitionConfig.ignoreSourceAnimTags != null
                 && transitionConfig.ignoreSourceAnimTags.Contains(target.tag))
        
            )
            {
                return null;
            }


            if (
                (transitionConfig.containSourceAnimNames != null && transitionConfig.containSourceAnimNames.Contains(target.name) == false)
                
                &&
                
                (transitionConfig.ignoreSourceAnimTags != null && transitionConfig.ignoreSourceAnimTags.Contains(target.tag) == false)
                
            )
            {
                return null;
            }



            // if (nameCanTransition && tagCanTransition)
            {
                var transition = target.AddTransition(toState);

                // transition.interruptionSource = UnityEditor.Animations.TransitionInterruptionSource.Destination;
                transition.interruptionSource = transitionConfig.interruptionSource;// UnityEditor.Animations.TransitionInterruptionSource.Source;

                transition.hasFixedDuration = true;
                transition.hasExitTime = transitionConfig.hasExitTime;
                transition.exitTime = transitionConfig.exitTime;
                transition.duration = transitionConfig.duration;
                transition.offset = transitionConfig.offset;

                if (transitionConfig.hasFixedDuration == false)
                {
                    if (target.motion is AnimationClip targetClip)
                    {
                        transition.exitTime /= targetClip.length;
                        transition.duration /= targetClip.length;
                        transition.offset /= targetClip.length;
                    }
                }

                // transition.exitTime = 1;
                // transition.duration = 0;

                // if (transitionConfig.conditions != null)
                // {

                if (transitionConfig.conditions != null)
                {
                    foreach (var conditionConfig in transitionConfig.conditions)
                    {
                        transition.AddCondition(conditionConfig.animatorConditionMode, conditionConfig.threshold, conditionConfig.parameter);
                    }    
                }
                
                // }

                return transition;
            }

            // return null;
        }


       
    }
}