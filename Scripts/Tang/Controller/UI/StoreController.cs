using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;

namespace Tang
{
    public class StoreController : MonoBehaviour
    {
		public int leftright;
		public int file;
		public int row;
		int fileMax;
		public int bagindex;
		public int storebuyindex;
		public GImage choice;
		GList baglist;
		GComponent ui;
		GComponent storeup;
		public List<BagData> bagDataList = new List<BagData>();
		public Action<int> onoff;
        public Action<int> show;
	
		public void Init()
		{
			ui = GetComponent<UIPanel>().ui;
			storeup = ui.GetChild("n5").asCom;
			baglist = storeup.GetChild("rightlist").asList;
			choice = storeup.GetChild("n12").asImage;

			baglist.itemRenderer = RenderListItem;
			baglist.numItems = 10;
            baglist.onClickItem.Add(onclick);
			lieshuxianding();
			close();
            choice.visible=false;
		}
		public void up()
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
		public void down()
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
		public void left()
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
		public void right()
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
		public void Enter()
		{
		}
		public void open()
		{
			ui.visible=true;
		}
		public void close()
		{
			ui.visible=false;
		}
		public void closechoice()
		{
			choice.visible=false;
		}
		public void openchoice()
		{
			choice.visible=true;
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
		//列数动态调整 add by tianjinpeng 2018/03/09 15:20:08
		void lieshuxianding()
        {
            if (baglist.numItems > 6)
            {
                fileMax = 6;
            }
            else
            {
                fileMax = baglist.numItems;
            }
        }
        //根据索引算行数列数 add by tianjinpeng 2018/03/14 10:36:12
		public void indexshuan(int index)
		{
			row=index/6;
			file=index%6;
			shuanindex();
		}
		public void shuanindex()
        {
            bagindex = (row * fileMax) + file;
            Debug.Log("索引" + bagindex + "行数" + row + "列数" + file);
            choicepos(bagindex);
            showstoredown();
            //downstroe(indexi);
        }
		void showstoredown()
        {
            show(bagindex);
        }
        GTweener choicemove;
        //右列表选择框位置调整 add by tianjinpeng 2018/03/09 15:29:54
		void choicepos(int index)
        {
            if (baglist.numItems == 0) { }
            else
            {
                Vector2 screenPos = baglist.GetChildAt(index).LocalToGlobal(Vector2.zero);
                Vector2 sPos = new Vector2(screenPos.x - 2, screenPos.y - 1);
                Vector2 logicScreenPos = storeup.GlobalToLocal(sPos);
                //choice.position = new Vector2(logicScreenPos.x, logicScreenPos.y);
                killtweener(choicemove);
                choicemove=choice.TweenMove(logicScreenPos,0.1f);
            }
        }
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
        //鼠标事件 add by tianjinpeng 2018/03/09 15:30:43
		void onclick(EventContext eventContext)
		{
			GObject sdfsad=(GObject)eventContext.data;
            int inda =baglist.GetChildIndex(sdfsad);
            leftright=1;
            indexshuan(inda);
            onoff(1);
            
			//indexshuan(inda);
			//openchoice();
            
            Debug.Log(""+inda);
		}
        //选择框位置初始化 add by tianjinpeng 2018/03/09 15:31:20
		public void choice0()
        {
            file=0;
            row=0;
            shuanindex();
		}
        //右列表是否空 add by tianjinpeng 2018/03/14 10:35:23
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
		//右列表上移 add by tianjinpeng 2018/03/09 15:23:03
        void uprightlist()
        {
            row--;
            if (baglist.numItems % fileMax == 0)
            {
                if (row < 0)
                {
                    row = baglist.numItems / fileMax - 1;
                }
            }
            else
            {
                if (row < 0)
                {
                    row = baglist.numItems / fileMax;
                    if (row == baglist.numItems / fileMax)
                    {
                        if (file > (baglist.numItems % fileMax) - 1)
                        {
                            file = (baglist.numItems % fileMax) - 1;
                        }
                    }
                }
            }

            shuanindex();

        }
        //右列表下移 add by tianjinpeng 2018/03/09 15:24:07
        void downrightlist()
        {

            row++;
            if (baglist.numItems % fileMax == 0)
            {
                if (baglist.numItems / fileMax - 1 < row)
                {
                    row = 0;
                }
            }
            else
            {
                if (baglist.numItems / fileMax < row)
                {
                    row = 0;

                }
                else if (row == (baglist.numItems / fileMax))
                {
                    if (file > (baglist.numItems % fileMax) - 1)
                    {
                        file = (baglist.numItems % fileMax) - 1;
                    }
                }
            }

            shuanindex();

        }
        //右列表右移 add by tianjinpeng 2018/03/09 15:26:33
        void rightrightlist()
        {

            file++;
            if (file > fileMax - 1) { file = 0; }
            if (baglist.numItems % fileMax != 0)
            {
                if (row == (baglist.numItems / fileMax))
                {
                    if (file > (baglist.numItems % fileMax) - 1)
                    {
                        file = 0;
                    }
                }
            }

            shuanindex();
        }
        //右列表左移 add by tianjinpeng 2018/03/09 15:27:08
        void leftrightlist()
        {

            file--;
            if (file < 0)
            {
                file = 0;
                leftright = 0;
                closechoice();
                onoff(0);

                //choiceleftpos(choiceindex);
            }
            if (row == (baglist.numItems / fileMax))
            {
                if (file > (baglist.numItems % fileMax) - 1)
                {
                    file = (baglist.numItems % fileMax) - 1;
                }
            }
            if (leftright == 1)
            {
                shuanindex();
            }


        }
        //返回choice位置 add by tianjinpeng 2018/03/14 10:30:58
        public Vector2 choicevector2()
        {
            return choice.position;
        }
        //返回当前choice前往的位置 add by tianjinpeng 2018/03/14 10:32:28
        public Vector2 choicenow(){
            Vector2 screenPos = baglist.GetChildAt(bagindex).LocalToGlobal(Vector2.zero);
            Vector2 sPos = new Vector2(screenPos.x - 2, screenPos.y - 1);
            Vector2 logicScreenPos = storeup.GlobalToLocal(sPos);
            return logicScreenPos;
        }
        //打断动画进程 add by tianjinpeng 2018/03/14 10:33:03
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
        
    }
}

