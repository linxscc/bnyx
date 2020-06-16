using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using tian;
using ZS;


namespace Tang.Editor
{
    public class BoxEditorWindow : EditorWindow
    {
        [MenuItem("Window/盒子编辑器")]
        static void Init()
        {
            BoxEditorWindow window = (BoxEditorWindow)EditorWindow.GetWindow(typeof(BoxEditorWindow));
            window.Show();
        }
        // Texture2D img;
        bool _usePathorImg = true;
        private string _roleDataFile = "Resources_moved/Scripts/box/plembox.json";
        private string _dataFile = "Resources_moved/Scripts/box/box.json";
        private string _prefabPath = "Assets/Resources_moved/Prefabs/Placement/Generate";

        private string ExcelPath;
        //bool firstbool = false;
        string newtagname;
        string currtagname;
        string newtagnae = "";

        Dictionary<string, tian.PlacementConfig> _placementConfigDic = new Dictionary<string, tian.PlacementConfig>();
        Dictionary<string, tian.PlacementData> _dataConfigDic = new Dictionary<string, tian.PlacementData>();
        List<tian.PlacementConfig> _placementConfigList = new List<tian.PlacementConfig>();
        List<string> taglist = new List<string>();
        List<int> showIndexList = new List<int>();
        tian.PlacementConfig _currPlacementConfig;

        Vector2 _listScrollViewPos = Vector2.zero;
        Vector2 _editScrollViewPos = Vector2.zero;
        Vector2 onelistViewPos = Vector2.zero;
        /*
                Rect _windowRect = new Rect(100, 100, 500, 500);
        */

