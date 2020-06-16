using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;
using System.Collections.ObjectModel;
using DG.Tweening;
using Newtonsoft.Json;

namespace Tang
{
    //选择方向类型
    public enum choicetype
    {
        None = 0,
        ray = 1,
        fix = 2
    }
    public enum choicedirection
    {
        Left = 0,
        Right = 1,
        Up = 2,
        Down = 3
    }
    public class ChoicetypeData
    {
        public choicetype choicetype = choicetype.None;
        public int index = 0;
        public bool defaultbool = true;
        public Vector3 direction = new Vector3();
        public ChoicetypeData(choicetype type, int indexint, bool onoff = true, Vector3 v3 = new Vector3())
        {
            choicetype = type;
            index = indexint;
            defaultbool = onoff;
            direction = v3;
        }
    }
    //
    public class ChoiceData
    {

        public GLoader gLoader;
        public EquipData equipData = new EquipData();
        public Action unEquiaAction;
        public Vector2 center = new Vector2();
        public Vector2 pos = new Vector2();
        public Bounds bounds = new Bounds();
        public Vector2 size = new Vector2();
        public Dictionary<string, ChoicetypeData> ChoicetypeDic = new Dictionary<string, ChoicetypeData>();
        public ChoiceData(GLoader loader, Vector2 point, Vector2 postion, Vector2 sizev2,
            ChoicetypeData LeftChoicetypeData, ChoicetypeData RightChoicetypeData, ChoicetypeData UpChoicetypeData, ChoicetypeData DownChoicetypeData)
        {
            gLoader = loader;

            center = point;
            pos = postion;
            size = sizev2;
            ChoicetypeDic.Add("Left", LeftChoicetypeData);
            ChoicetypeDic.Add("Right", RightChoicetypeData);
            ChoicetypeDic.Add("Up", UpChoicetypeData);
            ChoicetypeDic.Add("Down", DownChoicetypeData);
            Vector3 pointv3 = new Vector3(point.x, point.y, 0f);
            Vector3 sizev3 = new Vector3(sizev2.x, sizev2.y, 1f);
            bounds = new Bounds(pointv3, sizev3);
        }
        public bool DetectHit(Ray ray)
        {
            return bounds.IntersectRay(ray);
        }
        public bool DetectHit(Ray ray, out float dir)
        {
            return bounds.IntersectRay(ray, out dir);
        }
    }
    public class ChoiceDataList
    {
        public List<ChoiceData> choiceDatalist = new List<ChoiceData>();
        public void Unequia(int index)
        {
            ChoiceData choiceData = choiceDatalist[index];
            choiceData.unEquiaAction();
        }
        public int Choice(int currindex, choicedirection choicedirectionx)
        {
            ChoiceData choiceData = choiceDatalist[currindex];
            ChoicetypeData choicetypeData = choiceData.ChoicetypeDic[choicedirectionx.ToString()];
            int toindex = -1;
            if (choicetypeData.choicetype == choicetype.ray)
            {
                Vector3 dir = new Vector3();
                switch (choicedirectionx)
                {
                    case choicedirection.Left:
                        dir = Vector3.left;
                        break;
                    case choicedirection.Right:
                        dir = Vector3.right;
                        break;
                    case choicedirection.Up:
                        dir = Vector3.up;
                        break;
                    case choicedirection.Down:
                        dir = Vector3.down;
                        break;
                }
                toindex = DetectHit(currindex, dir);
            }
            else if (choicetypeData.choicetype == choicetype.fix)
            {
                toindex = choicetypeData.index;
            }
            else
            {

            }
            return toindex;
        }
        public int DetectHit(int currindex, Vector3 dir)
        {
            int index = -1;
            float mint = -1;
            ChoiceData choiceData = choiceDatalist[currindex];
            Vector3 center = new Vector3(choiceData.center.x, choiceData.center.y, 0f);
            Ray ray = new Ray(center, dir);
            List<int> intlist = new List<int>();
            for (int i = 0; i < choiceDatalist.Count; i++)
            {
                if (i != currindex)
                {
                    ChoiceData choice = choiceDatalist[i];
                    float flo = 0;
                    if (choice.DetectHit(ray, out flo))
                    {
                        if (mint == -1)
                        {
                            mint = flo;
                            index = i;
                        }
                        else
                        {
                            if (flo < mint)
                            {
                                mint = flo;
                                index = i;
                            }
                        }
                    }
                }
            }
            //if (intlist.Count == 1)
            //{
            //    index = intlist[0];
            //}
            //else if (intlist.Count > 0)
            //{
            //    for(int it= 0; it < intlist.Count; it++)
            //    {
            //        ChoiceData itemdata = choiceDatalist[intlist[it]];
            //        float dis = Vector2.Distance(itemdata.center, choiceData.center);
            //        if (it == 0)
            //        {
            //            mint = dis;
            //            index = intlist[it];
            //        }
            //        else
            //        {
            //            if (mint> dis)
            //            {
            //                mint = dis;
            //                index = intlist[it];
            //            }
            //        }
            //    }
            //}
            return index;
        }
        
