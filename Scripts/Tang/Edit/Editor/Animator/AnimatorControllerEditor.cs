using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using UnityEditor;
using CSScriptLibrary;
using Newtonsoft.Json.Linq;
using Spine.Unity;
using UnityEditor.Animations;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Tang.Editor
{
    [CustomEditor(typeof(AnimatorController))]
    public class AnimatorControllerEditor : UnityEditor.Editor
    {
        TextAsset text;

        private TextAsset jsonText;
        private string excelPath;
        private string JsonPath;

        private string skeletonDataAssetSaveId;
        private SkeletonDataAsset skeletonDataAsset;
        
        public override void OnInspectorGUI()
        {
            {
                skeletonDataAssetSaveId = AssetDatabase.GetAssetPath(target) + "_skeletonDataAssetId";
                string skeletonDataAssetPath = PlayerPrefs.GetString(skeletonDataAssetSaveId);

                skeletonDataAsset = AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(skeletonDataAssetPath);
                skeletonDataAsset = (SkeletonDataAsset)EditorGUILayout.ObjectField("SkeletonDataAsset", skeletonDataAsset, typeof(SkeletonDataAsset));
                
                PlayerPrefs.SetString(skeletonDataAssetSaveId, skeletonDataAsset ? AssetDatabase.GetAssetPath(skeletonDataAsset) : "");
            }

            {
                string excelPathKey = AssetDatabase.GetAssetPath(target) + "_ExcelPath";
                excelPath = PlayerPrefs.GetString(excelPathKey);
                PlayerPrefs.SetString(excelPathKey, EditorGUILayout.TextField("ExcelPath",excelPath));   
            }

            if (MyGUI.Button("更新状态机"))
            {
                string path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(target)); 
                JsonPath = path + "/" + target.name + ".json";
                text = AssetDatabase.LoadAssetAtPath<TextAsset>(JsonPath);
                
                if (text == null)
                {
                    if (MyGUI.Button("生成状态机模板"))
                    {
                        AssetDatabase.CopyAsset("Assets/Scripts/Tang/Edit/Editor/Animator/AnimatorControllerTemplate.json", JsonPath);
                    }
                }
                
                if (text != null)
                {
                    CreateAnimatorByJsonText(text.text);
                }
            }
            
            if (MyGUI.Button("添加帧事件支持"))
            {
                AnimatorController animatorController = target as AnimatorController;
                AnimatorControllerLayer[] animatorControllerLayers = animatorController.layers;
                AnimatorStateMachine animatorStateMachine = animatorControllerLayers[0].stateMachine;
                foreach (var state in animatorStateMachine.states)
                {
                    if (state.state != null)
                    {
                        if (state.state.motion != null)
                        {
                            if (state.state.motion is UnityEditor.Animations.BlendTree)
                            {
                                var blendTree = state.state.motion as UnityEditor.Animations.BlendTree;
                                if (blendTree.children.Length != 0)
                                {
                                    FrameEventBehaviour frameEventBehaviour = state.state.AddStateBehaviour<FrameEventBehaviour>();
                                    Motion motion = blendTree.children[0].motion;
                                    if (motion != null)
                                    {
                                        frameEventBehaviour.animName = motion.name;
                                        frameEventBehaviour.Parameter = blendTree.blendParameter;
                                        frameEventBehaviour.namelist.Clear();
                                        for (int i = 0; i < blendTree.children.Length; i++)
                                        {
                                            UnityEditor.Animations.ChildMotion child = blendTree.children[i];
                                            Motion childmotion = blendTree.children[i].motion;
                                            if (childmotion != null && childmotion.name != null && childmotion.name != "")
                                            {
                                                if (frameEventBehaviour.namelist.Count - 1 < (int)child.threshold)
                                                {
                                                    int count = (int)child.threshold -( frameEventBehaviour.namelist.Count - 1);
                                                    for (int c = 0; c < count; c++)
                                                    {
                                                        frameEventBehaviour.namelist.Add("null");
                                                    }
                                                }
                                                frameEventBehaviour.namelist.Insert((int)child.threshold, childmotion.name);
                                            }

                                        }
                                    }

                                }
                            }
                            else
                            {
                                Motion motion = state.state.motion;

                                FrameEventBehaviour frameEventBehaviour = state.state.AddStateBehaviour<FrameEventBehaviour>();
                                frameEventBehaviour.animName = motion.name;
                            }

                        }
                    }

                }
                
                EditorUtility.SetDirty(animatorController);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            
            if (MyGUI.Button("添加状态通知支持"))
            {
                SupportEventDispatch();
            }
            
            if (MyGUI.Button("copy"))
            {
                AnimatorController oldAnimatorController = target as AnimatorController;
                string oldAnimatorControllerPath = AssetDatabase.GetAssetPath(oldAnimatorController);
                string newAnimatorControllerPath = oldAnimatorControllerPath.Substring(0, oldAnimatorControllerPath.IndexOf("."));
                string newpath = newAnimatorControllerPath + "_new" + ".controller";

                AnimatorController NewAnimatorController = AnimatorController.CreateAnimatorControllerAtPath(newpath);

                var unityAnimationClipTable = new Dictionary<string, Motion>();
                Object[] objects = AssetDatabase.LoadAllAssetRepresentationsAtPath(oldAnimatorControllerPath);
                foreach (var item in objects)
                {
                    //新建AnimationClip
                    if (item is AnimationClip)
                    {
                        var clip = item as AnimationClip;
                        AnimationClip newClip = new AnimationClip
                        {
                            name = clip.name
                        };
                        AssetDatabase.AddObjectToAsset(newClip, NewAnimatorController);
                        newClip.SetCurve("", typeof(GameObject), "dummy", AnimationCurve.Linear(0, 0, clip.length, 0));
                        var settings = AnimationUtility.GetAnimationClipSettings(newClip);
                        settings.stopTime = clip.length;
                        settings.loopTime = clip.isLooping;

                        AnimationUtility.SetAnimationClipSettings(newClip, settings);

                        AnimationUtility.SetAnimationEvents(newClip, clip.events);

                        EditorUtility.SetDirty(newClip);
                        unityAnimationClipTable.Add(clip.name, newClip);
                    }
                    else if (item is UnityEditor.Animations.BlendTree)
                    {
                        var additem = item as UnityEditor.Animations.BlendTree;
                        string additemjson = Tools.Obj2Json<UnityEditor.Animations.BlendTree>(additem);
                        UnityEditor.Animations.BlendTree blendTree = Tools.Json2Obj<UnityEditor.Animations.BlendTree>(additemjson);
                        AssetDatabase.AddObjectToAsset(blendTree, NewAnimatorController);
                        unityAnimationClipTable.Add(blendTree.name, blendTree);
                    }

                }

                //新建AnimationClip保存
                EditorUtility.SetDirty(NewAnimatorController);
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();

                //转移动画AnimationClip引用
                NewAnimatorController.parameters = oldAnimatorController.parameters;

                AnimatorControllerLayer[] oldAnimatorControllerLayers = oldAnimatorController.layers;
                AnimatorStateMachine oldAnimatorStateMachine = oldAnimatorControllerLayers[0].stateMachine;

                AnimatorControllerLayer[] NewAnimatorControllerLayers = NewAnimatorController.layers;
                AnimatorStateMachine NewAnimatorStateMachine = NewAnimatorControllerLayers[0].stateMachine;

                foreach (var oldChildenstate in oldAnimatorStateMachine.states)
                {
                    if (oldChildenstate.state != null)
                    {
                        var oldChildAnimatorState = oldChildenstate.state;
                        var oldStatemotion = oldAnimatorController.GetStateEffectiveMotion(oldChildAnimatorState, 0);
                        UnityEditor.Animations.AnimatorState newState = NewAnimatorStateMachine.AddState(oldChildAnimatorState.name, oldChildenstate.position);
                        newState.name = oldChildAnimatorState.name;
                        newState.tag = oldChildAnimatorState.tag;

                        if (oldStatemotion != null)
                        {
                            if (unityAnimationClipTable.ContainsKey(oldStatemotion.name))
                            {
                                var Motion = unityAnimationClipTable[oldStatemotion.name];
                                NewAnimatorController.SetStateEffectiveMotion(newState, Motion);
                                //NewAnimatorStateMachine.AddState(newState, oldChildenstate.position);
                            }
                        }
                    }
                }

                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();

                foreach (var oldChildenstate in oldAnimatorStateMachine.states)
                {
                    if (oldChildenstate.state != null)
                    {
                        var oldChildAnimatorState = oldChildenstate.state;
                        UnityEditor.Animations.AnimatorState newState = NewAnimatorStateMachine.GetState(oldChildAnimatorState.name);
                        if (newState != null)
                        {
                            foreach (var oldTransition in oldChildAnimatorState.transitions)
                            {
                                UnityEditor.Animations.AnimatorState tostate = NewAnimatorStateMachine.GetState(oldTransition.destinationState.name);
                                var transition = newState.AddTransition(tostate);
                                // transition.interruptionSource = UnityEditor.Animations.TransitionInterruptionSource.Destination;
                                transition.interruptionSource = oldTransition.interruptionSource;// UnityEditor.Animations.TransitionInterruptionSource.Source;
                                transition.hasExitTime = oldTransition.hasExitTime;
                                transition.exitTime = oldTransition.exitTime;
                                transition.duration = oldTransition.duration;
                                transition.offset = oldTransition.offset;

                                foreach (var conditionConfig in oldTransition.conditions)
                                {
                                    transition.AddCondition(conditionConfig.mode, conditionConfig.threshold, conditionConfig.parameter);
                                }
                            }

                            //newState.AddStateMachineBehaviour<RoleBaseStateMachineBehaviour>();

                            foreach (var item in oldChildAnimatorState.behaviours)
                            {
                                //string sd = item.GetType().FullName;
                                //Type sda = Type.GetType(item.GetType().FullName);
                                //if (sda == null)
                                //{
                                //    sda = Type.GetType(item.GetType().Namespace + "." + item.GetType().FullName);
                                //}

                                newState.AddStateMachineBehaviour(item.GetType());

                            }
                        }
                    }
                }
                //EditorUtility.SetDirty(NewAnimatorController);
                //AssetDatabase.SaveAssets();

                EditorUtility.SetDirty(NewAnimatorController);
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();


                System.IO.File.Delete(oldAnimatorControllerPath);
                System.IO.File.Move(newpath, oldAnimatorControllerPath);
            }

            if (MyGUI.Button("移除多余clip"))
            {
                AnimatorController animatorController = target as AnimatorController;
                AnimatorControllerLayer[] animatorControllerLayers = animatorController.layers;
                AnimatorStateMachine animatorStateMachine = animatorControllerLayers[0].stateMachine;
                
                var objList = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(animatorController));
                
                Dictionary<int, bool> tmp = new Dictionary<int, bool>();
                
                foreach (var item in objList)
                {
                    if (item != null)
                    {
                        if (tmp.ContainsKey(item.GetInstanceID()))
                        {
                            DestroyImmediate(item, true);
                        }
                        else
                        {
                            tmp.Add(item.GetInstanceID(), true);
                        }
                    }
                }
                
                foreach (var item in objList)
                {
                    if (item != null)
                    {
                        bool needRemove = true;
                        
                        foreach (var state in animatorStateMachine.states)
                        {
                            if (state.state != null && state.state.motion != null && state.state.motion.Equals(item))
                            {
                                needRemove = false;
                            }
                        }
                        
                        if(needRemove)
                            DestroyImmediate(item, true);
                    }
                }
                
                EditorUtility.SetDirty(animatorController);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            
            if (MyGUI.Button("新的状态机生成"))
            {
                AnimatorCreator animatorCreator = new AnimatorCreator(AnimatorLoadExcel.GetJObjectFromExcel(excelPath).ToObject<RoleAnimator>());
                
                try
                {
//                    skeletonDataAsset.RoleAnimator = animatorCreator.RoleAnimator;
//                    skeletonDataAsset.RoleAnimatorString = Tools.Obj2Json(animatorCreator.RoleAnimator);

                    

                    AnimatorController animatorController = target as AnimatorController;
                

                    List<KeyValue<string, string>> RoleAnimatorStringList = null;

                    try
                    {
                        RoleAnimatorStringList = JObject.Parse(skeletonDataAsset.RoleAnimatorString)
                            .ToObject<List<KeyValue<string, string>>>();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    if (RoleAnimatorStringList == null)
                    {
                        RoleAnimatorStringList = new List<KeyValue<string, string>>();
                    }

                    RoleAnimatorStringList.RemoveAll(pair => pair.Key == animatorController.name);
                    RoleAnimatorStringList.Add(new KeyValue<string, string>(animatorController.name, Tools.Obj2Json(animatorCreator.RoleAnimator)));

                    skeletonDataAsset.RoleAnimatorString = Tools.Obj2Json(RoleAnimatorStringList);
                    
                    animatorCreator.Create();
                    CreateAnimatorByConfig(animatorCreator.GetConfig());
                }
                catch (InvalidOperationException elements)
                {
                    Debug.Log(elements);
                }
            }
            
            DrawDefaultInspector();
        }
        
        
        void CreateAnimatorByJsonText(string text)
        {
            if (string.IsNullOrEmpty(text) == false)
            {
                string animatorControllerConfigJsonString = text;
                AnimatorControllerConfig animatorControllerConfig =
                    LoadAnimatorConfigJson(animatorControllerConfigJsonString);
                Debug.Log("animatorControllerConfigJsonString = " + animatorControllerConfigJsonString);

                CreateAnimatorByConfig(animatorControllerConfig);
            }
        }

        void CreateAnimatorByConfig(AnimatorControllerConfig animatorControllerConfig)
        {
            if (animatorControllerConfig != null)
            {
                Debug.Log("target = " + target);

                Undo.RecordObject(target, "animatorController"); // 支持撤销 add by TangJian 2018/04/16 15:41:34

                AnimatorController animatorController = target as AnimatorController;

                AnimationClip[] animationClips = animatorController.animationClips;
                AnimatorControllerLayer[] animatorControllerLayers = animatorController.layers;
                AnimatorStateMachine animatorStateMachine = animatorControllerLayers[0].stateMachine;
            
                string animatorControllerPath = AssetDatabase.GetAssetPath(animatorController);
                Object[] clips = AssetDatabase.LoadAllAssetRepresentationsAtPath(animatorControllerPath);
                
                animatorController.parameters = null;
                foreach (var parameterConfig in animatorControllerConfig.parameters)
                {
                    animatorController.AddParameter(parameterConfig.name, parameterConfig.type);
                }

                // 添加所有状态 add by TangJian 2018/04/17 20:08:41                
                foreach (var stateConfig in animatorControllerConfig.states)
                {
                    var state = animatorStateMachine.AddStateEx(stateConfig.name);
                    state.behaviours = null;
                    state.transitions = null; // 清空所有状态 add by TangJian 2018/04/17 20:45:08
                    state.tag = stateConfig.tag;
                    state.speed = stateConfig.Speed;


                    CountsThrow(clips,stateConfig.animName);
                    state.motion = clips.First(o => o.name == stateConfig.animName) as Motion;
                    if (state.motion is AnimationClip animationClip)
                    {
                        AnimationClipSettings animationClipSettings =
                            AnimationUtility.GetAnimationClipSettings(animationClip);
                        animationClipSettings.loopTime = stateConfig.loop;
                        AnimationUtility.SetAnimationClipSettings(animationClip, animationClipSettings);
                    }

                    
                    
                    // 添加BlendTree
                    if (stateConfig.blendTree != null)
                    {
                        if (state.motion is UnityEditor.Animations.BlendTree)
                        {
                            UnityEditor.Animations.BlendTree blendTree = state.motion as UnityEditor.Animations.BlendTree;

                            // BlendTree属性设置 add by TangJian 2018/9/18 18:06
                            blendTree.name = "blendTree_" + state.name;
                            blendTree.blendParameter = stateConfig.blendTree.blendParameter;
                            blendTree.useAutomaticThresholds = stateConfig.blendTree.useAutomaticThresholds;

                            List<UnityEditor.Animations.ChildMotion> addChildMotionList = new List<UnityEditor.Animations.ChildMotion>();
                            foreach (var childMotionConfig in stateConfig.blendTree.childMotions)
                            {
                                UnityEditor.Animations.ChildMotion childMotion = new UnityEditor.Animations.ChildMotion();
                                childMotion.threshold = childMotionConfig.threshold;
                                childMotion.timeScale = childMotionConfig.timeScale;
                                addChildMotionList.Add(childMotion);
                            }


                            var oldBlendTree = state.motion as UnityEditor.Animations.BlendTree;

                            var oldChildren = oldBlendTree.children;

                            var newChildren = new List<UnityEditor.Animations.ChildMotion>();

                            // 更新oldChildren add by TangJian 2018/9/18 18:07
                            for (int i = 0; i < oldChildren.Length; i++)
                            {
                                if (i < addChildMotionList.Count)
                                {
                                    oldChildren[i].threshold = addChildMotionList[i].threshold;
                                    oldChildren[i].timeScale = addChildMotionList[i].timeScale;
                                }
                            }

                            // 添加oldChildren中的motion到newChildren add by TangJian 2018/9/18 18:08
                            foreach (var item in oldChildren)
                            {
                                newChildren.Add(item);
                            }

                            // 添加剩余addChildMotionList中的motion到newChildren add by TangJian 2018/9/18 18:08
                            if (blendTree.children.Length > oldChildren.Length)
                            {
                                for (int i = oldChildren.Length; i < blendTree.children.Length; i++)
                                {
                                    newChildren.Add(blendTree.children[i]);
                                }
                            }


                            blendTree.children = newChildren.ToArray();

                            if (stateConfig.blendTree.useAutomaticThresholds)
                            {
                                if (stateConfig.blendTree.minThreshold > int.MinValue && stateConfig.blendTree.maxThreshold > int.MinValue)
                                {
                                    blendTree.minThreshold = stateConfig.blendTree.minThreshold;
                                    blendTree.maxThreshold = stateConfig.blendTree.maxThreshold;
                                }
                                else
                                {
                                    blendTree.minThreshold = 0;
                                    blendTree.maxThreshold = blendTree.children.Length - 1;
                                }
                            }

                            state.motion = blendTree;
                        }
                        else
                        {
                            state.motion = GetBlendTreeToMotion(animatorController, "blendTree_" + state.name,
                                stateConfig.blendTree);
                        }
                    }


                    if (stateConfig.script != null)
                    {
                        if (stateConfig.script == "RoleHide")
                        {
                            state.AddStateMachineBehaviour<RoleHideStateMachineBehaviour>();
                        }
                    }
                }

                // 添加额外的跳转 add by TangJian 2018/04/17 20:20:52
                foreach (var transitionConfig in animatorControllerConfig.transitions)
                {
                    if (transitionConfig.sourceAnimName == "Any")
                    {
                        foreach (var stateConfig in animatorControllerConfig.states)
                        {
                            var fromState = animatorStateMachine.GetState(stateConfig.name);
                            var toState = animatorStateMachine.GetState(transitionConfig.destinationAnimName);
                            if (fromState != null && toState != null)
                                fromState.AddTransitionEx(toState, transitionConfig);
                        }
                    }
                    else
                    {
                        var fromState = animatorStateMachine.GetState(transitionConfig.sourceAnimName);
                        var toState = animatorStateMachine.GetState(transitionConfig.destinationAnimName);

                        if (fromState != null && toState != null)
                            fromState.AddTransitionEx(toState, transitionConfig);
                    }
                }

                // 添加状态的跳转 add by TangJian 2018/04/17 20:11:08
                foreach (var stateConfig in animatorControllerConfig.states)
                {
                    // 添加状态的跳转 add by TangJian 2018/04/17 20:11:39
                    foreach (var transitionConfig in stateConfig.transitions)
                    {
                        var fromState = animatorStateMachine.GetState(stateConfig.name);
                        var toState = animatorStateMachine.GetState(transitionConfig.destinationAnimName);

                        if (fromState != null && toState != null)
                            fromState.AddTransitionEx(toState, transitionConfig);
                    }
                }

                // 添加脚本 add by TangJian 2018/04/17 21:16:02 
                foreach (var stateConfig in animatorControllerConfig.states)
                {
                    var state = animatorStateMachine.GetState(stateConfig.name);


                    RoleBehaviour roleBehaviour = state.AddStateMachineBehaviour<RoleBehaviour>();
                            
                    // 限制状态可以执行到的时间 add by TangJian 2019/5/14 10:35
                    roleBehaviour.StateDuration = stateConfig.duration;
                            
                    if (state.motion != null)
                    {
                        if (state.motion is UnityEditor.Animations.BlendTree)
                        {
                            var blendTree = state.motion as UnityEditor.Animations.BlendTree;
                            if (blendTree.children.Length != 0)
                            {
                                FrameEventBehaviour frameEventBehaviour = state.AddStateMachineBehaviour<FrameEventBehaviour>();
                                Motion motion = blendTree.children[0].motion;
                                if (motion != null)
                                {
                                    frameEventBehaviour.animName = motion.name;
                                    frameEventBehaviour.Parameter = blendTree.blendParameter;
                                    frameEventBehaviour.namelist.Clear();
                                    frameEventBehaviour.beginTime = stateConfig.beginTime;
                                    frameEventBehaviour.endTime = stateConfig.endTime;
                                    for (int i = 0; i < blendTree.children.Length; i++)
                                    {
                                        UnityEditor.Animations.ChildMotion child = blendTree.children[i];
                                        Motion childmotion = blendTree.children[i].motion;
                                        if (childmotion != null && string.IsNullOrWhiteSpace(childmotion.name) == false)
                                        {
//                                          
                                            frameEventBehaviour.namelist.Insert(i, childmotion.name);
                                            frameEventBehaviour.floatList.Insert(i, child.threshold);
                                        }

                                    }
                                }

                            }

                        }
                        else
                        {
                            FrameEventBehaviour frameEventBehaviour = state.AddStateMachineBehaviour<FrameEventBehaviour>();
                            Motion motion = state.motion;
                            frameEventBehaviour.animName = motion.name;

                            if (motion is AnimationClip animationClip)
                            {
                                frameEventBehaviour.beginTime = stateConfig.beginTime / animationClip.length;
                                frameEventBehaviour.endTime = stateConfig.endTime / animationClip.length;
                            }
                        }

                    }

                    state.AddStateMachineBehaviour<HumanMoveXYByAnimBehaviour>();

                    if (state.tag == "attack")
                    {
                        state.AddStateMachineBehaviour<HumanMainHandAttackBehaviour>();
                    }

                    if (state.name == "jump_1")
                    {
                        state.AddStateMachineBehaviour<RolePreJumpBehaviour>();
                    }
                    
                    if (state.name == "JumpPre")
                    {
                        state.AddStateMachineBehaviour<RolePreJumpBehaviour>();
                    }

                    if (state.tag == "hurtHold")
                    {
                        state.AddStateMachineBehaviour<RoleHurtHoldStateMachineBehaviour>();
                    }

                    if (stateConfig.behaviours != null)
                        foreach (var item in stateConfig.behaviours)
                        {
                            MethodDelegate<bool> func = CSScript.CreateFunc<bool>(
                                @"
                                        using Tang;
                                        bool Sum(UnityEditor.Animations.AnimatorState state)
                                        {
                                            state.AddStateMachineBehaviour<" + item + @" > ();
                                            return true;
                                        }");

                            func(state);
                        }
                }

                // 记录未使用的状态 add by TangJian 2018/04/19 14:36:28
                List<UnityEditor.Animations.AnimatorState> unuseAnimatorStates = new List<UnityEditor.Animations.AnimatorState>();
                foreach (var state in animatorStateMachine.states)
                {
                    int index = animatorControllerConfig.states.FindIndex((AnimatorState a) =>
                    {
                        return a.name == state.state.name;
                    });
                    if (index < 0)
                    {
                        unuseAnimatorStates.Add(state.state);
                    }
                }

                // 移除未使用的状态 add by TangJian 2018/04/19 14:38:34
                foreach (var state in unuseAnimatorStates)
                {
                    animatorStateMachine.RemoveState(state);
                }
                
                
                
                // 整理状态机位置 add by TangJian 2018/04/19 14:40:00
                ChildAnimatorState[] childAnimatorStates = animatorStateMachine.states;
                int cols = 3;
                for (int i = 0; i < childAnimatorStates.Length; i++)
                {
                    float x = (i % cols) * 204;
                    float y = (i / cols) * 50;

                    childAnimatorStates[i].position = new Vector3(x, y, 0);
                }
                animatorStateMachine.states = childAnimatorStates;
                

                // 保存生成的状态机 add by TangJian 2019/1/30 17:21
                EditorUtility.SetDirty(animatorController);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            SupportEventDispatch();
        }

        void CountsThrow(Object[] objects, string var)
        {
            foreach (var clip in objects)
            {
                if (clip.name == var)
                {
                    break;
                }
                        
                if (clip == objects[objects.Length-1])
                {
                    throw new Exception(var + "不存在！");
                    return;
                }
            }
        }

        void SupportEventDispatch()
        {
            AnimatorController animatorController = target as AnimatorController;
            AnimatorControllerLayer[] animatorControllerLayers = animatorController.layers;
            AnimatorStateMachine animatorStateMachine = animatorControllerLayers[0].stateMachine;
            foreach (var state in animatorStateMachine.states)
            {
                if (state.state != null)
                {
                    StateEventTransmitBehaviour stateEventTransmitBehaviour = state.state.AddStateBehaviour<StateEventTransmitBehaviour>();
                    stateEventTransmitBehaviour.StateNawme = state.state.name;
                }
            }
                
            EditorUtility.SetDirty(animatorController);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public UnityEditor.Animations.BlendTree GetBlendTreeToMotion(AnimatorController animatorController, string name, BlendTree blendTreeConfig)
        {
            UnityEditor.Animations.BlendTree blendTree = new UnityEditor.Animations.BlendTree();

            blendTree.name = name;
            
            List<UnityEditor.Animations.ChildMotion> childMotionList = new List<UnityEditor.Animations.ChildMotion>();
            for (int i = 0; i < blendTreeConfig.childMotions.Count; i++)
            {
                var childMotionConfig = blendTreeConfig.childMotions[i];
                if (childMotionConfig.blendTree != null &&
                    string.IsNullOrWhiteSpace(childMotionConfig.blendTree.blendParameter) == false)
                {
                    UnityEditor.Animations.ChildMotion childMotion = new UnityEditor.Animations.ChildMotion();
                    childMotion.threshold = childMotionConfig.threshold;
                    childMotion.timeScale = childMotionConfig.timeScale;
                    childMotion.motion = GetBlendTreeToMotion(animatorController, blendTree.name + "_" + i, childMotionConfig.blendTree);
                    childMotionList.Add(childMotion);
                }
                else
                {
                    UnityEditor.Animations.ChildMotion childMotion = new UnityEditor.Animations.ChildMotion();
                    childMotion.threshold = childMotionConfig.threshold;
                    childMotion.timeScale = childMotionConfig.timeScale;
                    childMotionList.Add(childMotion);
                }
            }

            blendTree.children = childMotionList.ToArray();

            blendTree.blendParameter = blendTreeConfig.blendParameter;
            blendTree.useAutomaticThresholds = blendTreeConfig.useAutomaticThresholds;

            if (blendTreeConfig.useAutomaticThresholds)
            {
                if (blendTreeConfig.minThreshold > int.MinValue && blendTreeConfig.maxThreshold > int.MinValue)
                {
                    blendTree.minThreshold = blendTreeConfig.minThreshold;
                    blendTree.maxThreshold = blendTreeConfig.maxThreshold;
                }
                else
                {
                    blendTree.minThreshold = 0;
                    blendTree.maxThreshold = blendTree.children.Length - 1;
                }
            }
                                    
            if (AssetDatabase.GetAssetPath(animatorController) != "")
            {
                var objList = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(animatorController));

                foreach (var item in objList)
                {
                    if (item != null && blendTree.name == item.name)
                    {
                        DestroyImmediate(item, true);
                    }
                }
                AssetDatabase.AddObjectToAsset(blendTree, AssetDatabase.GetAssetPath(animatorController));
                Debug.Log(AssetDatabase.GetAssetPath(blendTree));
            }
            return blendTree;
        }

        public AnimatorControllerConfig LoadAnimatorConfigJson(string json)
        {
            AnimatorControllerConfig animatorControllerConfig = null;
            Dictionary<string, AnimatorControllerConfig> animatorControllerConfigDic = null;
            try
            {
                animatorControllerConfigDic = Tools.Json2Obj<Dictionary<string, AnimatorControllerConfig>>(json);
            }
            catch (Exception e)
            {
                Debug.Log("animatorControllerConfig 类型不为字典类型:" + e);
            }
            
            if (animatorControllerConfigDic != null)
            {
                animatorControllerConfig = new AnimatorControllerConfig();
                animatorControllerConfig.parameters = new List<AnimatorParameter>();
                animatorControllerConfig.states = new List<AnimatorState>();
                animatorControllerConfig.transitions = new List<AnimatorTransition>();
                
                foreach (var pair in animatorControllerConfigDic)
                {
                    if(pair.Value.parameters != null)
                        foreach (var parameter in pair.Value.parameters)
                        {
                            animatorControllerConfig.parameters.Add(parameter);
                        }
                    
                    if(pair.Value.states != null)
                        foreach (var state in pair.Value.states)
                        {
                            animatorControllerConfig.states.Add(state);
                        }
                    
                    if(pair.Value.transitions != null)
                        foreach (var transition in pair.Value.transitions)
                        {
                            animatorControllerConfig.transitions.Add(transition);
                        }
                }
            }
            else
            {
                animatorControllerConfig = Tools.Json2Obj<AnimatorControllerConfig>(json);
            }
            
            Debug.Assert(animatorControllerConfig != null, "animatorControllerConfig 不能为空");

            return animatorControllerConfig;
        }
    }
}