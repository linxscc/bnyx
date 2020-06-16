using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Spine.Unity;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using ZS;
using Random = UnityEngine.Random;


namespace Tang
{
    public class GameSettings
    {

    }

    [System.Serializable]
    public class GameEditorSettings
    {
        public static string SettingsFileName
        {
            get
            {
                return Application.dataPath + "/" + "GameEditorSettings";
                // return Application.persistentDataPath + "GameSettings";
            }
        }


        public static GameEditorSettings instance;
        public static GameEditorSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Load();
                }
                return instance;
            }
        }

        public static GameEditorSettings Load()
        {
            string jsonStr = System.IO.File.Exists(SettingsFileName) ? Tools.ReadStringFromFile(SettingsFileName) : "";
            GameEditorSettings gs = Tools.Json2Obj<GameEditorSettings>(jsonStr);
            if (gs == null)
            {
                gs = new GameEditorSettings();
            }
            return gs;
        }
        public static void Save(GameEditorSettings settings)
        {
            string jsonStr = Tools.Obj2Json(settings);
            Tools.WriteStringFromFile(SettingsFileName, jsonStr);
        }

        public void Save()
        {
            Save(this);
        }

        public Vector3 frontDoorOffset = Vector3.zero;
    }

    public class GameManager : MonoBehaviour
    {
        public string levelFile;

        ValueMonitorPool valueMonitorPool = new ValueMonitorPool();

        private static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = MainManager.GetInstance().GetManager<GameManager>();
                }
                return instance;
            }
        }

        public bool logEnabled = false;

        public string playerPrefabName;

        public RoleController Player1;
        public RoleController Player2;

        public GameData gameData = new GameData();
        private GamingUIController gamingUIController;
        private LoadingUIController loadingUIController;
        private GameOverUIController gameoverUIController;
        private SuspendUIController suspendUIController;
        private NewRoleInfoUIController newRoleInfoUIController;
        public ChoiceBubbleController choiceBubbleController;
        private StoreUIController storeUIController;
        public BossUIController BossUIController;
        
        FSM fsm = new FSM();
        IEnumerator currCoroutine;

        IEnumerator enterNextLevelCoroutine;

        public bool UseNewLevelConfig = false;

        public string CurrPathName;
        public string NewLevelName;
        bool firstgr = true;

        private RoleData player1Data;

        // 难度等级 add by TangJian 2019/3/7 22:15
        public int difficultyLevel = 1;

        void Awake()
        {
            Debug.unityLogger.logEnabled = logEnabled;
            Random.InitState(System.DateTime.Now.Millisecond);

            destroyEdit();
            valueMonitorPool.Clear();

            CreatObj();

            InitFSM();
        }
        
     
        private void CreatObj(){
            gamingUIController = UIManager.Instance.GetUI<GamingUIController>("Gaming");
            loadingUIController = UIManager.Instance.GetUI<LoadingUIController>("Loading");
            suspendUIController = UIManager.Instance.GetUI<SuspendUIController>("suspend");
            gameoverUIController = UIManager.Instance.GetUI<GameOverUIController>("GameOver");
            gameoverUIController.gameObject.SetActive(false);
            
            newRoleInfoUIController =  UIManager.Instance.GetUI<NewRoleInfoUIController>("NewRoleInfo");
            storeUIController = UIManager.Instance.GetUI<StoreUIController>("Store");
            
            

            gameEffectUIAnimation = UIManager.Instance.CreatSUIObj<SkeletonAnimation>("OverSpine01");
            gameEffectUIAnimationHint = UIManager.Instance.CreatSUIObj<SkeletonAnimation>("OverSpine02");
            BossUIController = UIManager.Instance.GetUI<BossUIController>("BossHP");
        }
        public void StartGame(RoleData player1Data = null)
        {
            this.player1Data = player1Data;
            fsm.SendEvent("StartNewGame");
        }

        public void InitFSM()
        {
            fsm.SetCurrStateName("Idle");
            fsm.AddState("Idle"); // 啥都不干状态 add by TangJian 2017/11/23 16:35:13            

            fsm.AddState("StartNewGame",
                () =>
                {
                    currCoroutine = StartNewGame();
                },
                () =>
                {
                    if (currCoroutine.MoveNext())
                    {
                    }
                    else
                    {
                        fsm.SendEvent("Gaming");
                        FirstGrandstabWaitLoopDataDic();
                    }
                },
                () =>
                {
                });

            fsm.AddState("Gaming", () =>
            {
                InputManager.Instance.SwitchActions("Gaming");
                gamingUIController.Show();
                InputManager.Instance.Subscript("Gaming",new BindingInputAction("roleInfoUIController",
                    (s, value) =>
                    {
                        switch (s)
                        {
                            case "UI_C":
                                if (value.Get<float>()>0.5f)
                                {
                                    if (newRoleInfoUIController.Panelupvisible) { }
                                    else {
                                        if (gamingUIController.GamingUIvisible()) {
                                            fsm.SendEvent("AllToRoleInfo");
                                        }
                                    }
                                }
                                break;
                            case "UI_Q":
                                if (value.Get<float>()>0.5f)
                                {
                                    gamingUIController.Leftslide();
                                }
                                break;
                            case "UI_B":
                                if (value.Get<float>()>0.5f)
                                {
                                    fsm.SendEvent("AllTostore");
                                }
                                break;
                            case "UI_E":
                                if (value.Get<float>()>0.5f)
                                {
                                    gamingUIController.Rightslide();
                                }
                                break;
                            case "UI_Escape":
                                if (value.Get<float>()>0.5f)
                                {
                                    fsm.SendEvent("AllToSuspend");
                                }
                                break;
                            default:
                                break;
                        }
                    }));
            }, () =>
            {

            }, () =>
            {
                gamingUIController.Hide();
            });


            fsm.AddState("store", () =>
            {
                
                storeUIController.Show();
                InputManager.Instance.Subscript("Store",new BindingInputAction("StoreUIController", (s, value) =>
                {
                    switch (s)
                    {
                        case "UI_W":
                            if (value.Get<float>()>0.5f)
                            {
                                storeUIController.up();
                            }
                            break;
                        case "UI_A":
                            if (value.Get<float>()>0.5f)
                            {
                                storeUIController.left();
                            }
                            break;
                        case "UI_S":
                            if (value.Get<float>()>0.5f)
                            {
                                storeUIController.down();
                            }
                            break;
                        case "UI_D":
                            if (value.Get<float>()>0.5f)
                            {
                                storeUIController.right();
                            }
                            break;
                        case "UI_B":
                            if (value.Get<float>()>0.5f)
                            {
                                fsm.SendEvent("AllToGaming");
                            }
                            break;
                        case "UI_Escape":
                            if (value.Get<float>()>0.5f)
                            {
                                fsm.SendEvent("AllToGaming");
                            }
                            break;
                        case "UI_Return":
                            if (value.Get<float>()>0.5f)
                            {
                                if (storeUIController.leftright == 2 && storeUIController.storebuyindex == 0)
                                {
                                    fsm.SendEvent("AllToGaming");
                                }
                                storeUIController.Enter();
                            }
                            break;
                        default:
                            break;
                    }
                }));
                InputManager.Instance.SwitchActions("Store");

            }, () =>
            {

            }, () =>
            {
                storeUIController.Hide();
            });


            fsm.AddState("RoleInfo", () =>
            {
                
                newRoleInfoUIController.StartRoleinfo();
                //roleInfoUIController.shuanxing();
                InputManager.Instance.Subscript("RoleInfo",new BindingInputAction("roleInfoUIController",
                    (s, value) =>
                    {
                        switch (s)
                        {
                            case "UI_W":
                                if (value.Get<float>()>0.5f)
                                {
                                    newRoleInfoUIController.ChoiceIndex(choicedirection.Up);
                                }
                                break;
                            case "UI_A":
                                if (value.Get<float>()>0.5f)
                                {
                                    newRoleInfoUIController.ChoiceIndex(choicedirection.Left);
                                }
                                break;
                            case "UI_S":
                                if (value.Get<float>()>0.5f)
                                {
                                    newRoleInfoUIController.ChoiceIndex(choicedirection.Down);
                                }
                                break;
                            case "UI_D":
                                if (value.Get<float>()>0.5f)
                                {
                                    newRoleInfoUIController.ChoiceIndex(choicedirection.Right);
                                }
                                break;
                            case "UI_C":
                                if (value.Get<float>()>0.5f)
                                {
                                    if (newRoleInfoUIController.Panelupvisible)
                                    {
                                        if (gamingUIController.GamingUIvisible()) { }
                                        else
                                        {
                                            fsm.SendEvent("AllToGaming");
                                        }
                                    }
                                }
                                break;
                            case "UI_R":
                                if (value.Get<float>()>0.5f)
                                {
                                    newRoleInfoUIController.UnEquip();
                                }
                                break;
                            case "UI_Escape":
                                if (value.Get<float>()>0.5f)
                                {
                                    if (newRoleInfoUIController.Panelupvisible)
                                    {
                                        if (gamingUIController.GamingUIvisible()) { }
                                        else
                                        {
                                            fsm.SendEvent("AllToGaming");
                                        }
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }));
                InputManager.Instance.SwitchActions("RoleInfo");
            }, () =>
            {

            }, () =>
            {
                newRoleInfoUIController.OverRoleinfo();
            });

            fsm.AddState("GameOver", () =>
            {
                GameOver();

                
                InputManager.Instance.Subscript("GameOver",new BindingInputAction("gameoverUIController",
                    (s, value) =>
                    {
                        switch (s)
                        {
                            default:
                                if (value.Get<float>()>0.5f)
                                {
                                    GameStart.Instance.ReloadGame();
                                }
                                break;
                        }
                    }));
                InputManager.Instance.SwitchActions("GameOver");

            }, () =>
            {

            }, () =>
            {
                gameoverUIController.gameObject.SetActive(false);
            });


            fsm.AddState("Suspend", () =>
            {
                suspendUIController.Show();
                InputManager.Instance.Subscript("Suspend",new BindingInputAction("suspendUIController",
                    (s, value) =>
                    {
                        switch (s)
                        {
                            case "UI_W":
                                if (value.Get<float>()>0.5f)
                                {
                                    suspendUIController.up();
                                }
                                break;
                            case "UI_S":
                                if (value.Get<float>()>0.5f)
                                {
                                    suspendUIController.down();
                                }
                                break;
                            case "UI_Return":
                                if (value.Get<float>()>0.5f)
                                {
                                    switch (suspendUIController.choiceindex)
                                    {
                                        case 0:
                                            if (gamingUIController.GamingUIvisible()) { }
                                            else
                                            {
                                                fsm.SendEvent("AllToGaming");
                                            }
                                            break;
                                        case 1:
                                            break;
                                        case 2:
                                            break;
                                        case 3:
                                            break;
                                        case 4:
                                            break;
                                    }
                                }
                                break;
                            case "UI_Escape":
                                if (value.Get<float>()>0.5f)
                                {
                                    fsm.SendEvent("AllToGaming");
                                }
                                break;
                            default:
                                break;
                        }
                    }));
                InputManager.Instance.SwitchActions("Suspend");
            }, () =>
            {

            }, () =>
            {
                suspendUIController.Hide();
            });


            fsm.AddState("Talk", () =>
            {
                choiceBubbleController.Show();
                InputManager.Instance.Subscript("Talk", new BindingInputAction("choiceBubbleController",
                    (s, value) =>
                    {
                        switch (s)
                        {
                            case "UI_W":
                                if (value.Get<float>() > 0.5f)
                                {
                                    choiceBubbleController.upchoice();
                                }

                                break;
                            case "UI_S":
                                if (value.Get<float>() > 0.5f)
                                {
                                    choiceBubbleController.downchoice();
                                }

                                break;
                            case "UI_U":
                                if (value.Get<float>() > 0.5f)
                                {
                                    if (choiceBubbleController.xuanzhe == 0)
                                    {
                                        fsm.SendEvent("AllTostore");
                                    }
                                    else if (choiceBubbleController.xuanzhe == 1)
                                    {
                                    }
                                    else if (choiceBubbleController.xuanzhe == 2)
                                    {
                                    }
                                }

                                break;
                            case "UI_Escape":
                                if (value.Get<float>() > 0.5f)
                                {
                                    fsm.SendEvent("AllToGaming");
                                }

                                break;
                            default:
                                break;
                        }
                    }));
                InputManager.Instance.SwitchActions("Talk");
            }, () =>
            {

            }, () =>
            {
                choiceBubbleController.Hide();
            });


            // event
            {
                fsm.AddEvent("StartNewGame", "All", "StartNewGame");
                fsm.AddEvent("Gaming", "All", "Gaming");

                fsm.AddEvent("GameOver", "Gaming", "GameOver",
                    () =>
                    {
                        var player1Object = GameObject.Find("Player1");
                        if (player1Object != null)
                        {
                            RoleController roleController = player1Object.GetComponent<RoleController>();
                            if (roleController != null)
                            {
                                if (roleController.IsDead)
                                {
                                    //gameoverUIController.RRestart();
                                    return true;
                                }
                            }
                        }
                        return false;
                    },
                    () =>
                    {
                        fsm.SendEvent("AllToGameOver");
                    });


                fsm.AddEvent("GameOver", "RoleInfo", "GameOver",
                    () =>
                    {
                        var player1Object = GameObject.Find("Player1");
                        if (player1Object != null)
                        {
                            RoleController roleController = player1Object.GetComponent<RoleController>();
                            if (roleController != null)
                            {
                                if (roleController.IsDead)
                                {
                                    //gameoverUIController.RRestart();
                                    return true;
                                }
                            }
                        }
                        return false;
                    },
                    () =>
                    {
                        fsm.SendEvent("AllToGameOver");

                    });
            }
        }


        
        private SkeletonAnimation gameEffectUIAnimation;
        private SkeletonAnimation gameEffectUIAnimationHint;

        public void GameOver()
        {
            gameEffectUIAnimation.gameObject.SetActive(true);
            gameEffectUIAnimation.state.SetAnimation(0, "eft2", false);

            this.DelayToDo(2.0f, () =>
            {
                gameEffectUIAnimationHint.gameObject.SetActive(true);
                gameEffectUIAnimationHint.state.SetAnimation(0, "eft3", true);
            });


        }

        public void GamePass()
        {
            gameEffectUIAnimation.gameObject.SetActive(true);
            gameEffectUIAnimation.state.SetAnimation(0, "eft1", false);
            
            GameObject magicPortalGameObject = GameObjectManager.Instance.Create("MagicPortal");
            magicPortalGameObject.transform.parent = Player1.transform.parent;
            magicPortalGameObject.transform.position = Player1.transform.position + new Vector3(0, 0, 2);
        }

        public void StartLevel(int level = 1)
        {

        }

        public void cleanSaveGame()
        {
            PlayerPrefs.SetString("GameData", null);
        }

        public void EnterNextLevel()
        {
            //            if (enterNextLevelCoroutine == null)
            //            {
            //                enterNextLevelCoroutine = IEnumerator_EnterNextLevel();
            //            }
            //            enterNextLevelCoroutine.MoveNext();

            Scene gameScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName("Game");
        }

        IEnumerator StartNewGame(int level = 1)
        {
            var newLevelConfigTask = LevelManager.Instance.GetNewLevelConfig(NewLevelName);

            while (newLevelConfigTask.IsCompleted == false)
            {
                yield return null;
            }
                
            LevelConfig newLevelConfig = newLevelConfigTask.Result;
                
            newLevelConfig.difficultyLevel = level;

            var loadScene = LoadLevelConfig(newLevelConfig);

            bool canMoveNext = true;
            while (canMoveNext)
            {
                canMoveNext = loadScene.MoveNext();
                yield return null;
            }
        }

        IEnumerator LoadLevelConfig(LevelConfig levelConfig)
        {
            levelConfig.currPath = levelConfig.paths[Random.Range(0, levelConfig.paths.Count)];

            CurrPathName = levelConfig.currPath.pathName;

            IEnumerator loadSceneList = SceneManager.Instance.LoadLevelConfig(levelConfig);
                
            // 显示载入条 add by TangJian 2019/3/25 22:18
            loadingUIController.Show();
            loadingUIController.SetProgress(0);
            
            while (loadSceneList.MoveNext())
            {
                loadingUIController.SetProgress(Convert.ToSingle(loadSceneList.Current));
                yield return null;
            }

            // 创建角色 进入第一个场景 add by TangJian 2017/11/23 17:34:30
            {
                var task = CreatePlayer1Async(gameData.player1Data);

                while (task.IsCompleted == false)
                {
                    yield return null;
                }

                Player1 = task.Result;
                    
                SceneManager.Instance.RoleEnterAndSwitchScene(Player1, levelConfig.currPath.beginRoom, "RoleAreas");
            }

            // 隐藏载入界面 add by TangJian 2017/11/23 18:22:25
            loadingUIController.gameObject.SetActive(false);
           
            gamingUIController.Init();
            newRoleInfoUIController.Init();
            gameoverUIController.Init();
            suspendUIController.Init();
            storeUIController.Init();
            initRoleInfovalueMonitorPool();
        }
        void initRoleInfovalueMonitorPool()
        {
            // 监控装备刷新 add by TangJian 2017 / 11 / 20 18:11:11
            valueMonitorPool.AddMonitor<string>((System.Func<string>)(() =>
            {
                return (string)(Player1.RoleData.EquipData.getMainHand<WeaponData>() != null ? Player1.RoleData.EquipData.getMainHand<WeaponData>().id : "");
            }), (string from, string to) =>
            {
                registereuqiadata(8, Player1.RoleData.EquipData.HasMainHand(), Player1.RoleData.EquipData.MainHand.icon, Player1.RoleData.EquipData.getMainHand<WeaponData>(),
                    () =>
                    {
                        if (Player1.RoleData.EquipData.HasMainHand())
                        {
                            unequia(Player1, Player1.RoleData.EquipData.getMainHand<WeaponData>().id);
                            Player1.UnEquipMainHandWeapon();
                        }
                    });
            }, true);

            // 监控装备刷新 add by TangJian 2017 / 11 / 20 18:11:11
            valueMonitorPool.AddMonitor<string>((System.Func<string>)(() =>
            {
                return (string)(Player1.RoleData.EquipData.getOffHand<WeaponData>() != null ? Player1.RoleData.EquipData.getOffHand<WeaponData>().id : "");
            }), (string from, string to) =>
            {
                registereuqiadata(9, Player1.RoleData.EquipData.HasOffHand(), Player1.RoleData.EquipData.OffHand.icon, Player1.RoleData.EquipData.getOffHand<WeaponData>(),
                    () =>
                    {
                        if (Player1.RoleData.EquipData.HasOffHand())
                        {
                            unequia(Player1, Player1.RoleData.EquipData.getOffHand<WeaponData>().id);
                            Player1.UnEquipOffHandWeapon();
                        }
                    });
            }, true);

            // 监控装备刷新 add by TangJian 2017 / 11 / 20 18:11:11
            valueMonitorPool.AddMonitor<string>((System.Func<string>)(() =>
            {
                return (string)(Player1.RoleData.EquipData.GetArmorData() != null ? Player1.RoleData.EquipData.GetArmorData().id : "");
            }), (string from, string to) =>
            {
                registereuqiadata(2, Player1.RoleData.EquipData.HasArmorData(), Player1.RoleData.EquipData.GetArmorData().icon, Player1.RoleData.EquipData.GetArmorData(),
                    () =>
                    {
                        if (Player1.RoleData.EquipData.HasArmorData())
                        {
                            unequia(Player1, Player1.RoleData.EquipData.GetArmorData().id);
                            Player1.UnEquipArmor();
                        }
                    });
            }, true);
            
            ValueMonitorPoolToequip(EquipType.Helmet,0);
            ValueMonitorPoolToequip(EquipType.Necklace,1);
            ValueMonitorPoolToequip(EquipType.Glove,3);
            ValueMonitorPoolToequip(EquipType.Trousers,4);
            ValueMonitorPoolToequip(EquipType.Shoe,5);
            ValueMonitorPoolToRing(0, 6);
            ValueMonitorPoolToRing(1, 7);
        }
        
        public void ValueMonitorPoolToequip(EquipType equipType,int index)
        {
            valueMonitorPool.AddMonitor<string>((System.Func<string>)(() =>
            {
                return (string)(Player1.RoleData.EquipData.GetEquip(equipType) != null ? Player1.RoleData.EquipData.GetEquip(equipType).id : "");
            }), (string from, string to) =>
            {
                registereuqiadata(index, Player1.RoleData.EquipData.HasEquip(equipType), Player1.RoleData.EquipData.GetEquip(equipType).icon, Player1.RoleData.EquipData.GetEquip(equipType),
                    () =>
                    {
                        if (Player1.RoleData.EquipData.HasEquip(equipType))
                        {
                            unequia(Player1, Player1.RoleData.EquipData.GetEquip(equipType).id);
                            Player1.UnEquip(equipType);
                        }
                    });
            }, true);
        }
        public void ValueMonitorPoolToRing(int ringindex, int index)
        {
            valueMonitorPool.AddMonitor<string>((System.Func<string>)(() =>
            {
                return (string)(Player1.RoleData.EquipData.GetEquipRing(ringindex) != null ? Player1.RoleData.EquipData.GetEquipRing(ringindex).id : "");
            }), (string from, string to) =>
            {
                registereuqiadata(index, Player1.RoleData.EquipData.HasEquipRing(ringindex), Player1.RoleData.EquipData.GetEquipRing(ringindex).icon, Player1.RoleData.EquipData.GetEquipRing(ringindex),
                    () =>
                    {
                        if (Player1.RoleData.EquipData.HasEquipRing(ringindex))
                        {
                            unequia(Player1, Player1.RoleData.EquipData.GetEquipRing(ringindex).id);
                            Player1.UnEquipRing(ringindex);
                        }
                    });
            }, true);
        }
        void registereuqiadata(int index, bool show, string icon, EquipData equipData, System.Action action)
        {
            GameManager.Instance.SetChoiceLoader(index, show, show ? "Textures/Icon/" + icon : "", equipData, action);
        }
        void unequia(RoleController roleController, string id)
        {
            GameObject giveUpWeaponObject = GameObjectManager.Instance.Create(id);
            DropItemController dropItemController = giveUpWeaponObject.GetComponent<DropItemController>();
            SceneManager.Instance.DropItemEnterSceneWithLocalPosition(dropItemController, roleController.SceneId,
                roleController.gameObject.transform.localPosition + new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f),
                    UnityEngine.Random.Range(1.0f, 1.5f), UnityEngine.Random.Range(-0.5f, 0.5f)));
        }
        void destroyEdit()
        {
            var edit = GameObject.FindGameObjectWithTag("Edit");
            if (edit != null)
            {
                Destroy(edit);
            }
        }

        public async Task<RoleController> CreatePlayer2(PlayerData playerData = null)
        {
            playerPrefabName = playerPrefabName == null ? "Human" : playerPrefabName;
            playerPrefabName = playerPrefabName == "" ? "Human" : playerPrefabName;
            
            GameObject role = await AssetManager.InstantiateRole(playerPrefabName ?? "Human");

            role.name = "Player2";
            role.tag = "Player2";

            role.AddComponentUnique<Player2InputController>();
            var roleController = role.GetComponent<RoleController>();

            var roleBehaviorTree = role.GetComponent<RoleBehaviorTree>();
            if (roleBehaviorTree != null)
            {
                Tools.Destroy(roleBehaviorTree);
            }

            roleController.RoleData.TeamId = "3";

            SceneManager.Instance.RoleEnterSceneWithLocalPosition(roleController, Player1.SceneId,
                new Vector3(Player1.transform.localPosition.x, Player1.transform.localPosition.y + 3,
                    Player1.transform.localPosition.z));
            return roleController;
        }
        
        async Task<RoleController > CreatePlayer1Async(PlayerData playerData = null)
        {
            playerPrefabName = playerPrefabName == null ? "Human" : playerPrefabName;
            playerPrefabName = playerPrefabName == "" ? "Human" : playerPrefabName;

//            GameObject role = GameObjectManager.Instance.Create(playerPrefabName ?? "Human");
            var task = AssetManager.Instantiate("Roles/Monster/" + playerPrefabName + ".prefab");

            GameObject roleGameObject = await task;

            roleGameObject.name = "Player1";
            roleGameObject.tag = "Player";

            roleGameObject.AddComponentUnique<Player1InputController>();
            var roleController = roleGameObject.GetComponent<RoleController>();

            var roleBehaviorTree = roleGameObject.GetComponent<RoleBehaviorTree>();
            var behaviorTree = roleGameObject.GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
            var InteractiveCube = roleGameObject.GetChild("InteractiveCube");
            var triggercontroller = InteractiveCube.GetComponent<TriggerController>();
            triggercontroller.needKeepingGameObject = true;
            if (roleBehaviorTree != null)
            {
                Tools.Destroy(roleBehaviorTree);
            }
            
            if (behaviorTree != null)
            {
                Tools.Destroy(behaviorTree);
            }
            roleController._withAI = false;

            if (false && // 不读档 add by TangJian 2018/9/21 0:03
                playerData != null && playerData.currRoleData != null)
            {
                roleController.RoleData = playerData.currRoleData;
                roleController.RoleData.TeamId = "1";

                //roleController.RoleData.EquipData.MainHand = ItemManager.Instance.getItemDataById<WeaponData>("swd-1");
                //roleController.RoleData.EquipData.OffHand = ItemManager.Instance.getItemDataById<WeaponData>("swd-1");

                //roleController.RoleData.EquipData.SoulData = ItemManager.Instance.getItemDataById<SoulData>("Soul-FireBall");
            }

            if (this.player1Data != null)
            {
                roleController.RoleData = this.player1Data;
            }

            ConstrainedCamera constrainedCamera = Camera.main.GetComponent<ConstrainedCamera>();
            constrainedCamera.Player1 = roleGameObject.transform;

            // 与npc交互
            roleController.OnInteractAction = (GameObject gameObject) =>
            {
                if (gameObject.tag == "NPC")
                {
                    choiceBubbleController = GameObject.Find("choicebubble").GetComponent<ChoiceBubbleController>();
                    fsm.SendEvent("AllToTalk");
                }
            };

            roleController.PickupConsumable = () =>
            {
                gamingUIController.RefreshGlist();
            };

            roleController.OnDying += (RoleController _roleController) =>
            {
                // 掉装备 add by TangJian 2019/3/14 11:45
                Debug.Log("掉装备");
            };
            
            return roleController;
        }
        
        RoleController CreatePlayer1(PlayerData playerData = null)
        {
            playerPrefabName = playerPrefabName == null ? "Human" : playerPrefabName;
            playerPrefabName = playerPrefabName == "" ? "Human" : playerPrefabName;

            GameObject role = GameObjectManager.Instance.Create(playerPrefabName ?? "Human");

            role.name = "Player1";
            role.tag = "Player";

            role.AddComponentUnique<Player1InputController>();
            var roleController = role.GetComponent<RoleController>();

            var roleBehaviorTree = role.GetComponent<RoleBehaviorTree>();
            var behaviorTree = role.GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
            var InteractiveCube = role.GetChild("InteractiveCube");
            var triggercontroller = InteractiveCube.GetComponent<TriggerController>();
            triggercontroller.needKeepingGameObject = true;
            if (roleBehaviorTree != null)
            {
                Tools.Destroy(roleBehaviorTree);
            }
            
            if (behaviorTree != null)
            {
                Tools.Destroy(behaviorTree);
            }
            roleController._withAI = false;

            if (false && // 不读档 add by TangJian 2018/9/21 0:03
                playerData != null && playerData.currRoleData != null)
            {
                roleController.RoleData = playerData.currRoleData;
                roleController.RoleData.TeamId = "1";

                //roleController.RoleData.EquipData.MainHand = ItemManager.Instance.getItemDataById<WeaponData>("swd-1");
                //roleController.RoleData.EquipData.OffHand = ItemManager.Instance.getItemDataById<WeaponData>("swd-1");

                //roleController.RoleData.EquipData.SoulData = ItemManager.Instance.getItemDataById<SoulData>("Soul-FireBall");
            }

            if (this.player1Data != null)
            {
                roleController.RoleData = this.player1Data;
            }

            ConstrainedCamera constrainedCamera = Camera.main.GetComponent<ConstrainedCamera>();
            constrainedCamera.Player1 = role.transform;

            // 与npc交互
            roleController.OnInteractAction = (GameObject gameObject) =>
            {
                if (gameObject.tag == "NPC")
                {
                    choiceBubbleController = GameObject.Find("choicebubble").GetComponent<ChoiceBubbleController>();
                    fsm.SendEvent("AllToTalk");
                }
            };

            roleController.PickupConsumable = () =>
            {
                gamingUIController.RefreshGlist();
            };

            roleController.OnDying += (RoleController _roleController) =>
            {
                // 掉装备 add by TangJian 2019/3/14 11:45
                Debug.Log("掉装备");
            };
            
            return roleController;
        }


        Dictionary<string, GroundStabController> GrandstabWaitLoopDataDic = new Dictionary<string, GroundStabController>();
        public void AddGrandstabWaitLoopData(string id, GroundStabController groundStabController)
        {
            if (firstgr)
            {
                if (GrandstabWaitLoopDataDic.ContainsKey(id) == false)
                {
                    GrandstabWaitLoopDataDic.Add(id, groundStabController);
                }
            }
            else
            {
                Debug.Log("错过初始化时机:" + id);
            }

        }
        void RemoveGrandstabWaitLoopData(string id)
        {
            GrandstabWaitLoopDataDic.Remove(id);
        }
        void clearGrandstabWaitLoopData()
        {
            GrandstabWaitLoopDataDic.Clear();
        }
        void FirstGrandstabWaitLoopDataDic()
        {
            foreach (var keyValue in GrandstabWaitLoopDataDic)
            {
                GroundStabController groundStabController = keyValue.Value;
                float time = groundStabController.firststate;
                Animator animator = groundStabController.MainAnimator;
                if (groundStabController.first)
                {
                    groundStabController.OnOfftoStab(animator, time, 1);
                }
            }
        }
        public void SetChoiceLoader(int index, bool show, string url, EquipData equipData, System.Action action)
        {
            newRoleInfoUIController.SetChoiceLoader(index, show, url, equipData, action);
        }
        public void PickupTips(string iconurl, string name, string count)
        {
            gamingUIController.PickupTips(iconurl, name, count);
            newRoleInfoUIController.HideRoleInfodown();
        }
        public void RoleinfoButton()
        {
            if (fsm.CurrStateName == "Gaming")
            {
                if (gamingUIController.GamingUIvisible())
                {
                    fsm.SendEvent("AllToRoleInfo");
                }
            }
            else
            {
                if (gamingUIController.GamingUIvisible() == false)
                {
                    fsm.SendEvent("AllToGaming");
                }
            }
        }
        void Update()
        {
            valueMonitorPool.Update();
            fsm.Update();

        }
        IEnumerator waitMove(float time, System.Action action)
        {
            for (float timer = time; timer >= 0; timer -= Time.deltaTime)
            {
                yield return 0;
            }
            action();
        }
    }
    public class GrandstabWaitLoopData
    {
        public Animator animator;
        public string id;
        public float attackTime; //攻击延迟
        public float downTime; //关闭延迟
        public float firststate;
        public bool first = true;
    }
}