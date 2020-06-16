using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;

namespace Tang
{
    public class StoreUIController : MonoBehaviour,UIInterface
    {
        public StoreController storeController;
        int leftindex;
        public int leftright;
        public int rightindex;
        public int storebuyindex;
        //GList leftlist;
        GImage leftchoice;
        GImage tisheng;
        GLoader n1;
        GLoader n2;
        GLoader n3;
        GLoader n4;
        GLoader buy1;
        GLoader buy2;
        GRichTextField t1;
        GRichTextField t2;
        GRichTextField t3;
        GRichTextField t4;
        GTextField buytext;
        GTextField namedown;
        GTextField descdown;
        GComponent ui;
        GComponent storeup;
        GComponent storedown;
        GComponent storebuy;
        List<GLoader> LeftList = new List<GLoader>();
        float storebuyfirsty;
        public Action offFsm;
        bool das;
        List<GRichTextField> textlist = new List<GRichTextField>();
        //消耗品列表 add by tianjinpeng 2018/03/09 15:04:48
        List<BagData> xiaohaopinList = new List<BagData>()
        {
            new BagData(0, "血药", "GoldCoin", 100,"大力药水"),
            new BagData(0, "血药", "GoldCoin", 100,"大力药水"),
            new BagData(0, "血药", "GoldCoin", 100,"大力药水"),
            new BagData(0, "血药", "GoldCoin", 100,"大力药水"),
            new BagData(0, "血药", "GoldCoin", 100,"大力药水"),
            new BagData(0, "血药", "GoldCoin", 100,"大力药水"),
            new BagData(0, "血药", "GoldCoin", 100,"大力药水"),
            new BagData(0, "血药", "GoldCoin", 100,"大力药水"),
            new BagData(0, "血药", "GoldCoin", 100,"大力药水"),
            new BagData(0, "血药", "GoldCoin", 100,"大力药水"),
        };
        
        //装备列表 add by tianjinpeng 2018/03/09 15:05:10
        List<BagData> euqiaList = new List<BagData>()
        {
            new BagData(0, "大剑", "lswd-1", 50,""),
            new BagData(1, "剑", "swd-1", 50,""),
            new BagData(0, "大剑", "lswd-1", 50,""),
            new BagData(1, "剑", "swd-1", 50,""),
            new BagData(0, "大剑", "lswd-1", 50,""),
            new BagData(1, "剑", "swd-1", 50,""),
            new BagData(0, "大剑", "lswd-1", 50,""),
            new BagData(1, "剑", "swd-1", 50,""),
            new BagData(0, "大剑", "lswd-1", 50,""),
            new BagData(1, "剑", "swd-1", 50,""),
            new BagData(0, "大剑", "lswd-1", 50,""),
            new BagData(1, "剑", "swd-1", 50,""),
            new BagData(0, "大剑", "lswd-1", 50,""),
            new BagData(1, "剑", "swd-1", 50,""),
            new BagData(0, "大剑", "lswd-1", 50,""),
            new BagData(1, "剑", "swd-1", 50,""),
                             
        };
        List<BagData> fanjuList = new List<BagData>();//防具列表 add by tianjinpeng 2018/03/09 15:05:34
        List<BagData> shipingList = new List<BagData>();//饰品列表 add by tianjinpeng 2018/03/09 15:06:08
        List<GLoader> buylist=new List<GLoader>();
        public void Init()
        {  
            storeController = this.gameObject.AddComponent<StoreController>();
            storeController.Init();
            ui = GetComponent<UIPanel>().ui;
            storeup = ui.GetChild("n5").asCom;//选项"是"
            storedown = ui.GetChild("n6").asCom;//选项"否"
            storebuy=ui.GetChild("n7").asCom;
            n1 = storeup.GetChild("left1").asLoader;//消耗品
            n2 = storeup.GetChild("left2").asLoader;//武器
            n3 = storeup.GetChild("left3").asLoader;//防具
            n4 = storeup.GetChild("left4").asLoader;//饰品
            t1 = storeup.GetChild("lefttext1").asRichTextField;
            t2 = storeup.GetChild("lefttext2").asRichTextField;
            t3 = storeup.GetChild("lefttext3").asRichTextField;
            t4 = storeup.GetChild("lefttext4").asRichTextField;
            tisheng=storeup.GetChild("tisheng").asImage;
            namedown=storedown.GetChild("n1").asTextField;//storedown物品名称
            descdown=storedown.GetChild("n3").asTextField;//storedown物品描述
            buy1=storebuy.GetChild("n5").asLoader;
            buy2=storebuy.GetChild("n6").asLoader;
            buytext=storebuy.GetChild("n2").asTextField;
            tisheng.visible=false;

            LeftList.Add(n1);
            LeftList.Add(n2);
            LeftList.Add(n3);
            LeftList.Add(n4);
            textlist.Add(t1);
            textlist.Add(t2);
            textlist.Add(t3);
            textlist.Add(t4);
            buylist.Add(buy1);
            buylist.Add(buy2);
            t1.onClick.Add(()=>
            {
                leftindex=0;
                onclick();
            });
            t2.onClick.Add(()=>
            {
                leftindex=1;
                onclick();
            });
            t3.onClick.Add(()=>
            {
                leftindex=2;
                onclick();
            });
            t4.onClick.Add(()=>
            {
                leftindex=3;
                onclick();
            });
            buy1.onClick.Add(()=>
            {
                storebuyindex=0;
                buychoice(storebuyindex);
                leftright=0;
                storeController.closechoice();
                openchoice();
                storeController.choice0();
                storebuy.visible=false;
                offFsm();
            });
            buy2.onClick.Add(()=>
            {
                storebuyindex=1;
                buychoice(storebuyindex);
                leftright=1;
                storebuy.visible=false;
            });

            leftchoice = storeup.GetChild("Choice").asImage;
            
            leftright = 0;
            leftindex = 0;
            storedown.visible=false;
            storebuy.visible=false;
            storeup.visible=false;
            storebuyfirsty=storeup.x;
            das=false;
            Hide();
            
            
            storeController.onoff = (int obj) =>
            {
                //leftright = obj;
                if (obj == 0)
                {
                    //openchoice();
                    choiceleftpos(leftindex);
                    AnimManager.Instance.Movesize(tisheng,leftchoice,storeController.choicevector2(),new Vector2(LeftList[leftindex].position.x+ 1, LeftList[leftindex].position.y),0.1f);
                }else{
                    if(obj!=leftright){
                        closechoice();
                        AnimManager.Instance.Movesize(tisheng,storeController.choice,leftchoice.position,storeController.choicenow(),0.1f);
                    }
                    showbuy();
                }
                leftright = obj;
            };
            storeController.show=(int index)=>
            {
                rightindex=index;
               
                BagData bagData;
                if (storeController.bagDataList.TryGet(index, out bagData))
                {
                    namedown.text=bagData.name;
                    descdown.text=bagData.desc;
                }
                if(storeController.leftright==0){
                    closestoredowntw();
                }else{
                    openstoredowntw();
                }
            };
            //iconchoice(0);
            choiceleftpos(leftindex);
            Import();
        }
        



