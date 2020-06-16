using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;
namespace Tang
{
    public class BagData
    {
        public BagData(int index, string name, string icon, int price, string desc)
        {
            this.index = index;
            this.name = name;
            this.icon = icon;
            this.price = price;
            this.desc = desc;
        }

        public int index;
        public string desc;
        public string name;
        public string icon;
        public int price;
        public ItemType itemType;

    }
    public class BagUIController : MonoBehaviour
    {
        // public class BagData
        // {
        //     public BagData(int index, string name, string icon, int price)
        //     {
        //         this.index = index;
        //         this.name = name;
        //         this.icon = icon;
        //         this.price = price;
        //     }

        //     public int index;
        //     string desc;
        //     string name;
        //     public string icon;
        //     public int price;
        //     public ItemType itemType;

        // }
        public GImage choice;
        GTextField _name;
        GTextField desc;
        GList baglist;
        GList leftlist;
        GComponent storeup;
        GComponent ui;
        int indexil;
        int indexh;
        public int indexi;
        int choiceindex;
        //int leftindex;
        public int leftright;
        int lieshu;
        private Action<int> OnBuy;

        public Action<int> onoff;
        public Action<int> show;

        public List<BagData> bagDataList = new List<BagData>();
        // Use this for initialization
        void Start()
        {
            // List<ItemData> itemDataList = new List<ItemData>();

            // int index = 0;
            // List<BagData> bagDataList = itemDataList.ToList<ItemData, BagData>((ItemData itemData) => { return new BagData(index++, itemData.name, itemData.icon); });
            // List<BagData> bagDataList = new List<BagData>()
            // {
            //     new BagData(0, "hp", "icon", 100),
            //     new BagData(1, "hp", "icon", 99),                
            // };
        }
        public void Init()
        {
            ui = GetComponent<UIPanel>().ui;
            storeup = ui.GetChild("n5").asCom;
            choice = storeup.GetChild("n12").asImage;
            baglist = storeup.GetChild("rightlist").asList;
            choice.visible = false;
            //leftlist = ui.GetChild("n2").asList;
            baglist.itemRenderer = RenderListItem;
            baglist.onClickItem.Add(onclick);
            //baglist.numItems = 24;
            //close();
            leftright = 0;
            choiceindex = 0;
            lieshuxianding();
            // OnBuy(0);
        }
        //导入当前列表 add by tianjinpeng 2018/03/09 15:16:56
        public void listcount(int index, List<BagData> GListright)
        {
            bagDataList = GListright;
            baglist.numItems = index;
            baglist.EnsureBoundsCorrect();
            lieshuxianding();
            Debug.Log("导入成功" + index);
            showstoredown();
        }
        //显示下栏信息 add by tianjinpeng 2018/03/09 16:35:11
        public void showstoredown()
        {
            show(indexi);
        }
        //当前列表内数量为零时无法进入右列表 add by tianjinpeng 2018/03/09 15:18:38
        public bool panduan()
        {
            if (baglist.numItems == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        //列表选择 add by tianjinpeng 2018/03/09 15:20:44
        public void Up()
        {
            switch (leftright)
            {
                case 0:
                    //upleftlist();
                    break;
                case 1:
                    uprightlist();
                    break;
            }

        }
        public void Down()
        {

            switch (leftright)
            {
                case 0:
                    //downleftlist();
                    break;
                case 1:
                    downrightlist();
                    break;
            }

        }
        public void Right()
        {

            switch (leftright)
            {
                case 0:
                    // leftright = 1;
                    // onoff(1);
                    //shuanindex();
                    break;
                case 1:
                    rightrightlist();
                    break;
            }
        }
        public void Left()
        {

            switch (leftright)
            {
                case 0:
                    break;
                case 1:
                    leftrightlist();
                    break;
            }

        }
        //列数动态调整 add by tianjinpeng 2018/03/09 15:20:08
        void lieshuxianding()
        {
            if (baglist.numItems > 6)
            {
                lieshu = 6;
            }
            else
            {
                lieshu = baglist.numItems;
            }
        }
        //右列表上移 add by tianjinpeng 2018/03/09 15:23:03
        void uprightlist()
        {
            indexh--;
            if (baglist.numItems % lieshu == 0)
            {
                if (indexh < 0)
                {
                    indexh = baglist.numItems / lieshu - 1;
                }
            }
            else
            {
                if (indexh < 0)
                {
                    indexh = baglist.numItems / lieshu;
                    if (indexh == baglist.numItems / lieshu)
                    {
                        if (indexil > (baglist.numItems % lieshu) - 1)
                        {
                            indexil = (baglist.numItems % lieshu) - 1;
                        }
                    }
                }
            }

            shuanindex();

        }
        //右列表下移 add by tianjinpeng 2018/03/09 15:24:07
        void downrightlist()
        {

            indexh++;
            if (baglist.numItems % lieshu == 0)
            {
                if (baglist.numItems / lieshu - 1 < indexh)
                {
                    indexh = 0;
                }
            }
            else
            {
                if (baglist.numItems / lieshu < indexh)
                {
                    indexh = 0;

                }
                else if (indexh == (baglist.numItems / lieshu))
                {
                    if (indexil > (baglist.numItems % lieshu) - 1)
                    {
                        indexil = (baglist.numItems % lieshu) - 1;
                    }
                }
            }

            shuanindex();

        }
        //右列表右移 add by tianjinpeng 2018/03/09 15:26:33
        void rightrightlist()
        {

            indexil++;
            if (indexil > lieshu - 1) { indexil = 0; }
            if (baglist.numItems % lieshu != 0)
            {
                if (indexh == (baglist.numItems / lieshu))
                {
                    if (indexil > (baglist.numItems % lieshu) - 1)
                    {
                        indexil = 0;
                    }
                }
            }

            shuanindex();
        }
        //右列表左移 add by tianjinpeng 2018/03/09 15:27:08
        void leftrightlist()
        {

            indexil--;
            if (indexil < 0)
            {
                indexil = 0;
                leftright = 0;
                choice.visible = false;
                onoff(0);

                //choiceleftpos(choiceindex);
            }
            if (indexh == (baglist.numItems / lieshu))
            {
                if (indexil > (baglist.numItems % lieshu) - 1)
                {
                    indexil = (baglist.numItems % lieshu) - 1;
                }
            }
            if (leftright == 1)
            {
                shuanindex();
            }


        }
        //通过行数列数计算出索引 add by tianjinpeng 2018/03/09 15:29:09
        public void shuanindex()
        {
            indexi = (indexh * lieshu) + indexil;
            Debug.Log("索引" + indexi + "行数" + indexh + "列数" + indexil);
            choicepos(indexi);
            showstoredown();
            //downstroe(indexi);
        }
        //右列表选择框位置调整 add by tianjinpeng 2018/03/09 15:29:54
        void choicepos(int index)
        {
            if (baglist.numItems == 0) { }
            else
            {
                Vector2 screenPos = baglist.GetChildAt(index).LocalToGlobal(Vector2.zero);
                Vector2 sPos = new Vector2(screenPos.x - 2, screenPos.y - 1);
                Vector2 logicScreenPos = storeup.GlobalToLocal(sPos);
                choice.position = new Vector2(logicScreenPos.x, logicScreenPos.y);
            }

        }
        // void choiceleftpos(int index)
        // {
        //     Vector2 screenPos = leftlist.GetChildAt(index).LocalToGlobal(Vector2.zero);
        //     Vector2 sPos = new Vector2(screenPos.x + 8, screenPos.y + 9);
        //     Vector2 logicScreenPos = GRoot.inst.GlobalToLocal(sPos);
        //     choice.position = new Vector2(logicScreenPos.x, logicScreenPos.y);
        // }

        //显示列表回调函数 add by tianjinpeng 2018/03/09 15:30:43
        void RenderListItem(int index, GObject obj)
        {
            GButton button = obj.asButton;
            GLoader sda = button.GetChild("icon").asLoader;
            BagData bagData;
            if (bagDataList.TryGet(index, out bagData))
            {
                sda.url = "Textures/Icon/" + bagData.icon;
                button.text = bagData.price.ToString();
            }

        }
        //选择框位置初始化 add by tianjinpeng 2018/03/09 15:31:20
        public void choice0()
        {
            indexil = 0;
            indexh = 0;
            shuanindex();
        }
        // public void open()
        // {
        //     ui.visible = true;
        // }
        // public void close()
        // {
        //     ui.visible = false;
        // }
        void onclick(EventContext eventContext){
            GObject sdfsad=(GObject)eventContext.data;
            int inda =baglist.GetChildIndex(sdfsad);
            Debug.Log(""+inda);
        }

    }
}

