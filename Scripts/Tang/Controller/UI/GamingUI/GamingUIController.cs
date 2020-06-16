using System.Collections.Generic;
using UnityEngine;
using System;
using FairyGUI;
using DG.Tweening;
using JetBrains.Annotations;

namespace Tang
{
    public class TipsData
    {
        public GComponent taget;
        public GLoader icon;
        public GTextField name;
        public GTextField count;
        public GTweener tweener;
        public Tweener totweener;
        public bool apha = false;
        
        public TipsData(GComponent game, GLoader lo, GTextField na,GTextField co)
        {
            taget = game;
            icon = lo;
            name = na;
            count = co;
        }
        
        public void Setdata(string iconurl,string nametext,string datacount)
        {
            icon.url = iconurl;
            name.text = nametext;
            count.text = datacount;
            taget.visible = true;
            apha = true;
        }
        
        public void starttweener()
        {
            tweener = taget.TweenFade(0f, 1f).OnComplete(()=> 
            {
                taget.visible = false;
                apha = false;
            });
        }
    }
    
    public class TipsDatalist
    {
        public List<TipsData> tipsDatas = new List<TipsData>();
        public GList gList;
        public GTweener tweener;
        public void CanToZero()
        {
            bool canorno = true;
            foreach (var item in tipsDatas)
            {
                if (item.apha == true)
                {
                    canorno = false;
                }
            }
            if (canorno)
            {
                gList.visible = false;
                gList.y = 300f;
                gList.visible = true;
                tipsDatas.Clear();
                gList.RemoveChildrenToPool();
            }
        }
        
        public void Add(string iconurl, string nametext, string datacount)
        {
            CanToZero();
            float toy = 300f -(( tipsDatas.Count + 1)*100f);
            newtipsData(iconurl, nametext, datacount);
            if (tweener != null)
            {
                GTween.Kill(tweener);
            }
            tweener = gList.TweenMoveY(toy, 0.3f);
        }
        
        public void newtipsData(string iconurl, string nametext, string datacount)
        {
            GComponent gComponent=gList.AddItemFromPool().asCom;
            gComponent.visible = true;
            gComponent.alpha = 1;
            TipsData tipsData = new TipsData(gComponent, gComponent.GetChild("n0").asLoader, gComponent.GetChild("n1").asTextField, gComponent.GetChild("n2").asTextField);
            gList.ResizeToFit(gList.numChildren);
            gList.EnsureBoundsCorrect();
            tipsData.Setdata(iconurl, nametext, datacount);
            tipsDatas.Add(tipsData);
            tipsData.starttweener();
        }
        //public void setnew
    }

    public class GamingUIController : MyMonoBehaviour, UIInterface
    {
//        RoleController player1Controller;
        ValueMonitorPool valueMonitorPool = new ValueMonitorPool();
        Tweener tweener_progressBar_hp;
        Tweener tweener_progressBar_hpMax;
        Tweener tweener_progressBar_mp;
        Tweener tweener_progressBar_mpMax;
        Tweener tweener_progressBar_tili;
        Tweener tweener_progressBar_tiliMax;
        GComponent functionlist;
        GComponent bosshpcomponent;


        private GProgressBar progressBar_hp;
        private GProgressBar progressBar_mp;
        private GProgressBar progressBar_tili;
        
        private GTextField money;
        private GTextField soul;
        private GImage image_soul;

        private GComponent comboComponent;
        private GTextField comboNumber;
        