        //为外部提供显示隐藏 add by tianjinpeng 2018/03/09 15:07:15
        GTweener storeuptweener;
        GTweener storedowntweener;
        
        
        void openstoreuptw(){
            killtweener(storeuptweener);
            storeup.visible=true;
            storeuptweener=storeup.TweenMoveY(22,0.2f).OnComplete(() =>{});;
        }
        void openstoredowntw(){
            killtweener(storedowntweener);
            storedown.visible=true;
            storedowntweener=storedown.TweenMoveY(895,0.2f).OnComplete(() =>{});;
        }
        void closestoreuptw(){
            killtweener(storeuptweener);
            storeuptweener=storeup.TweenMoveY(0,0.2f).OnComplete(() =>{storeup.visible=false;});;
        }
        void closestoredowntw(){
            killtweener(storedowntweener);
            storedowntweener=storedown.TweenMoveY(1080,0.2f).OnComplete(() =>{storedown.visible=false;});;
        }
        //停止动画进程 add by tianjinpeng 2018/02/08 10:36:42
        void killtweener(GTweener tweener)
        {
            if (tweener != null)
            {
                GTween.Kill(tweener);
                tweener = null;
            }
            else
            {

            }

        }

        //选择上移 add by tianjinpeng 2018/03/09 15:08:46
        public void up()
        {
            switch (leftright)
            {
                case 0:
                    upleftlist();
                    storeController.choice0();
                    break;
                case 1:
                    storeController.up();
                    break;
            }
        }
        //选择下移 add by tianjinpeng 2018/03/09 15:09:08
        public void down()
        {
            switch (leftright)
            {
                case 0:
                    downleftlist();
                    storeController.choice0();
                    break;
                case 1:
                    // uprightlist();
                    storeController.down();
                    break;
            }
        }
        //选择左移 add by tianjinpeng 2018/03/09 15:09:48
        public void left()
        {
            switch (leftright)
            {
                case 0:
                    // upleftlist();
                    break;
                case 1:
                    storeController.left();
                    break;
                case 2:
                    buyleft();
                    break;
            }
        }
        //选择右移 add by tianjinpeng 2018/03/09 15:10:10
        public void right()
        {
            switch (leftright)
            {
                case 0:
                    // upleftlist();
                    if (storeController.panduan())
                    {
                        leftright = 1;
                        storeController.leftright = 1;
                        storeController.shuanindex();
                        closechoice();
                        AnimManager.Instance.Movesize(tisheng,storeController.choice,leftchoice.position,storeController.choicenow(),0.1f);
                        //storeController.openchoice();
                    }
                    break;
                case 1:
                    storeController.right();
                    break;
                case 2:
                    buyright();
                    break;
            }
        }
        //左列表选择框上移 add by tianjinpeng 2018/03/09 15:11:05
        void upleftlist()
        {
            leftindex--;
            if (leftindex < 0) { leftindex = 3; }
            choiceleftpos(leftindex);
            //iconchoice(leftindex);

        }
        //左列表选择框下移 add by tianjinpeng 2018/03/09 15:11:59
        void downleftlist()
        {
            leftindex++;
            if (leftindex > 3) { leftindex = 0; }
            choiceleftpos(leftindex);
            //iconchoice(leftindex);
        }
        //隐藏选择框 add by tianjinpeng 2018/03/14 10:53:16
        void closechoice()
        {
            leftchoice.visible=false;
        }
        //显示选择框 add by tianjinpeng 2018/03/14 10:54:42
        void openchoice()
        {
            leftchoice.visible=true;
        }
        void buyleft()
        {
            storebuyindex--;
            if(storebuyindex<0){storebuyindex=1;}
            buychoice(storebuyindex);
        }
        void buyright()
        {
            storebuyindex++;
            if(storebuyindex>1){storebuyindex=0;}
            buychoice(storebuyindex);
        }
        void buychoice(int ddg)
        {
            int i;
            for (i = 0; i < 2; i++)
            {
                if (ddg==i)
                {
                    buylist[i].url="ui://UI/choicefr1";
                }else{
                    buylist[i].url="ui://UI/hui";
                }
            }
        }
        //左选择框位置调整 add by tianjinpeng 2018/03/09 15:13:37
        GTweener choicemove;
        void choiceleftpos(int qwe)
        {
            Vector2 screenPos = new Vector2(LeftList[qwe].position.x+ 1, LeftList[qwe].position.y);
            //leftchoice.position = new Vector2(screenPos.x, screenPos.y);
            killtweener(choicemove);
            choicemove=leftchoice.TweenMove(screenPos,0.1f);
            int i;
            for (i = 0; i < 4; i++)
            {
                if (qwe == i) { LeftList[i].visible = true; textlist[i].alpha = 1f; } else { LeftList[i].visible = false; textlist[i].alpha = 0.3f; }
            }
            Import();
        }
        //导入当前选择列表 add by tianjinpeng 2018/03/09 15:14:41
        void Import()
        {
            switch (leftindex)
            {
                case 0:
                    storeController.listcount(xiaohaopinList.Count,xiaohaopinList);
                    break;
                case 1:
                    storeController.listcount(euqiaList.Count,euqiaList);
                    break;
                case 2:
                    storeController.listcount(fanjuList.Count,fanjuList);
                    break;
                case 3:
                    storeController.listcount(shipingList.Count,shipingList);
                    break;
            }

        }
        //显示购买确认界面 add by tianjinpeng 2018/03/09 18:00:24
        public void showbuy(){
            leftright=2;
            storebuy.visible=true;
            buychoice(storebuyindex);
            BagData bagData;
            if (storeController.bagDataList.TryGet(rightindex, out bagData))
            {
                buytext.text=bagData.name;
            }
        }
        //enter按键 add by tianjinpeng 2018/03/09 18:13:31
        public void Enter()
        {
            if(leftright==1){
                showbuy();
            }else if(leftright==2){
                storebuy.visible=false;
                if(storebuyindex==0)
                {
                    leftright=0;
                    storeController.closechoice();
                    openchoice();
                    storeController.choice0();
                    //storeController.leftright=0;
                }else{
                    leftright=1;
                    storebuy.visible=false;
                }
            }
        }
        //鼠标单击事件 add by tianjinpeng 2018/03/14 10:37:07
        void onclick()
        {
            leftright=0;
            choiceleftpos(leftindex);
            storeController.closechoice();
            if(storeController.leftright != 0)
            {
                //storeController.leftright = 1;
                AnimManager.Instance.Movesize(tisheng,leftchoice,storeController.choicevector2(),new Vector2(LeftList[leftindex].position.x+ 1, LeftList[leftindex].position.y),0.1f);
            }
            storeController.leftright = 0;
            storeController.choice0();
            //openchoice();
        }

        public void Show(bool withAnim = true)
        {
            ui.visible = true;
            storeup.visible=true;
            openstoreuptw();
        }

        public void Hide(bool withAnim = true)
        {
            killtweener(storeuptweener);
            storeuptweener=storeup.TweenMoveY(0,0.2f).OnComplete(() =>
            {
                ui.visible = false;
                storeup.visible=false;
            });;
            if(storedown.visible==true){
                killtweener(storedowntweener);
                storedowntweener=storedown.TweenMoveY(1080,0.2f).OnComplete(() =>
                {
                    storedown.visible=false;
                });;
            }
        }

        public bool IsShow()
        {
            if(ui.visible = true)
                return true;
            else
                return false;
        }
        // IEnumerator waitMove(float time, System.Action action)
        // {
        //     for (float timer = time; timer >= 0; timer -= Time.deltaTime)
        //     {
        //         yield return 0;
        //     }
        //     action();
        // }


    }
}