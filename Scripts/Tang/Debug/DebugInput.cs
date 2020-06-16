using System.Collections.Generic;
using UnityEngine;

namespace Tang
{
    public class DebugInput : MonoBehaviour
    {
        SceneManager sceneManager;
        DropItemManager dropItemManager;
        GameObjectManager gameObjectManager;

        int sceneIndex = 0;

        GameObject scene1;
        GameObject scene2;

        void Start()
        {
            sceneManager = GameObject.FindGameObjectWithTag("ManagerObject").GetComponent<SceneManager>();
            dropItemManager = GameObject.FindGameObjectWithTag("ManagerObject").GetComponent<DropItemManager>();
            gameObjectManager = GameObject.FindGameObjectWithTag("ManagerObject").GetComponent<GameObjectManager>();

            scene1 = GameObject.Find("diyu");
            scene2 = GameObject.Find("zoulang");

            // switchScene();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                switchScene();
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                createEnemy();
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                dropWeapon();
            }

            if (Input.GetKeyDown(KeyCode.F4))
            {
                // dropCoin();
                // dropJar();

                // createItem("Box", true);
                createItem("Jar", true);
                // createItem("TreasureBox", true);

            }

            if (Input.GetKeyDown(KeyCode.F5))
            {
                //UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
                UnityEngine.SceneManagement.SceneManager.LoadScene("GameScenewudi1");
            }

            if (Input.GetKeyDown(KeyCode.F6))
            {
                var player = GameObject.Find("Player");
                var roleController = player.GetComponent<RoleController>();

                // 发射火球术
                var fireBall = GameObjectManager.Instance.Spawn("FireBall");

                fireBall.transform.parent = transform.parent;
                fireBall.transform.localPosition = transform.localPosition + new Vector3(roleController.GetDirectionInt() * 3, 0.5f, 0);

                FireBallController fireBallController = fireBall.GetComponent<FireBallController>();
                fireBallController.speed = new Vector3(roleController.GetDirectionInt() * 20, 0, 0);

                fireBallController.DamageController.damageData.owner = gameObject;
            }

            if (Input.GetKeyDown(KeyCode.F12))
            {
                
            }
        }

        void switchScene()
        {
            return;
            switch (sceneIndex % 2)
            {
                case 0:
                    scene1.SetActive(true);
                    scene2.SetActive(false);
                    break;
                case 1:
                    scene1.SetActive(false);
                    scene2.SetActive(true);
                    break;
            }
            sceneIndex += 1;
        }

        void createEnemy()
        {
            var player = GameObject.Find("Player");
            if (player)
            {
                // {
                //     var target = GameObjectManager.Instance.createGameObject("RoleBase");
                //     if (target)
                //     {
                //          target.AddComponent<Player2InputController>();
                //         //target.AddComponent<RoleAIController>();

                //         target.transform.parent = player.transform.parent;
                //         target.transform.localScale = player.transform.localScale;
                //         target.transform.localPosition = new Vector3(player.transform.localPosition.x, player.transform.localPosition.y + 3, player.transform.localPosition.z);
                //     }
                // }

                {
                    var target = GameObjectManager.Instance.Spawn("Shimo");
                    if (target)
                    {
                        target.AddComponent<Player2InputController>();
                        //target.AddComponent<RoleAIController>();

                        target.transform.parent = player.transform.parent;
                        target.transform.localScale = player.transform.localScale;
                        target.transform.localPosition = new Vector3(player.transform.localPosition.x, player.transform.localPosition.y + 3, player.transform.localPosition.z);
                    }
                }
            }
        }

        void dropWeapon()
        {
            var player = GameObject.Find("Player");
            if (player)
            {
                List<string> itemList = new List<string>()
                {
                    "Lswd-1",
                    "Katana-1",
                    "Swd-1",
                    "Blunt-1",
                    "Sswd-1"
                };

                foreach (var item in itemList)
                {
                    createWeapon(item, true, true);
                    // GameObject weapon = gameObjectManager.createWeapon(item);
                    // sceneManager.CurrSceneController.enterItem(weapon, player.transform.localPosition);
                    // weapon.transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360));
                }
            }
        }

        void dropCoin()
        {
            createItem("Coin");
        }

        void dropJar()
        {
            createItem("Jar");
        }

        void createItem(string id, bool randomPos = false, bool rotating = false)
        {
            var player = GameManager.Instance.Player1;
            if (player)
            {
                GameObject gameObject = gameObjectManager.Create(id);
                DropItemController dropItemController = gameObject.GetComponent<DropItemController>();
                if (dropItemController != null)
                {
                    var pos = player.transform.localPosition + new Vector3(0, 5, 0);
                    if (randomPos)
                    {
                        pos += new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5));
                    }

                    sceneManager.DropItemEnterSceneWithLocalPosition(dropItemController, player.SceneId, pos);
                    if (rotating)
                    {
                        gameObject.transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360));
                    }
                }
            }
        }

        void createWeapon(string id, bool randomPos = false, bool rotating = false)
        {
            var player = GameManager.Instance.Player1;
            if (player)
            {
                GameObject gameObject = gameObjectManager.Create(id);
                DropItemController dropItemController = gameObject.GetComponent<DropItemController>();
                if (dropItemController != null)
                {
                    var pos = player.transform.localPosition + new Vector3(0, 5, 0);
                    if (randomPos)
                    {
                        pos += new Vector3(Random.Range(-5, 5), Random.Range(1, 5), Random.Range(-5, 5));
                    }
                    
                    sceneManager.DropItemEnterSceneWithLocalPosition(dropItemController, player.SceneId, pos);
                    if (rotating)
                    {
                        gameObject.transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360));
                    }
                }
            }
        }
    }
}