        public bool GetChoiceLoadervisible(int index)
        {
            ChoiceData choiceData = choiceDatalist[index];
            return choiceData.gLoader.visible;
        }
        
        public void SetChoiceLoader(int index, bool show, string url, EquipData equipData = null, Action action = null)
        {
            ChoiceData choiceData = choiceDatalist[index];
            if (show)
            {
                choiceData.equipData = equipData;
                choiceData.gLoader.visible = true;
                choiceData.gLoader.url = url;
                choiceData.unEquiaAction = action;
            }
            else
            {
                choiceData.equipData = null;
                choiceData.gLoader.visible = false;
                choiceData.unEquiaAction = null;
            }
        }
        public Vector2 GetIndexPos(int index)
        {
            ChoiceData choiceData = choiceDatalist[index];
            return choiceData.pos;
        }
        public EquipData GetIndexEquipData(int index)
        {
            ChoiceData choiceData = choiceDatalist[index];
            return choiceData.equipData;
        }

    }

   
    public partial class NewRoleInfoUIController : MonoBehaviour, UIInterface
    {
        ChoiceDataList choiceDataList = new ChoiceDataList();
        GComponent ui;

        GComponent Ui
        {
            get
            {
                if (ui == null)
                {
                    ui = GetComponent<UIPanel>().ui;
                }

                return ui;
            }
        }

        int currindex = 0;
        ValueMonitorPool valueMonitorPool = new ValueMonitorPool();
        GComponent panelup;

        GComponent panelupUI;

        GTextField damage;
        GTextField defenses;
        GTextField MovingSpeed;
        GTextField MagicDamage;

        GTextField leveltext;
        GProgressBar progressBar_hp;
        GProgressBar progressBar_mp;
        GProgressBar progressBar_tili;
        Tweener tweener_progressBar_hp;
        Tweener tweener_progressBar_mp;
        Tweener tweener_progressBar_tili;
        GLoader showLoader;
        RoleController player1Controller;
        GTextField Crit;
        GTextField CriticalDamage;

        GImage choiceframe;
        // Start is called before the first frame update

        PanelDownUI panelDownUI;

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            GetUICompoent();
            Newgloderlist();
            showdropitem();
            InitCurrIndex();
        }