        bool inited = false;
        GComponent _mainView;
        GList _list;
        ScrollPane scrollPane;
        TipsDatalist tipsDatalist = new TipsDatalist();
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
        private void OnEnable()
        {
            MessageManager.Instance.Subscribe(MessageName.SET_PLAYER1_VIGOR, SetPlayer1Vigor);
            MessageManager.Instance.Subscribe(MessageName.SET_PLAYER1_HP, SetPlayer1Hp);
            MessageManager.Instance.Subscribe(MessageName.SET_PLAYER1_MP, SetPlayer1Mp);
            MessageManager.Instance.Subscribe(MessageName.SET_PLAYER1_Money, SetPlayer1Money);
            MessageManager.Instance.Subscribe(MessageName.SET_PLAYER1_Soul, SetPlayer1Soul);
            MessageManager.Instance.Subscribe(MessageName.SET_PLAYER1_SoulIcon, SetPlayer1SoulIcon);
            MessageManager.Instance.Subscribe(MessageName.SET_PLAYER1_ConsumableItemList, SetPlayer1ConsumableItem);
            MessageManager.Instance.Subscribe(MessageName.SET_PLAYER1_CurrConsumableItemIndex, SetPlayer1CurrConsumableItemIndex);
            MessageManager.Instance.Subscribe(MessageName.SET_PLAYER1_Combo, SetPlayer1Combo);
            
        }

        private void OnDisable()
        {
            MessageManager.Instance.Unsubscribe(MessageName.SET_PLAYER1_VIGOR, SetPlayer1Vigor);
            MessageManager.Instance.Unsubscribe(MessageName.SET_PLAYER1_HP, SetPlayer1Hp);
            MessageManager.Instance.Unsubscribe(MessageName.SET_PLAYER1_MP, SetPlayer1Mp);
            MessageManager.Instance.Unsubscribe(MessageName.SET_PLAYER1_Money, SetPlayer1Money);
            MessageManager.Instance.Unsubscribe(MessageName.SET_PLAYER1_Soul, SetPlayer1Soul);
            MessageManager.Instance.Unsubscribe(MessageName.SET_PLAYER1_SoulIcon, SetPlayer1SoulIcon);
            MessageManager.Instance.Unsubscribe(MessageName.SET_PLAYER1_ConsumableItemList, SetPlayer1ConsumableItem);
            MessageManager.Instance.Unsubscribe(MessageName.SET_PLAYER1_CurrConsumableItemIndex, SetPlayer1CurrConsumableItemIndex);
            MessageManager.Instance.Unsubscribe(MessageName.SET_PLAYER1_Combo, SetPlayer1Combo);
        }

        private void Start()
        {
            Init();
        }
        public void Init()
        {
            inited = true;

            valueMonitorPool.Clear();

            // player1 
            {
                // 血条 and 蓝条 add by TangJian 2017/11/14 22:35:21
                {
                    progressBar_hp = Ui.GetChildWithPath("Player1_State/ProgressBar_HP").asProgress;
                    progressBar_mp = Ui.GetChildWithPath("Player1_State/ProgressBar_MP").asProgress;
                    progressBar_tili = Ui.GetChildWithPath("Player1_State/ProgressBar_TiLi").asProgress;
                }
                
                //金钱数量和魂数量
                {
                    GComponent MoneyData = Ui.GetChild("MoneyData").asCom;
                    GLoader momeyimg = MoneyData.GetChild("moneyimg").asLoader;
                    GLoader soulimg = MoneyData.GetChild("Soulimg").asLoader; 
                    money = MoneyData.GetChild("money").asTextField; 
                    soul = MoneyData.GetChild("Soul").asTextField;
                }

                // 魂 add by TangJian 2017/11/14 22:35:33
                { 
                    image_soul = Ui.GetChild("Player1_Soul").asImage;
                }
                
                //消耗品循环列表add by tianjinpeng 2018/02/05 16:12:58
                {

                    _mainView = Ui.GetChild("scrollpane").asCom;
                    _list = _mainView.GetChild("list").asList;

                }
                
                {
                    functionlist = Ui.GetChild("functionlist").asCom;
                    GComponent roleinfobc = functionlist.GetChild("n1").asCom;
                    GComponent mapbut= functionlist.GetChild("n2").asCom;
                    roleinfobc.onClick.Add(() => { GameManager.Instance.RoleinfoButton(); });
                }
                { 
                    comboComponent = Ui.GetChild("comboShu").asCom; 
                    comboNumber = comboComponent.GetChild("n0").asTextField;
                    comboComponent.visible = false;
                }
                
                {
                    GComponent listtips = Ui.GetChild("n27").asCom;
                    listtips.container.gameObject.layer = LayerMask.NameToLayer("UI");
                    GList slist = listtips.GetChild("n7").asList;
                    slist.container.gameObject.layer = LayerMask.NameToLayer("UI");
                    tipsDatalist.gList = slist;
                }
            }
        }

