using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Text.RegularExpressions;
using Spine.Unity;
using Tang.FrameEvent;
using ZS;
using Object = UnityEngine.Object;

namespace Tang.Editor
{
    public class SkillEditorWindow : EditorWindow
    {
        [MenuItem("Window/技能编辑器")]
        static void Init()
        {
            SkillEditorWindow window = (SkillEditorWindow)EditorWindow.GetWindow(typeof(SkillEditorWindow));
            window.titleContent = new GUIContent("技能编辑器");
            window.Show();
        }

        private string skillDataFile = "Resources_moved/Scripts/Skill/Skill.json";
        private string skillDataAssetFile = "Assets/Resources_moved/Manager/SkillManagerDataAsset.asset";
        SkillManagerDataAsset skillManagerDataAsset;
        Dictionary<string, SkillData> skillDataDic = new Dictionary<string, SkillData>();
        List<SkillData> skillDataList = new List<SkillData>();

        private SkillData currSkillData;
        private SkillData currSkillChildData;

        Spine.Unity.Editor.SkeletonPreviewEditor previewEditor;
        public Spine.Unity.Editor.SkeletonPreviewEditor PreviewEditor
        {
            get
            {
                previewEditor = previewEditor ?? (Selection.activeObject is SkeletonDataAsset ?
                                    (PreviewEditor = UnityEditor.Editor.CreateEditor(Selection.activeObject, typeof(Spine.Unity.Editor.SkeletonPreviewEditor)) as Spine.Unity.Editor.SkeletonPreviewEditor) : null);
                return previewEditor;
            }

            set { previewEditor = value; }
        }

        Vector2 scrollPosition = Vector2.zero;
        Vector2 listScrollViewPos = Vector2.zero;
        Vector2 editScrollViewPos = Vector2.zero;
        Vector2 baseScrollViewPos = Vector2.zero;

        private Vector3 testSkillOffset = Vector3.zero;
        int usecount = 1;
        float inetal = 0.1f;
        Rect windowRect = new Rect(100, 100, 500, 500);
        Object target;

        GameObject roleTarget;
        private string SearchContext ;
        
        //TreeNode    
        private Dictionary<string,TreeNode> skillTreeNodes = new Dictionary<string, TreeNode>();
        private TreeNode currentNode;
        private int treeIndex = 0;
        

        private ExpandObject _expandObject = new ExpandObject();
        private dynamic _dynamicObject = new MyDynamicObject();
        
        public Object Target
        {
            get { return target ?? (target = Selection.activeObject); }
            set { target = value; }
        }

        private Vector2 scrollVector2 = Vector2.one;
        void OnGUI()
        {
           
            GUIStyle boxStyle = new GUIStyle("box");
            
            var width = position.size.x - boxStyle.border.horizontal;
            var height = position.size.y - boxStyle.border.vertical;

            var innerBoxWidth = width - (boxStyle.padding.horizontal + boxStyle.border.horizontal);
            var innerBoxHeight = height - (boxStyle.padding.vertical + boxStyle.border.vertical);
            
            MyGUIExtend.Instance.ToolbarButton(new Dictionary<string, Action>
            {
                {
                    "读取",loadSkill
                },
                {
                    "保存",saveSkill
                }
            });
            
            baseScrollViewPos = EditorGUILayout.BeginScrollView(baseScrollViewPos);

            using (new EditorGUILayout.VerticalScope()) 
            {
                //路径
                     EditorGUILayout.BeginVertical(boxStyle, GUILayout.Height(innerBoxHeight / 24));
                     using (new EditorGUILayout.HorizontalScope())
                     {
                         EditorGUILayout.LabelField(skillDataFile);
                     }
                     EditorGUILayout.EndVertical();
                //list 内容
                     EditorGUILayout.BeginVertical(boxStyle, GUILayout.Height(innerBoxHeight / 2f));
                     using (new EditorGUILayout.HorizontalScope())
                     {
                         EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(innerBoxWidth / 3),
                             GUILayout.Height(innerBoxHeight / 2));
                         EditorGUILayout.BeginVertical();
                         SearchContext = GUILayout.TextField(SearchContext);
                         listScrollViewPos = EditorGUILayout.BeginScrollView(listScrollViewPos);

                         #region list

                         if (!string.IsNullOrEmpty(SearchContext))
                         {
                             for (int i = 0; i < skillDataList.Count; i++)
                             {
                                 if (Regex.IsMatch(skillDataList[i].id.ToLower(), SearchContext))
                                 {
                                     ListID(i);
                                 }
                             }
                         }
                         else
                         {
                             for (int i = skillDataList.Count - 1; i >= 0; i--)
                             {
                                 EditorGUILayout.BeginHorizontal();
                                 ListID(i);
                                 EditorGUILayout.EndHorizontal();
                             }
                         }

                         #endregion
                         
                         if (MyGUI.Button("+"))
                         {
                             SkillData skillData = new SkillData(new SkillData("Front"),new SkillData("Centre"),new SkillData("Back"));
                             skillData.id = "New";
                             skillData.IsPhasesSkill = true;
                             skillDataList.Add(skillData);
                             CreatTreeNode();
                         }

                         EditorGUILayout.EndScrollView();
                         EditorGUILayout.EndVertical();
                         EditorGUILayout.EndVertical();

                         EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(innerBoxWidth / 6),
                             GUILayout.Height(innerBoxHeight / 2));
                          
                         currentSkillTreeNode(currSkillData,new Rect(innerBoxWidth/3+10,innerBoxHeight / 24+10,innerBoxWidth/6,innerBoxHeight / 2f));
                         GUILayout.Space(50);
                         
                         EditorGUILayout.EndVertical();

                         EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(innerBoxWidth / 2),
                             GUILayout.Height(innerBoxHeight / 2));
                         EditorGUILayout.BeginVertical();
                         editScrollViewPos = EditorGUILayout.BeginScrollView(editScrollViewPos);
                            