        private void GetUICompoent()
        {
            panelupUI = Ui.GetChild("panelup").asCom;
            
            valueMonitorPool.Clear();
            panelup = Ui.GetChild("panelup").asCom;
            panelup.visible = false;
            progressBar_hp = Ui.GetChildWithPath("panelup/hpbar").asProgress;
            progressBar_mp = Ui.GetChildWithPath("panelup/mpbar").asProgress;
            progressBar_tili = Ui.GetChildWithPath("panelup/physicalbar").asProgress;
            damage = panelup.GetChild("damage").asTextField;
            defenses = panelup.GetChild("defenses").asTextField;
            MovingSpeed = panelup.GetChild("Moving speed").asTextField;
            MagicDamage = panelup.GetChild("Magic damage").asTextField;


            showLoader = panelup.GetChild("n76").asLoader;
            RenderTexture renderTexture = Resources.Load<RenderTexture>("roleinfotexture");
            showLoader.texture = new NTexture(renderTexture);



            Crit = panelup.GetChild("crit").asTextField;
            CriticalDamage = panelup.GetChild("Critical damage").asTextField;
            choiceframe = panelup.GetChild("choiceframe").asImage;
            leveltext = panelup.GetChild("n78").asTextField;
            player1Controller = GameObject.Find("Player1").GetComponent<RoleController>();
            
            panelDownUI = UIManager.Instance.GetUI<PanelDownUI>("paneldown");
        }
        public void Newgloderlist()
        {
            GLoader oneGloder = panelup.GetChild("n1").asLoader;
            oneGloder.visible = false;
            choiceDataList.choiceDatalist.Add(new ChoiceData(
                oneGloder,
                new Vector2(oneGloder.position.x + oneGloder.width / 2f,
                    -(oneGloder.position.y + oneGloder.height / 2f)),
                oneGloder.position, oneGloder.size,
                new ChoicetypeData(choicetype.fix, 4),
                new ChoicetypeData(choicetype.ray, 0),
                new ChoicetypeData(choicetype.fix, 3),
                new ChoicetypeData(choicetype.ray, 0)
            ));
            GLoader twoGloder = panelup.GetChild("n2").asLoader;
            twoGloder.visible = false;
            choiceDataList.choiceDatalist.Add(new ChoiceData(
                twoGloder,
                new Vector2(twoGloder.position.x + twoGloder.width / 2f,
                    -(twoGloder.position.y + twoGloder.height / 2f)),
                twoGloder.position, twoGloder.size,
                new ChoicetypeData(choicetype.fix, 5),
                new ChoicetypeData(choicetype.ray, 0),
                new ChoicetypeData(choicetype.ray, 0),
                new ChoicetypeData(choicetype.ray, 0)
            ));
            GLoader threeGloder = panelup.GetChild("n3").asLoader;
            threeGloder.visible = false;
            choiceDataList.choiceDatalist.Add(new ChoiceData(
                threeGloder,
                new Vector2(threeGloder.position.x + threeGloder.width / 2f,
                    -(threeGloder.position.y + threeGloder.height / 2f)),
                threeGloder.position, threeGloder.size,
                new ChoicetypeData(choicetype.fix, 6),
                new ChoicetypeData(choicetype.ray, 0),
                new ChoicetypeData(choicetype.ray, 0),
                new ChoicetypeData(choicetype.ray, 0)
            ));
            GLoader fourGloder = panelup.GetChild("n4").asLoader;
            fourGloder.visible = false;
            choiceDataList.choiceDatalist.Add(new ChoiceData(
                fourGloder,
                new Vector2(fourGloder.position.x + fourGloder.width / 2f,
                    -(fourGloder.position.y + fourGloder.height / 2f)),
                fourGloder.position, fourGloder.size,
                new ChoicetypeData(choicetype.fix, 7),
                new ChoicetypeData(choicetype.fix, 8),
                new ChoicetypeData(choicetype.ray, 0),
                new ChoicetypeData(choicetype.fix, 8)
            ));
            GLoader fiveGloder = panelup.GetChild("n5").asLoader;
            fiveGloder.visible = false;
            choiceDataList.choiceDatalist.Add(new ChoiceData(
                fiveGloder,
                new Vector2(fiveGloder.position.x + fiveGloder.width / 2f,
                    -(fiveGloder.position.y + fiveGloder.height / 2f)),
                fiveGloder.position, fiveGloder.size,
                new ChoicetypeData(choicetype.ray, 0),
                new ChoicetypeData(choicetype.fix, 0),
                new ChoicetypeData(choicetype.fix, 7),
                new ChoicetypeData(choicetype.ray, 0)
            ));
            GLoader sixGloder = panelup.GetChild("n6").asLoader;
            sixGloder.visible = false;
            choiceDataList.choiceDatalist.Add(new ChoiceData(
                sixGloder,
                new Vector2(sixGloder.position.x + sixGloder.width / 2f,
                    -(sixGloder.position.y + sixGloder.height / 2f)),
                sixGloder.position, sixGloder.size,
                new ChoicetypeData(choicetype.ray, 0),
                new ChoicetypeData(choicetype.fix, 1),
                new ChoicetypeData(choicetype.ray, 0),
                new ChoicetypeData(choicetype.ray, 0)
            ));
            GLoader SevenGloder = panelup.GetChild("n7").asLoader;
            SevenGloder.visible = false;
            choiceDataList.choiceDatalist.Add(new ChoiceData(
                SevenGloder,
                new Vector2(SevenGloder.position.x + SevenGloder.width / 2f,
                    -(SevenGloder.position.y + SevenGloder.height / 2f)),
                SevenGloder.position, SevenGloder.size,
                new ChoicetypeData(choicetype.ray, 0),
                new ChoicetypeData(choicetype.fix, 2),
                new ChoicetypeData(choicetype.ray, 0),
                new ChoicetypeData(choicetype.ray, 0)
            ));
            GLoader EightGloder = panelup.GetChild("n8").asLoader;
            EightGloder.visible = false;
            choiceDataList.choiceDatalist.Add(new ChoiceData(
                EightGloder,
                new Vector2(EightGloder.position.x + EightGloder.width / 2f,
                    -(EightGloder.position.y + EightGloder.height / 2f)),
                EightGloder.position, EightGloder.size,
                new ChoicetypeData(choicetype.fix, 9),
                new ChoicetypeData(choicetype.fix, 3),
                new ChoicetypeData(choicetype.ray, 0),
                new ChoicetypeData(choicetype.fix, 9)
            ));
            GLoader NineGloder = panelup.GetChild("n9").asLoader;
            NineGloder.visible = false;
            choiceDataList.choiceDatalist.Add(new ChoiceData(
                NineGloder,
                new Vector2(NineGloder.position.x + NineGloder.width / 2f,
                    -(NineGloder.position.y + NineGloder.height / 2f)),
                NineGloder.position, NineGloder.size,
                new ChoicetypeData(choicetype.fix, 3),
                new ChoicetypeData(choicetype.ray, 0),
                new ChoicetypeData(choicetype.fix, 3),
                new ChoicetypeData(choicetype.None, 0)
            ));
            GLoader TenGloder = panelup.GetChild("n10").asLoader;
            TenGloder.visible = false;
            choiceDataList.choiceDatalist.Add(new ChoiceData(
                TenGloder,
                new Vector2(TenGloder.position.x + TenGloder.width / 2f,
                    -(TenGloder.position.y + TenGloder.height / 2f)),
                TenGloder.position, TenGloder.size,
                new ChoicetypeData(choicetype.ray, 0),
                new ChoicetypeData(choicetype.fix, 7),
                new ChoicetypeData(choicetype.fix, 7),
                new ChoicetypeData(choicetype.None, 0)
            ));
        }
        private void showdropitem()
        {
            valueMonitorPool.AddMonitor((Func<TriggerController>)(() =>
            {
                GameObject player = GameObject.Find("Player1");
                TriggerController selfTriggerController = player.GetComponentInChildren<TriggerController>();
                otherTriggerController = selfTriggerController.GetFirstKeepingTriggerController();
                return otherTriggerController;
            }), (TriggerController from, TriggerController to) =>
            {
                if (Typedown)
                {
                    if (otherTriggerController != null)
                    {
                        GameObject interactObject = otherTriggerController.ITriggerDelegate.GetGameObject();
                        if (interactObject != null)
                        {
                            DropItemController dropItemController = interactObject.GetComponent<DropItemController>();
                            DoorController doorController = interactObject.GetComponent<DoorController>();
                            if (dropItemController != null)
                            {
                                var itemData = ItemManager.Instance.getItemDataById<ItemData>(dropItemController.ItemId);
                                if (itemData != null)
                                {
                                    GameObject player = GameObject.Find("Player1");
                                    RoleController roleController = player.GetComponent<RoleController>();
                                    if (itemData is WeaponData)
                                    {
                                        AttributeShow<WeaponData>(itemData as EquipData);
                                        ShowRoleInfodown();
                                    }
                                    else if (itemData is ArmorData)
                                    {
                                        AttributeShow<ArmorData>(itemData as EquipData);
                                        ShowRoleInfodown();
                                    }
                                    else if (itemData is DecorationData)//现阶段物品掉落使用
                                    {
                                        DecorationData RoleDecorationData;
                                        AttributeShow<DecorationData>(itemData as EquipData);
                                        ShowRoleInfodown();
                                    }
                                    else
                                    {
                                        HideRoleInfodown();
                                    }
                                }
                            }
                            else if (doorController != null)
                            {
                                Debug.Log("door");
                            }
                        }

                    }
                    else
                    {
                        HideRoleInfodown();
                    }
                }
            });
        }
        private void InitCurrIndex()
        {
            valueMonitorPool.AddMonitor(() => currindex, (f1,f2) =>
            {
                Vector2 pos = choiceDataList.GetIndexPos(currindex);
                choiceframe.position = new Vector3(pos.x - 8f, pos.y - 8f, 0);
                AttributeShow<EquipData>(choiceDataList.GetIndexEquipData(currindex));
                if (choiceDataList.GetChoiceLoadervisible(currindex))
                {
                    ShowPlayerInterface();
                    ShowRoleInfodown();
                }
                else
                {
                    HideRoleInfodown();
                }
            });
        }
        