        private void SetPlayer1Vigor(object[] objects)
        {
            float value = (float)objects[0];
            float max = (float)objects[1];

            progressBar_tili.max = max;
            
            tweener_progressBar_tili?.Kill();
            tweener_progressBar_tili = progressBar_tili.DoValue(value, (value - (float)progressBar_tili.value) / max);
            tweener_progressBar_tili.OnComplete(() =>
            {
                tweener_progressBar_tili = null;
            });
        }

        private void SetPlayer1Hp(object[] objects)
        {
            float value = (float)objects[0];
            float max = (float)objects[1];

            progressBar_hp.max = max;

            tweener_progressBar_hp?.Kill();
            tweener_progressBar_hp = progressBar_hp.DoValue((float)value, (value - (float)progressBar_hp.value) / max);
            tweener_progressBar_hp.OnComplete(() =>
            {
                tweener_progressBar_hp = null;
            });
        }

        private void SetPlayer1Mp(object[] objects)
        {
            float value = (float)objects[0];
            float max = (float)objects[1];

            progressBar_mp.max = max;

            tweener_progressBar_mp?.Kill();
            tweener_progressBar_mp = progressBar_mp.DoValue((float)value, (value - (float)progressBar_mp.value) / max);
            tweener_progressBar_mp.OnComplete(() =>
            {
                tweener_progressBar_mp = null;
            });
        }

        private void SetPlayer1Money(object[] objects)
        {
            float value = (float) objects[0];

            money.text = value.ToString();
        }
        
        private void SetPlayer1Soul(object[] objects)
        {
            float value = (float) objects[0];

            soul.text = value.ToString();
        }
        private async void SetPlayer1SoulIcon(object[] objects)
        {
            string value = (string) objects[0];
            if (!string.IsNullOrEmpty(value))
            {
                var tex = AssetManager.LoadAssetAsync<Texture>("Assets/Resources_moved/Textures/Icon/" + value+".png");
                Texture texture = await tex;
                image_soul.texture = new NTexture(texture);
            }
        }

        private List<ConsumableData> ConsumableDatas;
        private void SetPlayer1ConsumableItem([NotNull] object[] objects)
        {
            if (objects == null) throw new ArgumentNullException(nameof(objects));
            List<ConsumableData> value = (List<ConsumableData>) objects[0];

            ConsumableDatas = value;
            _list.SetVirtualAndLoop();
            _list.itemRenderer = RenderListItem;
            _list.numItems = 1;
            _list.scrollPane.onScroll.Add(DoSpecialEffect);
            DoSpecialEffect();
            scrollPane = _list.scrollPane;
            scrollPane.scrollStep = 93f;
            
            if (value.Count < 1)
            {
                _list.numItems = 1;
                _list.RefreshVirtualList();
            }
            else
            {
                _list.numItems = value.Count;
            }

            {
                ConsumableData consumableData;
                ListExtend.TryGet<ConsumableData>(value,
                    (int) ((_list.GetFirstChildInView() + 1) % _list.numItems), out consumableData);
                _list.RefreshVirtualList();    
            }
        }

        private void SetPlayer1CurrConsumableItemIndex(object[] objects)
        {
            int value = (int)objects[0];
            if (value != (_list.GetFirstChildInView() + 1) % _list.numItems)
            {
                value = (_list.GetFirstChildInView() + 1) % _list.numItems;

            }
        }

        private void SetPlayer1Combo(object[] objects)
        {
            int value = (int) objects[0];
            
            if (value == 0)
            {
                comboComponent.visible = false;
            }
            else
            {
                comboComponent.visible = true;
                comboNumber.text = value.ToString();
            }
        }
        
        
        //放大图标动画 add by tianjinpeng 2018/02/07 16:21:52
        void DoSpecialEffect()
        {
            //change the scale according to the distance to middle
            float midX = _list.scrollPane.posX + _list.viewWidth / 2;
            int cnt = _list.numChildren;
            for (int i = 0; i < cnt; i++)
            {
                GObject obj = _list.GetChildAt(i);
                float dist = Mathf.Abs(midX - obj.x - obj.width / 2);
                if (dist > obj.width) //no intersection
                    obj.SetScale(1, 1);
                else
                {
                    float ss = 1 + (1 - dist / obj.width) * 0.24f;
                    obj.SetScale(ss, ss);
                }
            }
            _mainView.GetChild("n3").text = "" + ((_list.GetFirstChildInView() + 1) % _list.numItems);
        }
        
