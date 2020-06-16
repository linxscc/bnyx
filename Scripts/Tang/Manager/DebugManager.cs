using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Task = System.Threading.Tasks.Task;

namespace Tang
{
    public class DebugManager : MyMonoBehaviour
    {
        private static DebugManager instance;

        public static DebugManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = MainManager.GetInstance().GetManager<DebugManager>();
                }
                return instance;
            }
        }

        ConsoleController consoleController;
        ConsoleController Console
        {
            get
            {
                if (consoleController == null)
                {
                    consoleController = GameObject.FindGameObjectWithTag("Console").GetComponent<ConsoleController>();
                }
                return consoleController;
            }
        }

        public Dictionary<string, System.Action> drawGizmosActionDict = new Dictionary<string, System.Action>();
        public Dictionary<string, System.Action> DrawGizmosActionDict { get { return drawGizmosActionDict; } }


        public bool DebugEnable = false;

        public DebugData debugData = new DebugData();

        public void Log(object str)
        {
            Console.Print(str);
        }

        void Start()
        {
            
        }

        public override async void OnUpdate()
        {
            Definition.Debug = DebugEnable;

            if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                var role4 = await CreateRoleAsync("Boss002");
            }

            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                var role5 = GameObject.Find("Player1");
                bool enble = role5.GetComponent<Player1InputController>().enabled;

                role5.GetComponent<Player1InputController>().enabled = !enble;
            }
            
            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                var role5 = GameObject.Find("Player1");
                role5.GetComponent<RoleController>().SkeletonAnimator.SetBackwards(true);
            }

            if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                var role5 = GameObject.Find("Player1");
                role5.GetComponent<RoleController>().SkeletonAnimator.SetBackwards(false);
            }
            
            
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {

                var player = GameObject.Find("Player1");
                AnimManager.Instance.PrintBubble("[color=#FFFFFF,#000000]fgtdfg[/color]", player);
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
//                GameManager.Instance.CreatePlayer2();
                var role = await CreateXBoxplayer(debugData.CreateRoles);
                debugData.useKeyboard = false;
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                string[] monsterArray = {"Prisoner", "BigPrisoner", "Human"};
                var role4 = await CreateRoleAsync("Prisoner");
                var behaviorTree = role4.GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
                Tools.Destroy(behaviorTree);
 
                var navMeshAgent = role4.GetComponent<NavMeshAgent>();
                Tools.Destroy(navMeshAgent); 
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                CreateGameObject("necklace1", true);
                CreateGameObject("ring1", true);
                CreateGameObject("glove1", true);
                CreateGameObject("shoe1", true);
                CreateGameObject("helmet1", true);
                CreateGameObject("trousers1", true);
                CreateGameObject("armor_2", true);
            }

            if (Input.GetKeyDown(KeyCode.F4))
            {
                SceneManager.Instance.CurrScene.RefreshMonster("Prisoner",3);
            }

            if (Input.GetKeyDown(KeyCode.F5))
            {
                
            }

            if (Input.GetKeyDown(KeyCode.F6))
            {
                var player = GameObject.Find("Player1");
                var roleController = player.GetComponent<RoleController>();
                var fireBall = GameObjectManager.Instance.Spawn("FireBall");

                fireBall.transform.parent = transform.parent;
                fireBall.transform.localPosition = transform.localPosition + new Vector3(roleController.GetDirectionInt() * 3, 0.5f, 0);

                FlySkillController flySkillController = fireBall.GetComponent<FlySkillController>();

                flySkillController.UseGravity = false;
                flySkillController.Speed = new Vector3(roleController.GetDirectionInt() * 20, 0, 0);
                flySkillController.MainDamageData.owner = gameObject;
                flySkillController.MainDamageData.teamId = roleController.RoleData.TeamId;
            }

            if (Input.GetKeyDown(KeyCode.F7))
            {
                var player = GameObject.Find("Player1");
                var roleController = player.GetComponent<RoleController>();

                roleController.RoleData.EquipData.MainHand = ItemManager.Instance.getItemDataById<WeaponData>("swd-1");
                roleController.RoleData.EquipData.OffHand = ItemManager.Instance.getItemDataById<WeaponData>("swd-1");
            }

            if (Input.GetKeyDown(KeyCode.F8))
            {
                debugData.debug = !debugData.debug;
                // var player = GameObject.Find("Player1");
                // var roleController = player.GetComponent<RoleController>();
                // roleController.buffController.AddBuff("SpeedUp");
                // roleController.buffController.AddBuff("Bleeding");
            }

            if (Input.GetKeyDown(KeyCode.F9))
            {
                ConfigManager.Instance.Refresh();
            }

            if (Input.GetKeyDown(KeyCode.F11))
            {
                GameManager.Instance.Player1.RoleData.Hp = 999999;
                GameManager.Instance.Player1.RoleData.Atk = 999999;
//                GameManager.Instance.cleanSaveGame();
            }

            if (Input.GetKeyDown(KeyCode.F12))
            {
                var player = GameObject.Find("Player1");
                if (player.GetComponent<RoleController>())
                {
                    player.GetComponent<RoleController>().RoleData.WalkSpeed = 20;
                }
            }
        }
        
        async Task<GameObject> CreateRoleAsync(string roleId, bool withAI = false, string teamId = "2")
        {
            var player = GameManager.Instance.Player1;
            Debug.Assert(player != null);

            var target = await AssetManager.Instantiate(Definition.RoleAssetPrefix + roleId + ".prefab");
            Debug.Assert(target != null);

            RoleController roleController = target.GetComponent<RoleController>();
            Debug.Assert(roleController != null);

            if (withAI)
            {
                Tools.AddComponent<RoleBehaviorTree>(target);
                Tools.AddComponent<RoleNavMeshAgent>(target);
            }

            // 设置角色队伍 add by TangJian 2017/12/20 22:02:00
            roleController.RoleData.TeamId = teamId;

            SceneManager.Instance.RoleEnterSceneWithLocalPosition(roleController, player.SceneId,
                new Vector3(player.transform.localPosition.x, player.transform.localPosition.y + 3,
                    player.transform.localPosition.z));
            return target;
        }
        
        
        public async Task<RoleController> CreateXBoxplayer(string Id)
        {
            
            var player = GameManager.Instance.Player1;
            GameObject role = await AssetManager.InstantiateRole(Id);

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

            SceneManager.Instance.RoleEnterSceneWithLocalPosition(roleController, player.SceneId,
                new Vector3(player.transform.localPosition.x, player.transform.localPosition.y + 3,
                    player.transform.localPosition.z));
            return roleController;
        }
        public GameObject CreateRole(string roleId, bool withAI = false, string teamId = "2")
        {
            var player = GameManager.Instance.Player1;
            Debug.Assert(player != null);

            var target = GameObjectManager.Instance.Spawn(roleId);
            Debug.Assert(target != null);

            RoleController roleController = target.GetComponent<RoleController>();
            Debug.Assert(roleController != null);

            if (withAI)
            {
                Tools.AddComponent<RoleBehaviorTree>(target);
                Tools.AddComponent<RoleNavMeshAgent>(target);
            }

            // 设置角色队伍 add by TangJian 2017/12/20 22:02:00
            roleController.RoleData.TeamId = teamId;

            SceneManager.Instance.RoleEnterSceneWithLocalPosition(roleController, player.SceneId,
                new Vector3(player.transform.localPosition.x, player.transform.localPosition.y + 3,
                    player.transform.localPosition.z));
            return target;
        }

        void dropItem()
        {
            var player = GameObject.Find("Player1");
            if (player)
            {
                foreach (var item in ItemManager.Instance.itemDataDict)
                {
                    CreateGameObject(item.Value.id, true);
                }
            }
        }

        void dropCoin()
        {
            CreateItem("Coin");
        }

        void dropJar()
        {
            CreateItem("Jar");
        }

        public void CreateItem(string id, bool randomPos = false, bool rotating = false)
        {
            var player = GameManager.Instance.Player1;
            if (player)
            {
                GameObject gameObject = GameObjectManager.Instance.Create(id);
                DropItemController dropItemController = gameObject.GetComponent<DropItemController>();
                if (gameObject != null)
                {
                    var pos = player.transform.localPosition + new Vector3(0, 5, 0);
                    if (randomPos)
                    {
                        pos += new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5));
                    }
                    SceneManager.Instance.DropItemEnterSceneWithLocalPosition(dropItemController,  player.SceneId, pos);
                    if (rotating)
                    {
                        gameObject.transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360));
                    }
                }
            }
        }

        void CreateGameObject(string id, bool randomPos = false, bool rotating = false)
        {
            var player = GameManager.Instance.Player1;
            if (player)
            {
                // GameObject gameObject = GameObjectManager.Instance.createGameObject(id);
                GameObject gameObject = GameObjectManager.Instance.Spawn(id);
                DropItemController dropItemController = gameObject.GetComponent<DropItemController>();
                if (dropItemController != null)
                {
                    var pos = player.transform.localPosition + new Vector3(0, 5, 0);
                    if (randomPos)
                    {
                        pos += new Vector3(Random.Range(-5, 5), Random.Range(1, 5), Random.Range(-5, 5));
                    }
                    SceneManager.Instance.DropItemEnterSceneWithLocalPosition(dropItemController, player.SceneId, pos);
                    if (rotating)
                    {
                        gameObject.transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360));
                    }
                }
            }
        }

        public void AddDrawGizmos(string id, System.Action action, float retainTime = 999)
        {
            if (DebugEnable)
            {
                RemoveDrawGizmos(id);
                drawGizmosActionDict.Add(id, action);

                DelayFunc(() =>
                {
                    RemoveDrawGizmos(id);
                }, retainTime);
            }
        }

        public void RemoveDrawGizmos(string id)
        {
            if (drawGizmosActionDict.ContainsKey(id))
            {
                drawGizmosActionDict.Remove(id);
            }
        }
        
        void OnDrawGizmos()
        {
            foreach (var item in drawGizmosActionDict)
            {
                item.Value();
            }
        }
    }
}