        float _scalefloat = 1;
        
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
                    "读取", LoadRole
                },
                {
                    "All", () =>
                    {
                        currtagname = "";
                        showIndexList.Clear();
                        for (int ix = 0; ix < _placementConfigList.Count; ix++)
                        {
                            showIndexList.Add(ix);
                        }
                    }
                },
                {
                    "仅保存数据", saveData
                },
                {
                    "保存数据仅生成当前选中预制体", () =>
                    {
                        saveData();
                        Createprefab();
                    }
                }
            });
            
            
            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(width), GUILayout.Height(height));
            
            MyGUIExtend.Instance.Foldout("BoxEditor","路径信息", () =>
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(_roleDataFile);

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(_prefabPath);

                EditorGUILayout.EndHorizontal();
            
                EditorGUILayout.BeginHorizontal();
                _scalefloat = MyGUI.FloatFieldWithTitle("图片整体缩放大小", _scalefloat);
                EditorGUILayout.EndHorizontal();

            });
            
            // 编辑区域 add by TangJian 2017/11/15 16:28:19
            EditorGUILayout.BeginHorizontal();
            //一级列表
            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width((innerBoxWidth / 2) * (1f / 3f) - 5f), GUILayout.ExpandHeight(true));
            onelistViewPos = EditorGUILayout.BeginScrollView(onelistViewPos);
            for (int e = taglist.Count - 1; e >= 0; e--)
            {
                var tag = taglist[e];
                EditorGUILayout.BeginHorizontal();
                
                int Index = MyGUIExtend.Instance.ListSingleButton("BoxTag",taglist[e],e,(() =>
                {
                    currtagname = tag;
                    showIndexList.Clear();
                    //firstbool = true;
                    foreach (var item in _placementConfigList)
                    {
                        if (item.tagstring == tag)
                        {
                            showIndexList.Add(_placementConfigList.IndexOf(item));
                        }
                    }
                }));
                
                MyGUIExtend.Instance.Mouse_RightDrop(new Dictionary<string, Action>
                {
                    {
                        "删除",(() => { taglist.RemoveAt(Index); })
                    }
                });
                EditorGUILayout.EndVertical();
            }
            newtagname = EditorGUILayout.TextField(newtagname, GUILayout.Width((innerBoxWidth / 2) * (1f / 3f) - 15f));
            if (MyGUI.Button("+"))
            {
                taglist.Add(newtagname);
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            // // 二级列表框 add by TangJian 2017/11/15 16:27:46
            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width((innerBoxWidth / 2) * (2f / 3f)), GUILayout.ExpandHeight(true));
            SearchContext = GUILayout.TextField(SearchContext);
            _listScrollViewPos = EditorGUILayout.BeginScrollView(_listScrollViewPos);
            
            if (!string.IsNullOrEmpty(SearchContext))
            {
                for (int i = 0; i < _placementConfigList.Count; i++)
                {
                    if (Regex.IsMatch(_placementConfigList[i].id.ToLower(),SearchContext))
                    {
                        ListID(i);
                    }
                }
            }
            else
            {
                for (int i = _placementConfigList.Count - 1; i >= 0; i--)
                {
                    if (!showIndexList.Contains(i)) continue;
                    EditorGUILayout.BeginHorizontal();
                    ListID(i);
                    EditorGUILayout.EndHorizontal();
                }
            }
            if (MyGUI.Button("+"))
            {
                _placementConfigList.Add(new tian.PlacementConfig());
                _placementConfigList[_placementConfigList.Count - 1].tagstring = currtagname;
                showIndexList.Add(_placementConfigList.Count - 1);
            }

            GUILayout.Space(10);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();


            // 编辑框 add by TangJian 2017/11/15 16:28:46
            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(innerBoxWidth / 2), GUILayout.ExpandHeight(true));
            _editScrollViewPos = EditorGUILayout.BeginScrollView(_editScrollViewPos);
            if (_currPlacementConfig != null)
            {
                _currPlacementConfig.tagstring = MyGUI.TextFieldWithTitle("标签", _currPlacementConfig.tagstring);
                _currPlacementConfig.id = MyGUI.TextFieldWithTitle("ID", _currPlacementConfig.id);
                _currPlacementConfig.prefab = MyGUI.TextFieldWithTitle("prefab", _currPlacementConfig.prefab);
                _currPlacementConfig.AddRigidbody = MyGUI.ToggleWithTitle("是否应用重力", _currPlacementConfig.AddRigidbody);
                _currPlacementConfig.colliderlayer = (tian.ColliderLayer)MyGUI.EnumPopupWithTitle("碰撞layer", _currPlacementConfig.colliderlayer);
                _currPlacementConfig.colliderSizeType = (tian.ColliderSizeType)MyGUI.EnumPopupWithTitle("size半自动与手工填写", _currPlacementConfig.colliderSizeType);
                _currPlacementConfig.placementData.deathEffect = MyGUI.TextFieldWithTitle("死亡动画|摧毁动画|爆炸动画", _currPlacementConfig.placementData.deathEffect);
                if (_currPlacementConfig.colliderSizeType != tian.ColliderSizeType.TextureRect)
                {
                    _currPlacementConfig.boundsPosition = MyGUI.Vector3WithTitle("碰撞体position", _currPlacementConfig.boundsPosition);
                    _currPlacementConfig.size = MyGUI.Vector3WithTitle("碰撞体size", _currPlacementConfig.size);
                }
                _currPlacementConfig.NoNavMeshObstacle = MyGUI.ToggleWithTitle("不需要NavMeshObstacle", _currPlacementConfig.NoNavMeshObstacle);
                _currPlacementConfig.NoSortRenderPos = MyGUI.ToggleWithTitle("不需要SortRenderPos", _currPlacementConfig.NoSortRenderPos);
                // Renderer朝向 add by TangJian 2018/12/18 15:57
                _currPlacementConfig.RendererOrientation =
                    (RendererOrientation)MyGUI.EnumPopupWithTitle("Renderer朝向", _currPlacementConfig.RendererOrientation);

                _currPlacementConfig.randerChoice = (tian.RanderChoice)MyGUI.EnumPopupWithTitle("Render选择", _currPlacementConfig.randerChoice);
                switch (_currPlacementConfig.randerChoice)
                {
                    case tian.RanderChoice.Anim:
                        Spine.Unity.SkeletonDataAsset skeletonDataAsset = (Spine.Unity.SkeletonDataAsset)AssetDatabase.LoadAssetAtPath(_currPlacementConfig.SkeletonDataAssetpath, typeof(Spine.Unity.SkeletonDataAsset));
                        UnityEditor.Animations.AnimatorController animatorController = (UnityEditor.Animations.AnimatorController)AssetDatabase.LoadAssetAtPath(_currPlacementConfig.AnimatorControllerpath, typeof(UnityEditor.Animations.AnimatorController));
                        animatorController = (UnityEditor.Animations.AnimatorController)EditorGUILayout.ObjectField(new GUIContent("动画控制器"), animatorController, typeof(UnityEditor.Animations.AnimatorController), true);
                        skeletonDataAsset = (Spine.Unity.SkeletonDataAsset)EditorGUILayout.ObjectField(new GUIContent("Skeleton"), skeletonDataAsset, typeof(Spine.Unity.SkeletonDataAsset), true);
                        _currPlacementConfig.SkeletonDataAssetpath = AssetDatabase.GetAssetPath(skeletonDataAsset);
                        _currPlacementConfig.AnimatorControllerpath = AssetDatabase.GetAssetPath(animatorController);
                        _currPlacementConfig.ImagePath = "";
                        if (MyGUI.Button("打开纹理正面侧面编辑器"))
                        {
                            BoxTextureEditorWindow.Open(_currPlacementConfig);
                        }
                        break;
                    case tian.RanderChoice.Image:
                        _currPlacementConfig.sceneDecorationPosition = (SceneDecorationPosition)MyGUI.EnumPopupWithTitle("显示位置", _currPlacementConfig.sceneDecorationPosition);
                        _usePathorImg = EditorGUILayout.ToggleLeft(new GUIContent("使用路径还是贴图"), _usePathorImg);

                        Material material = AssetDatabase.LoadAssetAtPath<Material>(_currPlacementConfig.materialPath);
                        material = (Material)EditorGUILayout.ObjectField("材质", material, typeof(Material), false);
                        _currPlacementConfig.materialPath = AssetDatabase.GetAssetPath(material);
                        
                        if (_usePathorImg)
                        {
                            Texture2D img = (Texture2D)AssetDatabase.LoadAssetAtPath(_currPlacementConfig.ImagePath, typeof(Texture2D));
                            img = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("贴图"), img, typeof(Texture2D), _usePathorImg);
                            _currPlacementConfig.ImagePath = AssetDatabase.GetAssetPath(img);
                            EditorGUILayout.LabelField("图片路径:", _currPlacementConfig.ImagePath);
                        }
                        else
                        {
                            _currPlacementConfig.ImagePath = MyGUI.TextFieldWithTitle("图片路径:", _currPlacementConfig.ImagePath);

                            Texture2D img = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("贴图"), AssetDatabase.LoadAssetAtPath(_currPlacementConfig.ImagePath, typeof(Texture2D)), typeof(Texture2D), _usePathorImg);
                        }

                        if (MyGUI.Button("打开纹理正面侧面编辑器"))
                        {
                            BoxTextureEditorWindow.Open(_currPlacementConfig);
                        }

                        _currPlacementConfig.scaleType = (tian.ScaleType)MyGUI.EnumPopupWithTitle("使用整体缩放大小还是单独缩放大小", _currPlacementConfig.scaleType);
                        if (_currPlacementConfig.scaleType == tian.ScaleType.Alone)
                        {
                            _currPlacementConfig.AloneScale = MyGUI.FloatFieldWithTitle("单独缩放大小", _currPlacementConfig.AloneScale);
                        }
                        // currPlacementConfig.SkeletonDataAsset = null;
                        // currPlacementConfig.AnimatorController = null;
                        _currPlacementConfig.SkeletonDataAssetpath = "";
                        _currPlacementConfig.AnimatorControllerpath = "";
                        break;
                    default:
                        // currPlacementConfig.SkeletonDataAsset = null;
                        _currPlacementConfig.ImagePath = "";
                        _currPlacementConfig.SkeletonDataAssetpath = "";
                        _currPlacementConfig.AnimatorControllerpath = "";
                        // currPlacementConfig.AnimatorController = null;
                        break;
                }

                _currPlacementConfig.placementType = (tian.PlacementType)MyGUI.EnumPopupWithTitle("placementType", _currPlacementConfig.placementType);
                switch (_currPlacementConfig.placementType)
                {
                    case tian.PlacementType.TreasureBox:
                        DrawDropItemList(ref _currPlacementConfig.placementData.dropItemList);
                        break;
                    case tian.PlacementType.trap:
                        _currPlacementConfig.atk = MyGUI.IntFieldWithTitle("Atk", _currPlacementConfig.atk);
                        _currPlacementConfig.trapType = (tian.TrapType)MyGUI.EnumPopupWithTitle("陷阱类型:", _currPlacementConfig.trapType);
                        switch (_currPlacementConfig.trapType)
                        {
                            case tian.TrapType.GroundStab:
                                GroundStabStateType groundStabStateType = (GroundStabStateType)_currPlacementConfig.trapState;
                                groundStabStateType = (GroundStabStateType)MyGUI.EnumPopupWithTitle("地刺类型", groundStabStateType);
                                _currPlacementConfig.trapState = (int)groundStabStateType;
                                string st1 = "";
                                string st2 = "";
                                switch (_currPlacementConfig.trapState)
                                {
                                    case 0:
                                        st1 = "攻击延迟";
                                        st2 = "关闭延迟";
                                        break;
                                    case 1:
                                        st1 = "地刺地上停留时间";
                                        st2 = "地刺地下停留时间";
                                        _currPlacementConfig.float3 = MyGUI.FloatFieldWithTitle("首次停留时间", _currPlacementConfig.float3);
                                        break;
                                }
                                _currPlacementConfig.float1 = MyGUI.FloatFieldWithTitle(st1, _currPlacementConfig.float1);
                                _currPlacementConfig.float2 = MyGUI.FloatFieldWithTitle(st2, _currPlacementConfig.float2);
                                break;
                        }
                        break;
                    case tian.PlacementType.bucket:
                        _currPlacementConfig.placementData.hporCount = (tian.HporCount)MyGUI.EnumPopupWithTitle("打击数或hp", _currPlacementConfig.placementData.hporCount);
                        switch (_currPlacementConfig.placementData.hporCount)
                        {
                            case tian.HporCount.Hp:
                                _currPlacementConfig.placementData.atkhp = MyGUI.FloatFieldWithTitle("Hp", _currPlacementConfig.placementData.atkhp);
                                break;
                            case tian.HporCount.count:
                                _currPlacementConfig.placementData.atkcount = MyGUI.IntFieldWithTitle("被打击次数", _currPlacementConfig.placementData.atkcount);
                                break;
                            default:
                                break;
                        }

                        DrawDropItemList(ref _currPlacementConfig.placementData.dropItemList);
                        break;
                    case tian.PlacementType.Ladder:
                        _currPlacementConfig.Laddertype = (laddertype)MyGUI.EnumPopupWithTitle("梯子在哪面", _currPlacementConfig.Laddertype);
                        break;
                    case PlacementType.FenceDoor:
                        _currPlacementConfig.Laddertype = (laddertype)MyGUI.EnumPopupWithTitle("朝向", _currPlacementConfig.Laddertype);
                        break;
                    default:
                        break;
                }

                    
                // 材质类型
                _currPlacementConfig.matType = (MatType)MyGUI.EnumPopupWithTitle("材质类型", _currPlacementConfig.matType);
                _currPlacementConfig.hitEffectType = (HitEffectType)MyGUI.EnumPopupWithTitle("打击效果类型", _currPlacementConfig.hitEffectType);
                
                //支持位移碰撞
                _currPlacementConfig.placementShake = MyGUI.ToggleWithTitle("支持位移碰撞",_currPlacementConfig.placementShake);
            }
            else
            {
                if (currtagname != null)
                {

                    EditorGUILayout.LabelField("当前选中标签:" + currtagname);
                    newtagnae = MyGUI.TextFieldWithTitle("新标签名称:", newtagnae);
                    if (MyGUI.Button("标签名称改动"))
                    {
                        if (taglist.Contains(currtagname))
                        {
                            //taglist[taglist.IndexOf(currtagname)] = newtagnae;
                            //currtagname = newtagnae;
                            foreach (var children in _placementConfigList)
                            {
                                if (children.tagstring == currtagname)
                                {
                                    children.tagstring = newtagnae;
                                }
                            }
                            taglist[taglist.IndexOf(currtagname)] = newtagnae;
                            currtagname = newtagnae;
                        }
                        newtagnae = "";
                    }

                }
            }

            GUILayout.Space(10);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();


            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private string SearchContext;

        private void ListID(int i)
        {
            int Index = MyGUIExtend.Instance.ListSingleButton("BoxEditor",_placementConfigList[i].id,i,(() =>
            {
                _currPlacementConfig = _placementConfigList[i];
            }));
                    
            MyGUIExtend.Instance.Mouse_RightDrop(new Dictionary<string, Action>
            {
                {
                    "删除",(() =>
                    {
                        _placementConfigList.RemoveAt(Index);
                    })
                },
                {
                    "复制",(() =>
                    {
                        var roleData = Tools.Json2Obj<tian.PlacementConfig>(Tools.Obj2Json(_placementConfigList[Index], true));
                        _placementConfigList.Add(roleData);
                        showIndexList.Add(_placementConfigList.Count - 1);
                    })
                }
            });
        }
        List<DropItem> DrawDropItemList(ref List<DropItem> dropItemList)
        {
            EditorGUILayout.BeginVertical();
            if (MyGUI.Button("添加"))
            {
                dropItemList.Add(new DropItem());
            }
            if (dropItemList != null && dropItemList.Count > 0)
            {
                GUIStyle boxStyle = new GUIStyle("box");
                EditorGUILayout.BeginVertical(boxStyle, GUILayout.ExpandWidth(true));
                for (int i = 0; dropItemList.Count > i; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginVertical();
                    if (MyGUI.Button("删除", GUILayout.Width(50)))
                    {
                        dropItemList.RemoveAt(i);
                        break;
                    }
                    if (MyGUI.Button("复制", GUILayout.Width(50)))
                    {
                        var dropItem = Tools.Json2Obj<DropItem>(Tools.Obj2Json(dropItemList[i], true));
                        dropItemList.Add(dropItem);
                    }
                    EditorGUILayout.EndVertical();
                    GUIStyle nextbox = new GUIStyle("box");
                    EditorGUILayout.BeginVertical(nextbox, GUILayout.ExpandWidth(true));
                    
                    
                    dropItemList[i].itemType = (ItemType)MyGUI.EnumPopupWithTitle("ItemType", dropItemList[i].itemType);
                    dropItemList[i].itemId = MyGUI.TextFieldWithTitle("itemId", dropItemList[i].itemId);
                    dropItemList[i].chance = MyGUI.FloatFieldWithTitle("机率", dropItemList[i].chance);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();

            return dropItemList;
        }


        void OnEnable()
        {
            titleContent = new GUIContent("盒子编辑器");
            LoadRole();
        }

        void LoadRole()
        {
            //string jsonString = Tools.ReadStringFromFile(Application.dataPath + "/" + _roleDataFile);
            _placementConfigDic = Tools.LoadOneData<PlacementConfig> (Application.dataPath + "/" + "Resources_moved/Scripts/box/InteractionComponentsList");

            _placementConfigList = _placementConfigDic.Values.ToList();
            _currPlacementConfig = _placementConfigList[0];
            string scalejson = Tools.ReadStringFromFile(Application.dataPath + "/Resources_moved/Scripts/box/scale.json");
            _scalefloat = Tools.Json2Obj<float>(scalejson);
            string tagjsonstring = Tools.ReadStringFromFile(Application.dataPath + "/Resources_moved/Scripts/box/Taglist.json");
            taglist = Tools.Json2Obj<List<string>>(tagjsonstring);
            currtagname = "";
            showIndexList.Clear();
            for (int ix = 0; ix < _placementConfigList.Count; ix++)
            {
                showIndexList.Add(ix);
            }
        }
        void saveData()
        {
            _placementConfigDic = _placementConfigList.ToDictionary(item => item.id, item => item);
            //string jsonString = Tools.Obj2Json(_placementConfigDic, true);
            //Debug.Log("jsonString = " + jsonString);
            //Tools.WriteStringFromFile(Application.dataPath + "/" + _roleDataFile, jsonString);
            Tools.SaveOneData<PlacementConfig>(_placementConfigDic, Application.dataPath + "/" + "Resources_moved/Scripts/box/InteractionComponentsList");
            string scalejson = Tools.Obj2Json(_scalefloat, true);
            Tools.WriteStringFromFile(Application.dataPath + "/Resources_moved/Scripts/box/scale.json", scalejson);
            string tagjsonstring = Tools.Obj2Json(taglist, true);
            Tools.WriteStringFromFile(Application.dataPath + "/Resources_moved/Scripts/box/Taglist.json", tagjsonstring);
        }

        void SaveRole()
        {
            List<tian.PlacementData> listpl = new List<tian.PlacementData>();

            _placementConfigDic = _placementConfigList.ToDictionary(item => item.id, item => item);

            //string jsonString = Tools.Obj2Json(_placementConfigDic, true);
            //Debug.Log("jsonString = " + jsonString);
            //Tools.WriteStringFromFile(Application.dataPath + "/" + _roleDataFile, jsonString);
            Tools.SaveOneData<PlacementConfig>(_placementConfigDic, Application.dataPath + "/" + "Resources_moved/Scripts/box/InteractionComponentsList");
            
            string scalejson = Tools.Obj2Json(_scalefloat, true);
            Tools.WriteStringFromFile(Application.dataPath + "/Resources_moved/Scripts/box/scale.json", scalejson);
            string tagjsonstring = Tools.Obj2Json(taglist, true);
            Tools.WriteStringFromFile(Application.dataPath + "/Resources_moved/Scripts/box/Taglist.json", tagjsonstring);
            // 生成预制体 add by TangJian 2017/11/16 17:42:05

            // 创建目录 add by TangJian 2018/06/12 17:12:19
            Tools.CreateFolder(Application.dataPath.Substring(0, Application.dataPath.IndexOf("/Assets")) + "/" + _prefabPath);
            Tools.CreateFolder(Application.dataPath.Substring(0, Application.dataPath.IndexOf("/Assets")) + "/" + _prefabPath + "/Mats");
            Tools.CreateFolder(Application.dataPath.Substring(0, Application.dataPath.IndexOf("/Assets")) + "/" + _prefabPath + "/Render");
            Tools.CreateFolder(Application.dataPath.Substring(0, Application.dataPath.IndexOf("/Assets")) + "/" + _prefabPath + "/Render/Anim");
            Tools.CreateFolder(Application.dataPath.Substring(0, Application.dataPath.IndexOf("/Assets")) + "/" + _prefabPath + "/Render/Image");





            //try
            //{
            int currIndex = 0;

            //// 移除老的预制体 add by TangJian 2018/11/20 22:46
            //var files = Tools.GetFilesInFolder(Application.dataPath.Substring(0, Application.dataPath.IndexOf("/Assets")) + "/" + prefabPath + "/Render");
            //foreach (var item in files)
            //{
            //    EditorUtility.DisplayProgressBar("移除老的预制体", item, (float)currIndex++ / files.Count);

            //    Debug.Log("删除文件: " + item);
            //    AssetDatabase.DeleteAsset(item.Substring(item.IndexOf("Assets/")));
            //}

            // 生成新的预制体 add by TangJian 2018/11/20 22:46
            currIndex = 0;
            foreach (var item in _placementConfigDic)
            {
                EditorUtility.DisplayProgressBar("生成新的预制体", item.Value.id, (float)currIndex++ / _placementConfigDic.Count);

                listpl.Add(item.Value.placementData);
                // dataConfigDic.Add(item.Value.placementData.id,item.Value.placementData);
                var itemObject = PrefabCreator.CreatePlacement(item.Value, _prefabPath, _scalefloat);
                DestroyImmediate(itemObject);
            }
            //}
            //catch (System.Exception e)
            //{
            //    Debug.LogError(e);
            //}
            //finally
            //{
            EditorUtility.ClearProgressBar();
            //}

//            _dataConfigDic = listpl.ToDictionary(item => item.id, item => item);
//            string tostr = Tools.Obj2Json(_dataConfigDic, true);
//            Tools.WriteStringFromFile(Application.dataPath + "/" + _dataFile, tostr);
        }
        void Createprefab()
        {
            var itemObject = PrefabCreator.CreatePlacement(_currPlacementConfig, _prefabPath, _scalefloat);
            DestroyImmediate(itemObject);
        }
    }
}