        //循环列表载入 add by tianjinpeng 2018/02/07 16:22:39
        void RenderListItem(int index, GObject obj)
        {
            GButton item = (GButton)obj;
            item.SetPivot(0.5f, 0.5f);
            GLoader sda = item.GetChild("icon").asLoader;
            sda.url = ConsumableItemListpanduan(index, "icon");
            item.text = ConsumableItemListpanduan(index, "count");
        }
        
        //载入路径 add by tianjinpeng 2018/02/07 16:23:37
        string ConsumableItemListpanduan(int index, string name)
        {
            ConsumableData consumableDataw;
            if (ConsumableDatas.TryGet(index, out consumableDataw))
            {
                if (name == "icon")
                {
                    return "Textures/Icon/" + consumableDataw.icon;
                    //"Textures/Icon/"+consumableDataw.icon;
                }
                else if (name == "count")
                {
                    return "" + consumableDataw.count;
                }
                else { return ""; }
            }
            else { return ""; }

            //return "";
        }
        
        public bool GamingUIvisible()
        {
            if (_mainView.visible == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void RefreshGlist()
        {//刷新循环列表add by tianjinpeng 2018/02/06 11:25:22
            _list.RefreshVirtualList();
        }
        
        public void Leftslide()
        {
            scrollPane.ScrollRight(-1, true);
        }
        
        public void Rightslide()
        {
            scrollPane.ScrollRight(1, true);
        }

        public void PickupTips(string iconurl, string name, string count)
        {
            tipsDatalist.Add(iconurl, name, count);
        }

        public override void Update()
        {
            valueMonitorPool.Update();
        }
        

        int showState = 0;
        private Transition transition;
        private GComponent MoneyData;
        private GComponent Player1_State;

        public void Show(bool withAnim = false)
        {
            if (!inited) return;

            AnimManager.Instance.GmoveXFode(_mainView, 65f, 1f, 0.2f, true);
            
            MoneyData = ui.GetChild("MoneyData").asCom;
            Player1_State = ui.GetChild("Player1_State").asCom;
            if (transition != null)
            {
                transition.Stop();
            }

            if (withAnim)
            {
                {
                    showState = 1;

                    MoneyData.visible = true;
                    Player1_State.visible = true;

                    transition = ui.GetTransition("ShowMoney");
                    transition = ui.GetTransition("ShowPlayer1");
                    transition.Play(() =>
                    {
                        showState = 2;
                    });
                }
            }
            else
            {
                MoneyData.visible = true;
                Player1_State.visible = true;
                showState = 2;
            }
        }

        public void Hide(bool withAnim = false)
        {
            if (!inited) return;

            AnimManager.Instance.GmoveXFode(_mainView, 0f, 0f, 0.2f, false);

            MoneyData = ui.GetChild("MoneyData").asCom;
            Player1_State = ui.GetChild("Player1_State").asCom;

            if (transition != null)
            {
                transition.Stop();
            }

            if (withAnim)
            {
                if (showState == 2 || showState == 1)
                {
                    showState = 1;

                    MoneyData.visible = true;
                    Player1_State.visible = true;

                    transition = ui.GetTransition("HideMoney");
                    transition = ui.GetTransition("HidePlayer1");
                    transition.Play(() =>
                    {
                        showState = 0;
                        MoneyData.visible = false;
                        Player1_State.visible = false;
                    });
                }
            }
            else
            {
                MoneyData.visible = false;
                Player1_State.visible = false;
                showState = 0;
            }
        }

        public bool IsShow()
        {
            throw new NotImplementedException();
        }
    }
}