        private void OnEnable()
        {
            MessageManager.Instance.Subscribe(MessageName.SET_PLAYER1_VIGOR,  SetPlayer1Vigor);
            MessageManager.Instance.Subscribe(MessageName.SET_PLAYER1_FinalHp,  SetPlayer1FinalHp);
            MessageManager.Instance.Subscribe(MessageName.SET_PLAYER1_Exp,  SetPlayer1Exp);
            MessageManager.Instance.Subscribe(MessageName.SET_PLAYER1_Level,  SetPlayer1Level);
            MessageManager.Instance.Subscribe(MessageName.SET_PLAYER1_Damage,  SetPlayer1Damage);
            MessageManager.Instance.Subscribe(MessageName.SET_PLAYER1_MovingSpeed,  SetPlayer1MovingSpeed);
            MessageManager.Instance.Subscribe(MessageName.SET_PLAYER1_FinalCritical,  SetPlayer1FinalCritical);
        }

        private void OnDisable()
        {
            MessageManager.Instance.Unsubscribe(MessageName.SET_PLAYER1_VIGOR,  SetPlayer1Vigor);
            MessageManager.Instance.Unsubscribe(MessageName.SET_PLAYER1_FinalHp,  SetPlayer1FinalHp);
            MessageManager.Instance.Unsubscribe(MessageName.SET_PLAYER1_Exp,  SetPlayer1Exp);
            MessageManager.Instance.Unsubscribe(MessageName.SET_PLAYER1_Level,  SetPlayer1Level);
            MessageManager.Instance.Unsubscribe(MessageName.SET_PLAYER1_Damage,  SetPlayer1Damage);
            MessageManager.Instance.Unsubscribe(MessageName.SET_PLAYER1_MovingSpeed,  SetPlayer1MovingSpeed);
            MessageManager.Instance.Unsubscribe(MessageName.SET_PLAYER1_FinalCritical,  SetPlayer1FinalCritical);
        }

