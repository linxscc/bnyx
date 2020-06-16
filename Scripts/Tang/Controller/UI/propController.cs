
using UnityEngine;
using FairyGUI;
namespace Tang
{
    public class propController : MonoBehaviour
    {
        GComponent _mainView;
        GList _list;
        ScrollPane scrollPane;
        
        RoleController player1Controller;

        // Use this for initialization
        void Start()
        {
            _mainView = this.GetComponent<UIPanel>().ui;
            //player1Controller = GameObject.Find("Player1").GetComponent<RoleController>();

            _list = _mainView.GetChild("list").asList;
            _list.SetVirtualAndLoop();

            _list.itemRenderer = RenderListItem;
            _list.numItems =6;
            _list.scrollPane.onScroll.Add(DoSpecialEffect);

            DoSpecialEffect();
            scrollPane=_list.scrollPane;
            scrollPane.scrollStep=93f;


        }

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
        void RenderListItem(int index, GObject obj)
        {
            GButton item = (GButton)obj;
            item.SetPivot(0.5f, 0.5f);
            
            GLoader sda=item.GetChild("icon").asLoader;
            sda.url="Textures/Icon/soul1";
            //item.icon = "ui://UI/key";
        }
        void Update(){
            if(Input.GetKeyDown(KeyCode.Q)){
                scrollPane.ScrollRight(-1,true);

            }

            if(Input.GetKeyDown (KeyCode.E)){
                scrollPane.ScrollRight(1,true);
            }
        }

    }

}