                         if (currSkillData.skillDatas.Count>0&& currentNode!= null)
                         {
                             CurrSkillDataInput(currentNode.nodeSkillData);
                         }
                         else
                         {
                             CurrSkillDataInput(currSkillData);    
                         }

                         EditorGUILayout.EndScrollView();
                         EditorGUILayout.EndVertical();
                         EditorGUILayout.EndVertical();
                     }
                     EditorGUILayout.EndVertical();
                //测试   
                     EditorGUILayout.BeginVertical(boxStyle, GUILayout.Height(innerBoxHeight / 12));
                     using (new EditorGUILayout.VerticalScope())
                     {
                         if (Application.isPlaying)
                         {
                             using (new EditorGUILayout.VerticalScope())
                             {
                                 usecount = MyGUI.IntFieldWithTitle("使用次数:", usecount);
                                 inetal = MyGUI.FloatFieldWithTitle("间隔时间:", inetal);
                                 testSkillOffset = MyGUI.Vector3WithTitle("释放位置:", testSkillOffset);
                             }

                             roleTarget = (GameObject) EditorGUILayout.ObjectField(roleTarget, typeof(GameObject), true);

                             if (MyGUI.Button("测试技能"))
                             {
                                 var playerObject = GameObject.Find("Player1");
                                 RoleController roleController = playerObject.GetComponent<RoleController>();
                                 //SkillManager.Instance.UseSkill(currSkillData, playerObject.transform, roleController.RoleData.TeamId, roleController.GetDirection());
                                 EditorCoroutineSequenceRunner.RemoveCoroutine("SkillTest");
                                 EditorCoroutineSequenceRunner.AddCoroutineIfNot("SkillTest",
                                     TestAnimEffect(usecount, inetal, roleController.RoleData.TeamId, playerObject.transform,
                                         testSkillOffset, roleController.GetDirection()));
                             }
                         }
                     }
                     EditorGUILayout.EndVertical();
                //Spine
                     EditorGUILayout.BeginVertical(boxStyle, GUILayout.Height(innerBoxHeight / 4));
                     using (new EditorGUILayout.HorizontalScope())
                     {
                         if (Application.isPlaying == false)
                         {
                             if (!string.IsNullOrEmpty(currSkillData.SkeletonDataAssetPath))
                             {
                                 SkeletonAnimOnGUI(boxStyle, position.width, position.height/3.2f);
                                 panduandanqian();
                                 //var skeletondataasset = (SkeletonDataAsset)AssetDatabase.LoadAssetAtPath(currSkillData.SkeletonDataAssetPath, typeof(SkeletonDataAsset));
                                 //if (skeletondataasset != null)
                                 {

                                     //Selection.activeObject = skeletondataasset;
                                     //PreviewEditor = UnityEditor.Editor.CreateEditor(Selection.activeObject, typeof(Spine.Unity.Editor.SkeletonPreviewEditor)) as Spine.Unity.Editor.SkeletonPreviewEditor;
                                     if (currSkillData.onoffCollider && Selection.activeObject is SkeletonDataAsset)
                                     {
                                         PreviewEditor.AddDrawExtraGuideLineAction(currSkillData.id, (Camera camera) =>
                                         {
                                             Handles.color = Color.green;
                                             Handles.DrawWireCube(currSkillData.colliderCenter, currSkillData.colliderSize);
                                         });
                                     }

                                     if (currSkillData.onoffDamage && Selection.activeObject is SkeletonDataAsset)
                                     {
                                         PreviewEditor.AddDrawExtraGuideLineAction(currSkillData.id + "Damage",
                                             (Camera camera) =>
                                             {
                                                 Handles.color = Color.red;
                                                 Handles.DrawWireCube(currSkillData.DamageColliderCenter,
                                                     currSkillData.DamageColliderSize);
                                             });
                                     }

                                     if (currSkillData.onoffFocus && Selection.activeObject is SkeletonDataAsset)
                                     {
                                         PreviewEditor.AddDrawExtraGuideLineAction(currSkillData.id + "focus",
                                             (Camera camera) =>
                                             {
                                                 Handles.color = Color.red;
                                                 Handles.ArrowHandleCap(0, currSkillData.focuspos, Quaternion.Euler(0, 0, 0),
                                                     1, UnityEngine.EventType.Repaint);
                                             });
                                     }
                                
                                 }
                             }
                         }
                     }
                     EditorGUILayout.EndVertical(); 
            }    
            EditorGUILayout.EndScrollView();

        }
        
       
        private void CurrSkillDataInput(SkillData currSkillData)
        {
            currSkillData.id = MyGUI.TextFieldWithTitle("id", currSkillData.id);
            currSkillData.IsPhasesSkill = EditorGUILayout.ToggleLeft("IsPhasesSkill:", currSkillData.IsPhasesSkill);
            if (currSkillData.IsPhasesSkill)
            {
                GUILayout.Space(10);
                GUILayout.Label("阶段1");
                BaseSkillInput(currSkillData.FrontSkillData);
                AnimSkillInput(currSkillData.FrontSkillData);
                
                GUILayout.Space(20);
                GUILayout.Label("阶段2");
                BaseSkillInput(currSkillData.CentreSkillData);
                DamageSkillInput(currSkillData.CentreSkillData);
                PhysicsSkillInput(currSkillData.CentreSkillData);
                
                GUILayout.Space(20);
                GUILayout.Label("阶段3");
                BaseSkillInput(currSkillData.BackSkillData);
                DamageSkillInput(currSkillData.BackSkillData);
                AnimSkillInput(currSkillData.BackSkillData);
            }
            else
            {
                BaseSkillInput(currSkillData);
                DamageSkillInput(currSkillData);
                PhysicsSkillInput(currSkillData);
            }

        }

        private void BaseSkillInput(SkillData currSkillData)
        {
            #region 基本
                
            MyGUIExtend.Instance.Foldout("基本"+this.currSkillData.id+currSkillData.id,"基本", () =>
            {
                currSkillData.name = MyGUI.TextFieldWithTitle("name", currSkillData.name);
                MonoScript component =
                    (MonoScript) AssetDatabase.LoadAssetAtPath(currSkillData.componentPath, typeof(MonoScript));
                component = (UnityEditor.MonoScript) EditorGUILayout.ObjectField(new GUIContent("脚本"),
                    component, typeof(UnityEditor.MonoScript), true);
                 
                if (component != null)
                {
                    currSkillData.componentPath = AssetDatabase.GetAssetPath(component);
                    currSkillData.componentTypeName =
                        component.GetClass().Namespace + "." + component.GetClass().Name;
                }
                currSkillData.SurvivalTime = MyGUI.FloatFieldWithTitle("存活时间", currSkillData.SurvivalTime);

                // 朝向模式 add by TangJian 2019/4/20 13:24
                currSkillData.SkillOrientationMode =
                    (SkillOrientationMode) MyGUI.EnumPopupWithTitle("朝向模式",
                        currSkillData.SkillOrientationMode);
            });
              
              #endregion

            #region 渲染

            MyGUIExtend.Instance.Foldout( "渲染"+this.currSkillData.id+currSkillData.id,  "渲染", () =>
            {
                currSkillData.rendererType =
                      (RendererType) MyGUI.EnumPopupWithTitle("rander类型:", currSkillData.rendererType);
                  switch (currSkillData.rendererType)
                  {
                      case RendererType.SkeletonAnimator:
                      {
                          Spine.Unity.SkeletonDataAsset SkeletonDataAsset =
                              (Spine.Unity.SkeletonDataAsset) AssetDatabase.LoadAssetAtPath(
                                  currSkillData.SkeletonDataAssetPath, typeof(Spine.Unity.SkeletonDataAsset));
                          UnityEditor.Animations.AnimatorController animatorController =
                              (UnityEditor.Animations.AnimatorController) AssetDatabase.LoadAssetAtPath(
                                  currSkillData.AnimControllerPath,
                                  typeof(UnityEditor.Animations.AnimatorController));
                          animatorController =
                              (UnityEditor.Animations.AnimatorController) EditorGUILayout.ObjectField(
                                  new GUIContent("动画控制器"), animatorController,
                                  typeof(UnityEditor.Animations.AnimatorController), true);
                          SkeletonDataAsset =
                              (Spine.Unity.SkeletonDataAsset) EditorGUILayout.ObjectField(
                                  new GUIContent("Skeleton"), SkeletonDataAsset,
                                  typeof(Spine.Unity.SkeletonDataAsset), true);
                          currSkillData.SkeletonDataAssetPath = AssetDatabase.GetAssetPath(SkeletonDataAsset);
                          currSkillData.AnimControllerPath = AssetDatabase.GetAssetPath(animatorController);
                          if (SkeletonDataAsset != null)
                          {
                              string[] skins =
                                  new string[SkeletonDataAsset.GetSkeletonData(false).Skins.Count];
                              int skinIndex = 0;
                              for (int i = 0; i < skins.Length; i++)
                              {
                                  string skinNameString = SkeletonDataAsset.GetSkeletonData(false).Skins
                                      .Items[i].Name;
                                  skins[i] = skinNameString;
                                  if (skinNameString == currSkillData.SkinName)
                                      skinIndex = i;
                              }

                              skinIndex = EditorGUILayout.Popup("Initial Skin(皮肤)", skinIndex, skins);
                              currSkillData.SkinName = skins[skinIndex];
                          }
                      }
                          break;
                      case RendererType.Sprite:
                      {
                          currSkillData.SpritePath =
                              MyGUI.ObjectField<Sprite>("精灵: ", currSkillData.SpritePath);
                      }
                          break;
                      case RendererType.Skeleton:
                          Spine.Unity.SkeletonDataAsset skeletonDataAsset = AssetDatabase.LoadAssetAtPath<Spine.Unity.SkeletonDataAsset>(currSkillData.SkeletonPath);
                          skeletonDataAsset = (Spine.Unity.SkeletonDataAsset)EditorGUILayout.ObjectField("Skeleton", skeletonDataAsset, typeof(Spine.Unity.SkeletonDataAsset));
                          currSkillData.SkeletonPath = AssetDatabase.GetAssetPath(skeletonDataAsset);
                         
                          if (skeletonDataAsset != null)
                          {
                              if (skeletonDataAsset != null && skeletonDataAsset.GetSkeletonData(false) != null && skeletonDataAsset.GetSkeletonData(false).Animations != null)
                              {
                                  string[] Animations = new string[skeletonDataAsset.GetSkeletonData(false).Animations.Count];
                                  int skinIndex = 0;
                                  for (int i = 0; i < Animations.Length; i++)
                                  {
                                      string skinNameString = skeletonDataAsset.GetSkeletonData(false).Animations.Items[i].Name;
                                      Animations[i] = skinNameString;
                                      if (skinNameString == currSkillData.SkeletonClipName)
                                          skinIndex = i;
                                  }
                                  skinIndex = EditorGUILayout.Popup("Animation(动画片段)", skinIndex, Animations);
                                  currSkillData.SkeletonClipName = Animations[skinIndex];
                              }

                          }
                          break;
                      case RendererType.Anim:
                          currSkillData.AnimName = EditorGUILayout.TextField("AnimName:", currSkillData.AnimName);
                          break;
                     
                      default:
                          Debug.Log("UnKnown:"+currSkillData.rendererType);
                          break;
                  }

                  currSkillData.shadow = MyGUI.ToggleWithTitle("阴影", currSkillData.shadow);
                  if (currSkillData.shadow)
                  {
                      currSkillData.shadowScale =
                          MyGUI.Vector3WithTitle("阴影scale", currSkillData.shadowScale);
                      currSkillData.CutOffDistance =
                          MyGUI.FloatFieldWithTitle("阴影CutOffDistance", currSkillData.CutOffDistance);
                      currSkillData.MaxScaleMultpler = MyGUI.FloatFieldWithTitle("阴影MaxScaleMultpler",
                          currSkillData.MaxScaleMultpler);
                  }
            });
            #endregion
        }
        private void DamageSkillInput(SkillData currSkillData)
        {
            #region 伤害
            
            MyGUIExtend.Instance.Foldout( "伤害"+this.currSkillData.id+currSkillData.id,  "伤害", () =>
            {
                currSkillData.atk = MyGUI.FloatFieldWithTitle("atk", currSkillData.atk);

                currSkillData.onoffDamage = MyGUI.ToggleWithTitle("是否有伤害区域", currSkillData.onoffDamage);
                if (currSkillData.onoffDamage)
                {
                    currSkillData.DamageColliderCenter =
                        MyGUI.Vector3WithTitle("伤害区域Center", currSkillData.DamageColliderCenter);
                    currSkillData.DamageColliderSize =
                        MyGUI.Vector3WithTitle("伤害区域Size", currSkillData.DamageColliderSize);
                    currSkillData.DamageDirectionType =
                        (DamageDirectionType) MyGUI.EnumPopupWithTitle("受击类型:",
                            currSkillData.DamageDirectionType);
                    currSkillData.damageForceType =
                        (DamageForceType) MyGUI.EnumPopupWithTitle("伤害力度类型", currSkillData.damageForceType);
                    currSkillData.poiseCut = MyGUI.FloatFieldWithTitle("削韧", currSkillData.poiseCut);
                }

                currSkillData.Intensity = MyGUI.FloatFieldWithTitle("击退力度", currSkillData.Intensity);
                currSkillData.angleIntensity = MyGUI.Vector3WithTitle("击退角度", currSkillData.angleIntensity);

                currSkillData.HitEffectType =
                    (HitEffectType) MyGUI.EnumPopupWithTitle("攻击效果类型", currSkillData.HitEffectType);
            });
            #endregion
        }
        private void PhysicsSkillInput(SkillData currSkillData)
        {
            #region 变换
            MyGUIExtend.Instance.Foldout( "变换"+this.currSkillData.id+currSkillData.id,  "变换", () =>
            {
                currSkillData.DelayTime =
                    MyGUI.FloatFieldWithTitle("DelayTime", currSkillData.DelayTime);
                currSkillData.pos = MyGUI.Vector3WithTitle("pos", currSkillData.pos);
                currSkillData.rotateSpeed = MyGUI.Vector3WithTitle("RotateSpeed", currSkillData.rotateSpeed);

                currSkillData.IsRandomTrans =
                    MyGUI.ToggleWithTitle("是否随机位置播放", currSkillData.IsRandomTrans);
                if (currSkillData.IsRandomTrans)
                {
                    currSkillData.MinRandomVector3 = MyGUI.Vector3WithTitle("MinPostion", currSkillData.MinRandomVector3);
                    currSkillData.MaxRandomVector3 = MyGUI.Vector3WithTitle("MaxPostion", currSkillData.MaxRandomVector3);
                }
                else
                {
                    currSkillData.pos = MyGUI.Vector3WithTitle("Postion", currSkillData.pos);   
                }
            });
            #endregion
              
            #region 物理
            
            MyGUIExtend.Instance.Foldout("物理"+this.currSkillData.id+currSkillData.id,"物理" , () =>
            {
                currSkillData.withRigidbody = MyGUI.ToggleWithTitle("是否拥有刚体", currSkillData.withRigidbody);
                  currSkillData.useGravity = MyGUI.ToggleWithTitle("是否使用重力", currSkillData.useGravity);
                  if (currSkillData.useGravity)
                  {
                      currSkillData.gravitationalAcceleration =
                          MyGUI.FloatFieldWithTitle("重力加速度: ", currSkillData.gravitationalAcceleration);
                      currSkillData.focuspos = MyGUI.Vector3WithTitle("重心位置", currSkillData.focuspos);
                  }

                  currSkillData.speed = MyGUI.Vector3WithTitle("速度", currSkillData.speed);
                  currSkillData.rotateSpeed = MyGUI.Vector3WithTitle("旋转速度", currSkillData.rotateSpeed);

                  // 碰撞区域 add by TangJian 2019/4/20 11:06
                  currSkillData.onoffCollider = MyGUI.ToggleWithTitle("是否有碰撞区域", currSkillData.onoffCollider);
                  if (currSkillData.onoffCollider)
                  {
                      int Collidertagindex = 0;
                      for (int ign = 0; ign < UnityEditorInternal.InternalEditorUtility.tags.Length; ign++)
                      {
                          if (UnityEditorInternal.InternalEditorUtility.tags[ign] ==
                              currSkillData.CollidertagName)
                          {
                              Collidertagindex = ign;
                          }
                      }

                      int Colliderlayerindex = 0;
                      for (int lagn = 0;
                          lagn < UnityEditorInternal.InternalEditorUtility.layers.Length;
                          lagn++)
                      {
                          if (UnityEditorInternal.InternalEditorUtility.layers[lagn] ==
                              currSkillData.ColliderlayerName)
                          {
                              Colliderlayerindex = lagn;
                          }
                      }

                      Collidertagindex = EditorGUILayout.Popup("碰撞体Tag", Collidertagindex,
                          UnityEditorInternal.InternalEditorUtility.tags);
                      currSkillData.CollidertagName =
                          UnityEditorInternal.InternalEditorUtility.tags[Collidertagindex];
                      Colliderlayerindex = EditorGUILayout.Popup("碰撞体Layer", Colliderlayerindex,
                          UnityEditorInternal.InternalEditorUtility.layers);
                      currSkillData.ColliderlayerName =
                          UnityEditorInternal.InternalEditorUtility.layers[Colliderlayerindex];
                      currSkillData.colliderCenter =
                          MyGUI.Vector3WithTitle("碰撞体Center", currSkillData.colliderCenter);
                      currSkillData.colliderSize =
                          MyGUI.Vector3WithTitle("碰撞体Size", currSkillData.colliderSize);
                  }

                  currSkillData.onoffTirgger = MyGUI.ToggleWithTitle("是否有触发器区域", currSkillData.onoffTirgger);
                  if (currSkillData.onoffTirgger)
                  {
                      int tagindex = 0;
                      for (int ign = 0; ign < UnityEditorInternal.InternalEditorUtility.tags.Length; ign++)
                      {
                          if (UnityEditorInternal.InternalEditorUtility.tags[ign] == currSkillData.tagName)
                          {
                              tagindex = ign;
                          }
                      }

                      int layerindex = 0;
                      for (int lagn = 0;
                          lagn < UnityEditorInternal.InternalEditorUtility.layers.Length;
                          lagn++)
                      {
                          if (UnityEditorInternal.InternalEditorUtility.layers[lagn] ==
                              currSkillData.layerName)
                          {
                              layerindex = lagn;
                          }
                      }

                      tagindex = EditorGUILayout.Popup("触发器Tag", tagindex,
                          UnityEditorInternal.InternalEditorUtility.tags);
                      currSkillData.tagName = UnityEditorInternal.InternalEditorUtility.tags[tagindex];
                      layerindex = EditorGUILayout.Popup("触发器Layer", layerindex,
                          UnityEditorInternal.InternalEditorUtility.layers);
                      currSkillData.layerName = UnityEditorInternal.InternalEditorUtility.layers[layerindex];
                      currSkillData.triggerMode =
                          (TriggerMode) MyGUI.EnumPopupWithTitle("触发模式", currSkillData.triggerMode);
                      currSkillData.TirggercolliderCenter = MyGUI.Vector3WithTitle("触发器碰撞体Center",
                          currSkillData.TirggercolliderCenter);
                      currSkillData.TirggercolliderSize =
                          MyGUI.Vector3WithTitle("触发器碰撞体Size", currSkillData.TirggercolliderSize);
                  }
            });
            #endregion
              
            #region 子技能处理
                
            MyGUIExtend.Instance.Foldout("子技能处理"+this.currSkillData.id+currSkillData.id,"子技能处理", () =>
            {
                currSkillData.type =
                    (SkillGroupType) MyGUI.EnumPopupWithTitle("类型", currSkillData.type);
                currSkillData.parentType = (FrameEventInfo.ParentType)MyGUI.EnumPopupWithTitle("跟随类型", currSkillData.parentType);
                currSkillData.IsMorePlay = MyGUI.ToggleWithTitle("多次释放", currSkillData.IsMorePlay);
                if (currSkillData.IsMorePlay)
                {
                    currSkillData.DurationTime =
                        MyGUI.FloatFieldWithTitle("持续时长", currSkillData.DurationTime);
                    currSkillData.PlayCount =
                        MyGUI.IntFieldWithTitle("数量（播放次数）", currSkillData.PlayCount);
                }
            });
            #endregion
        }

        private void AnimSkillInput(SkillData currSkillData)
        {
            #region 其他
            MyGUIExtend.Instance.Foldout("其他"+this.currSkillData.id+currSkillData.id, "其他" , () =>
            {
                currSkillData.AnimTime = EditorGUILayout.FloatField("动画播放总时长:",currSkillData.AnimTime);
                  
                currSkillData.BeginAlpha = EditorGUILayout.Slider("初始透明度:",currSkillData.BeginAlpha, 0, 1);
                currSkillData.AnimSelectAlphaTime = EditorGUILayout.Slider("选择时间点:",currSkillData.AnimSelectAlphaTime, 0,currSkillData.AnimTime);
                currSkillData.SelectAlpha = EditorGUILayout.Slider("时间点透明度:",currSkillData.SelectAlpha, 0, 1);
                GUILayout.Space(20);
                currSkillData.BeginScale = EditorGUILayout.Slider("初始缩放值:",currSkillData.BeginScale, 0, 1);
                currSkillData.AnimSelectScaleTime = EditorGUILayout.Slider("选择时间点:",currSkillData.AnimSelectScaleTime, 0,currSkillData.AnimTime);
                currSkillData.SelectScale = EditorGUILayout.Slider("时间点缩放值:",currSkillData.SelectScale, 0, 1);   
            });
            #endregion  
        }
        
        private void ListID(int i)
        {
            int Index = MyGUIExtend.Instance.ListSingleButton("SkillEditor", skillDataList[i].id, i, (() =>
            {
                currSkillData = skillDataList[i];
                Selection.activeObject = (Spine.Unity.SkeletonDataAsset)AssetDatabase.LoadAssetAtPath(currSkillData.SkeletonDataAssetPath, typeof(Spine.Unity.SkeletonDataAsset));
                PreviewEditor = UnityEditor.Editor.CreateEditor(Selection.activeObject, typeof(Spine.Unity.Editor.SkeletonPreviewEditor)) as Spine.Unity.Editor.SkeletonPreviewEditor;                
            }));
                            
            MyGUIExtend.Instance.Mouse_RightDrop(new Dictionary<string, Action>
            {
                {
                    "删除",() => skillDataList.RemoveAt(Index)
                },
                {
                    "复制", () =>
                    {
                        var skillData = Tools.Json2Obj<SkillData>(Tools.Obj2Json(skillDataList[Index], true));
                        skillData.id = skillData.id + "_Copy";
                        skillDataList.Add(skillData);
                        CreatTreeNode();
                    }
                }
            });
        }
    
        private List<TreeNode> treeNodes = new  List<TreeNode>();
        private void CreatTreeNode()
        {
            ;foreach (var ItemSkill in skillDataList)
            {
                var itemRoot = TreeNode.Instance.CreatSkillTree(ItemSkill);
                treeNodes.Add(itemRoot);
            }
        }

        private void currentSkillTreeNode(SkillData currentSkillData,Rect rect)
        {
            var root = treeNodes.Find(item => item.nodeSkillData == currentSkillData);
            treeIndex = 0;
            DrawFileTree (root, 0,rect);
        }
        
        
        bool IsRepair = false;

        private void DrawFileTree(TreeNode node, int level, Rect rect)
        {
            if (node == null)
            {
                return;
            }

            GUIStyle style = new GUIStyle("Label");
            style.normal.background = null;
            if (node == currentNode)
            {
                style.normal.textColor = Color.cyan;
            }

            Rect BaseRect = new Rect(5 + 5 * level + rect.x, 5 + 20 * treeIndex + rect.y, rect.width - 15 * 2, 20);
            Rect FoldoutRect = new Rect(BaseRect.x, BaseRect.y, 15, BaseRect.height);
            Rect BtnRect = new Rect(BaseRect.x + 15, BaseRect.y, BaseRect.width - 15, BaseRect.height);

            treeIndex++;


            using (new EditorGUILayout.VerticalScope())
            {
                if (node.nodeType == TreeNode.TreeNodeType.Switch)
                {
                    node.isOpen = EditorGUI.Foldout(FoldoutRect, node.isOpen, "", true);
                    if (GUI.Button(BtnRect, node.nodeSkillData.id, style))
                    {
                        currentNode = node;
                    }
                }
                else
                {
                    if (GUI.Button(BtnRect, node.nodeSkillData.id, style))
                    {
                        currentNode = node;
                    }
                }

                DrawDrop(BtnRect);
            }

            if (node.isOpen && node.children != null)
            {
                foreach (var childTree in node.children)
                {
                    DrawFileTree(childTree, level + 1, rect);
                }
            }
            else
            {
                if (!IsRepair) return;
                IsRepair = false;
                CreatTreeNode();
            }
        }
        

        private void DrawDrop(Rect rect)
        {
            MyGUIExtend.Instance.Single_RightDrop(rect,new Dictionary<string, Action>
            {
                {
                    "添加", () =>
                    {
                        IsRepair = true;
                        var newSkill = new SkillData {id = "New"};
                        currentNode.nodeSkillData.skillDatas.Add(newSkill);
                            
                    }
                },
                {
                    "删除", () =>
                    {
                        IsRepair = true;
                        currentNode.parent?.nodeSkillData.skillDatas.Remove(currentNode.nodeSkillData);
                    }
                }
            });
        }
        
       
        
        void panduandanqian()
        {
            if (Selection.activeObject == (Spine.Unity.SkeletonDataAsset)AssetDatabase.LoadAssetAtPath(currSkillData.SkeletonDataAssetPath, typeof(Spine.Unity.SkeletonDataAsset)))
            {

            }
            else
            {
                Selection.activeObject = (Spine.Unity.SkeletonDataAsset)AssetDatabase.LoadAssetAtPath(currSkillData.SkeletonDataAssetPath, typeof(Spine.Unity.SkeletonDataAsset));
            }

        }

        void OnEnable()
        {
            title = "技能编辑器";
            loadSkill();
            CreatTreeNode();

        }
        void OnDisable()
        {
            if(PreviewEditor)
                PreviewEditor.ClearDrawExtraGuideLineActions();
        }

        void loadSkill()
        {
            //string jsonString = Tools.ReadStringFromFile(Application.dataPath + "/" + skillDataFile);
            skillDataDic = Tools.LoadOneData<SkillData>(Application.dataPath + "/" + "Resources_moved/Scripts/Skill/SkillDataList");

            skillDataList = skillDataDic.Values.ToList();
            currSkillData = skillDataList[0];

            skillManagerDataAsset = AssetDatabase.LoadAssetAtPath<SkillManagerDataAsset>(skillDataAssetFile);
            if (skillManagerDataAsset == null)
            {
                skillManagerDataAsset = ScriptableObject.CreateInstance<SkillManagerDataAsset>();
                AssetDatabase.CreateAsset(skillManagerDataAsset, skillDataAssetFile);
            }
        }

        
        public System.Collections.IEnumerator TestAnimEffect(int times, float interval, string TeamId, Transform transform, Vector3 pos, Direction direction)
        {
            //var playerObject = GameObject.Find("Player1");

            //RoleController roleController = playerObject.GetComponent<RoleController>();

            for (int i = 0; i < times; i++)
            {
                ISkillController skillController = SkillManager.Instance.UseSkill(currSkillData, transform, TeamId, direction, pos);

                if (roleTarget == null)
                {
                    var roleTranformList = transform.parent.GetChidrenLayer("Role");
                    if (roleTranformList != null && roleTranformList.Count > 1)
                    {
                        roleTarget = roleTranformList[0].gameObject;
                        if (roleTarget.tag == "Player1")
                        {
                            roleTarget = roleTranformList[1].gameObject;
                        }
                    }
                }

                if (roleTarget != null)
                {
                    skillController.FlyTo(roleTarget.transform.position);
                }
                float time = 0;
                while (time < interval)
                {
                    time += Time.deltaTime;
                    yield return 0;
                }
            }

            yield return 0;
        }
        void SkeletonAnimOnGUI(GUIStyle boxStyle, System.Single innerBoxWidth, float height)
        {
            var Target = Selection.activeObject;
            EditorGUILayout.BeginHorizontal();
            if (Target is SkeletonDataAsset && Target == Selection.activeObject)
            {
                // 动画选择列表 add by TangJian 2018/8/24 16:05
                {
                    EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(((innerBoxWidth / 3) * 2 - 32) * 0.3f), GUILayout.ExpandHeight(true));

                    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                    // PreviewEditor.OnInspectorGUI ();
                    EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
                    PreviewEditor.DrawAnimationList();
                    // EditorGUILayout.Space();
                    //PreviewEditor.DrawSlotList();
                    // EditorGUILayout.Space();
                    // PreviewEditor.DrawUnityTools();
                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.EndVertical();
                }

                // 动画播放界面 add by TangJian 2018/8/24 16:05
                {
                    EditorGUILayout.BeginVertical(boxStyle);

                    if (PreviewEditor.HasPreviewGUI())
                    {
                        Rect r = GUILayoutUtility.GetRect(((innerBoxWidth / 3) * 2 - 32) * 0.7f, height);
                        GUIStyle style = new GUIStyle("PreBackground");
                        PreviewEditor.OnInteractivePreviewGUI(r, style);
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
      
        void Update()
        {
            if (mouseOverWindow) // 如果鼠标在窗口上, 则重绘界面 add by TangJian 2017/09/28 21:57:44
            {
                base.Repaint();
            }
        }
        
        void saveSkill()
        {
            skillDataDic = skillDataList.ToDictionary(item => item.id, item => item);

            //string jsonString = Tools.Obj2Json(skillDataDic, true);
            //Debug.Log("jsonString = " + jsonString);
            //Tools.WriteStringFromFile(Application.dataPath + "/" + skillDataFile, jsonString);
            Tools.SaveOneData<SkillData>(skillDataDic, Application.dataPath + "/" + "Resources_moved/Scripts/Skill/SkillDataList");

            skillManagerDataAsset.skillListSaveDatas = skillDataList;
            //skillManagerDataAsset.skillListSaveDatasString = jsonString;

            EditorUtility.SetDirty(skillManagerDataAsset);
            AssetDatabase.SaveAssets();
        }

    }
}