        private void SetPlayer1Vigor(object[] objects)
        {
            float value = (float)objects[0];
            float valueMax = (float)objects[1];
            progressBar_tili.max = valueMax;
            
            tweener_progressBar_tili?.Kill();
            tweener_progressBar_tili = progressBar_tili.DoValue((float) value, 0.5f);
            tweener_progressBar_tili.OnComplete(() => { tweener_progressBar_tili = null; });
        }
        private void SetPlayer1FinalHp(object[] objects)
        {
            float value = (float)objects[0];
            float valueMax = (float)objects[1];
            progressBar_hp.max = valueMax;
            
            if (tweener_progressBar_hp != null)
            {
                TweenExtensions.Kill(tweener_progressBar_hp);
            }

            tweener_progressBar_hp =
                progressBar_hp.DoValue((float) value, 0.3f);
            tweener_progressBar_hp.OnComplete(() => { tweener_progressBar_hp = null; });
        }
        private void SetPlayer1Exp(object[] objects)
        {
            int value = (int)objects[0];
            
            progressBar_mp.max = RoleUpgradeDataAsset.Instance.GetExp(player1Controller.RoleData.level + 1);

            tweener_progressBar_mp?.Kill();
            tweener_progressBar_mp = progressBar_mp.DoValue((float) value, 0.5f);
            tweener_progressBar_mp.OnComplete(() => { tweener_progressBar_mp = null; });
        }
        private void SetPlayer1Level(object[] objects)
        {
            int value = (int) objects[0];
            progressBar_mp.max =
                (float) RoleUpgradeDataAsset.Instance.GetExp(value + 1);
            leveltext.text = value.ToString();
        }
        private void SetPlayer1Damage(object[] objects)
        {
            float valueFinalAtk = (float) objects[0];
            float valueFinalDef = (float) objects[1];
            
            damage.text = valueFinalAtk.ToString();
            defenses.text = valueFinalDef.ToString();
        }
        private void SetPlayer1MovingSpeed(object[] objects)
        {
            float value = (float) objects[0];
            MovingSpeed.text = value.ToString();
        } 
        private void SetPlayer1FinalCritical(object[] objects)
        {
            float value = (float) objects[0];
            float valueDamage = (float) objects[1];
            
            Crit.text = value.ToString();
            CriticalDamage.text = valueDamage.ToString();
        }
        
    }
    
    public partial class NewRoleInfoUIController
    {
         private bool panelupvisible;
        public bool Panelupvisible
        {
            get
            {
                panelupvisible = panelup.visible;
                return panelupvisible;
            }
        }
        
        GTweener showAndHideTweeneralphadown;
        GTweener showAndHideTweenerdown;
        GTweener showAndHideTweener;
        GTweener showAndHidealphaTweener;

        //显示下栏 add by tianjinpeng 2018/02/08 10:35:47
        public void ShowRoleInfodown()
        {
            panelDownUI.Show();
        }

        //隐藏下栏显示 add by tianjinpeng 2018/02/08 10:34:52
        public void HideRoleInfodown()
        {
            panelDownUI.Hide();
        }
        
        //还原
        //显示上栏 add by tianjinpeng 2018/02/08 10:49:43
        public void ShowRoleInfoUI(bool withAnim = true)
        {
            panelupUI.visible = true;


            if (transition != null)
            {
                transition.Stop();
            }

            if (withAnim)
            {
                // if (showState == 0)
                {
                    showState = 1;

                    panelupUI.visible = true;

                    transition = ui.GetTransition("show");
                    transition.Play(() => { showState = 2; });
                }
            }
            else
            {
                panelupUI.visible = true;
                showState = 2;
            }
        }

        //隐藏上栏 add by tianjinpeng 2018/02/08 10:49:15
        public void HideRoleInfoUI(bool withAnim = true)
        {
            if (transition != null)
            {
                transition.Stop();
            }

            if (withAnim)
            {
                if (showState == 2 || showState == 1)
                {
                    showState = 1;

                    panelupUI.visible = true;
//                    panelup.visible = true;

                    transition = ui.GetTransition("hide");
                    transition.Play(() =>
                    {
                        showState = 0;
                        panelupUI.visible = false;
//                        panelup.visible = false;
                    });
                }
            }
            else
            {
                panelupUI.visible = false;
//                panelup.visible = false;
                showState = 0;
            }
        }

        public void RefreshRoleInfodownData(EquipData equipData, string text1 = "atk", string text2 = "def",
            string text3 = "atkspeed", AttrType attrType1 = AttrType.Atk, AttrType attrType2 = AttrType.Def,
            AttrType attrType3 = AttrType.Mass)
        {
            if (equipData != null)
            { 

            }
        }

        public void SetChoiceLoader(int index, bool show, string url, EquipData equipData, Action action)
        {
            choiceDataList.SetChoiceLoader(index, show, url, equipData, action);
        }

        public void ChoiceIndex(choicedirection choicedirection)
        {
            int newindex = choiceDataList.Choice(currindex, choicedirection);
            if (newindex != -1)
            {
                currindex = newindex;
            }
        }

        public void firstopen()
        {
            Vector2 pos = choiceDataList.GetIndexPos(currindex);
            choiceframe.position = new Vector3(pos.x - 8f, pos.y - 8f, 0);
            RefreshRoleInfodownData(choiceDataList.GetIndexEquipData(currindex));
            if (choiceDataList.GetChoiceLoadervisible(currindex))
            {
                ShowRoleInfodown();
            }
            else
            {
                HideRoleInfodown();
            }
        }

        public void StartRoleinfo()
        {
            Typedown = false;
            ShowRoleInfoUI();
            firstopen();
        }

        public void OverRoleinfo()
        {
            HideRoleInfoUI();
            if (panelDownUI.IsVisible() == true)
            {
                HideRoleInfodown();
            }

            Typedown = true;
        }



        string EquipTypeToChinese(EquipType equipType)
        {
            switch (equipType)
            {
                case EquipType.Armor:
                    return "盔甲";
                case EquipType.Blunt:
                    return "钝器";
                case EquipType.Bow:
                    return "弓箭";
                case EquipType.Decoration:
                    return "饰品";
                case EquipType.Glove:
                    return "手套";
                case EquipType.Helmet:
                    return "头盔";
                case EquipType.Katana:
                    return "武士刀";
                case EquipType.Lswd:
                    return "大剑";
                case EquipType.Necklace:
                    return "项链";
                case EquipType.None:
                    return "0";
                case EquipType.Ring:
                    return "戒指";
                case EquipType.Saxe:
                    return "短斧";
                case EquipType.Shield:
                    return "盾";
                case EquipType.Shoe:
                    return "鞋子";
//                case EquipType.Soul:
//                    return "魂";
                case EquipType.Spear:
                    return "长枪";
                case EquipType.Sswd:
                    return "5";
                case EquipType.Swd:
                    return "1";
                case EquipType.Trousers:
                    return "裤子";
            }

            return null;
        }

        string ShowAttrListChinese(AttrType attrType)
        {
            switch (attrType)
            {
                case AttrType.Hp:
                    return "血量"; //1
                case AttrType.Mp:
                    return "蓝量"; //2
                case AttrType.HpMax:
                    return "血量上限"; //3
                case AttrType.MpMax:
                    return "蓝量上限"; //4

                case AttrType.Atk:
                    return "攻击力"; //5
                case AttrType.AtkMin:
                    return "攻击力最小值"; //6
                case AttrType.AtkMax:
                    return "攻击力最大值"; //7

                case AttrType.MagicalMin:
                    return "法术强度最小值"; //8
                case AttrType.MagicalMax:
                    return "法术强度最大值"; //9

                case AttrType.Def:
                    return "物理防御力"; //10
                // case AttrType.AtkSpeed:
                //     return "攻击速度";//11
                // case AttrType.MoveSpeed:
                //     return "移动速度";//12
                case AttrType.CriticalRate:
                    return "暴击率"; //13
                case AttrType.CriticalDamage:
                    return "暴击伤害"; //14
                case AttrType.Tili:
                    return "体力"; //15
                case AttrType.TiliMax:
                    return "最大体力值"; //16
                case AttrType.WalkSpeed:
                    return "通常移动速度"; //17
                case AttrType.RunSpeed:
                    return "跑动速度"; //18

                case AttrType.AtkSpeedScale:
                    return "攻速倍率"; //19
                case AttrType.MoveSpeedScale:
                    return "移速倍率"; //20

                case AttrType.Poise:
                    return "韧性"; //21
                case AttrType.PoiseMax:
                    return "韧性最大值"; //22
                case AttrType.PoiseCut:
                    return "韧性削减值"; //23
                case AttrType.PoiseScale:
                    return "韧性倍率"; //24
                case AttrType.Mass:
                    return "质量"; //25
                case AttrType.HpPercent:
                    return "当前血量百分比"; //26
                case AttrType.SuperArmor:
                    return "霸体"; //27
                case AttrType.RushSpeed:
                    return "冲刺速度"; //28

            }

            return "";
        }

        void AttributeShow<T>(EquipData itemData) where T : EquipData
        {
            panelDownUI.SetTitle(itemData.name);

            // 显示描述desc
            {
                panelDownUI.SetDesc(itemData.desc);
                panelDownUI.SetLV(itemData.level.ToString());
                panelDownUI.SetKind(EquipTypeToChinese(itemData.equipType));
            }

            // 设置图标
            {
                panelDownUI.SetIcon(itemData.icon);
            }

            // 显示属性
            {
                List<KeyValuePair<AttrType, float>> showAttrList = new List<KeyValuePair<AttrType, float>>();

                foreach (AttrType at in Enum.GetValues(typeof(AttrType)))
                {
                    float value = itemData.GetAttr(at);
                    if (value > 0)
                        showAttrList.Add(new KeyValuePair<AttrType, float>(at, value));
                }

                showAttrList.Sort((KeyValuePair<AttrType, float> a, KeyValuePair<AttrType, float> b) =>
                {
                    if (a.Value < b.Value)
                    {
                        return 1;
                    }
                    else if (a.Value > b.Value)
                    {
                        return -1;
                    }

                    return 0;
                });

                foreach (var item in showAttrList)
                {
                    Debug.Log("item.key = " + item.Key + ", value = " + item.Value);
                }


                if (showAttrList.Count >= 3)
                {
                    //var pair = showAttrList[0];
                    //panelDownUI.SetATK(pair.Key);
                    //panelDownUI.Setatk(pair.Value.ToString());
                    var pair = showAttrList[0];
                    panelDownUI.SetATK(ShowAttrListChinese(pair.Key), pair.Value.ToString(), " ");

                    //pair = showAttrList[1];
                    //panelDownUI.SetACC(pair.Key);
                    //panelDownUI.Setacc(pair.Value.ToString());
                    pair = showAttrList[1];
                    panelDownUI.SetACC(ShowAttrListChinese(pair.Key), pair.Value.ToString(), " ");


                    //pair = showAttrList[2];
                    //panelDownUI.SetEND(pair.Key);
                    //panelDownUI.Setend(pair.Value.ToString());
                    pair = showAttrList[2];
                    panelDownUI.SetEND(ShowAttrListChinese(pair.Key), pair.Value.ToString(), " ");
                }
                else if (showAttrList.Count == 2)
                {
                    //var pair = showAttrList[0];
                    //panelDownUI.SetATK(pair.Key);
                    //panelDownUI.Setatk(pair.Value.ToString());
                    var pair = showAttrList[0];
                    panelDownUI.SetATK(ShowAttrListChinese(pair.Key), pair.Value.ToString(), " ");

                    //pair = showAttrList[1];
                    //panelDownUI.SetACC(pair.Key);
                    //panelDownUI.Setacc(pair.Value.ToString());
                    pair = showAttrList[1];
                    panelDownUI.SetACC(ShowAttrListChinese(pair.Key), pair.Value.ToString(), " ");

                    ////                pair = showAttrList[2];
                    //panelDownUI.SetEND("");
                    //panelDownUI.Setend("");
                    panelDownUI.SetEND("", "", "");
                }
                else if (showAttrList.Count == 1)
                {
                    //var pair = showAttrList[0];
                    //panelDownUI.SetATK(pair.Key);
                    //panelDownUI.Setatk(pair.Value.ToString());
                    var pair = showAttrList[0];
                    panelDownUI.SetATK(ShowAttrListChinese(pair.Key), pair.Value.ToString(), " ");

                    //                pair = showAttrList[1];
                    //panelDownUI.SetACC("");
                    //panelDownUI.Setacc("");
                    panelDownUI.SetACC("", "", "");


                    ////                pair = showAttrList[2];
                    //panelDownUI.SetEND("");
                    //panelDownUI.Setend("");
                    panelDownUI.SetEND("", "", "");
                }
                else if (showAttrList.Count == 0)
                {
                    //                var pair = showAttrList[0];
                    //panelDownUI.SetATK("");
                    //panelDownUI.Setatk("");
                    panelDownUI.SetATK("", "", "");


                    ////                pair = showAttrList[1];
                    //panelDownUI.SetACC("");
                    //panelDownUI.Setacc("");
                    panelDownUI.SetACC("", "", "");

                    ////                pair = showAttrList[2];
                    //panelDownUI.SetEND("");
                    //panelDownUI.Setend("");
                    panelDownUI.SetACC("", "", "");
                }

            }
        }

        public string Addrore(float f)
        {
            if (f > 0)
            {
                return "+";
            }
            else if (f < 0)
            {
                return "-";
            }
            else
            {
                return "";
            }
        }

        TriggerController otherTriggerController;

        bool Typedown = true;

        //人物界面弹窗物品信息
        void ShowPlayerInterface()
        {
            valueMonitorPool.AddMonitor((Func<TriggerController>)(() =>
            {
                GameObject player = GameObject.Find("Player1");
                TriggerController selfTriggerController = player.GetComponentInChildren<TriggerController>();
                otherTriggerController = selfTriggerController.GetFirstKeepingTriggerController();
                return otherTriggerController;
            }), (TriggerController from, TriggerController to) =>
            {
                if (Typedown)
                {
                    if (otherTriggerController != null)
                    {
                        GameObject interactObject = otherTriggerController.ITriggerDelegate.GetGameObject();
                        if (interactObject != null)
                        {
                            DropItemController dropItemController = interactObject.GetComponent<DropItemController>();
                            DoorController doorController = interactObject.GetComponent<DoorController>();
                            if (dropItemController != null)
                            {
                                var itemData = ItemManager.Instance.getItemDataById<ItemData>(dropItemController.ItemId);
                                if (itemData != null)
                                {
                                    GameObject player = GameObject.Find("Player1");
                                    RoleController roleController = player.GetComponent<RoleController>();
                                    if (itemData is DecorationData)//现阶段物品掉落使用
                                    {
                                        DecorationData RoleDecorationData;
                                        AttributeShow<DecorationData>(itemData as EquipData);
                                        ShowRoleInfodown();
                                    }
                                    else
                                    {
                                        HideRoleInfodown();
                                    }
                                }
                            }
                            else if (doorController != null)
                            {
                                Debug.Log("door");
                            }
                        }

                    }
                    else
                    {
                        HideRoleInfodown();
                    }
                }
            });
        }

        //掉落物品信息 add by tianjinpeng 2018/02/08 10:23:58
        
        public void UnEquip()
        {
            choiceDataList.Unequia(currindex);
        }
        // Update is called once per frame
        void Update()
        {
            valueMonitorPool.Update();
        }
        int showState = 0;
        private Transition transition;

        public void Show(bool withAnim = true)
        {

            ShowRoleInfoUI();

            panelDownUI.Show();
            
        }

        public void Hide(bool withAnim = false)
        {

            HideRoleInfoUI();

            panelDownUI.Hide();
        }   
        public bool IsShow()
        {
            throw new NotImplementedException();
        }
        
    }
}