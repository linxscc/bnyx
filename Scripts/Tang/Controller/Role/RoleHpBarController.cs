using UnityEngine;
using FairyGUI;

namespace Tang
{
    public class RoleHpBarController : MonoBehaviour
    {
        public GameObject parent;
        GComponent console;
        RoleController roleController;
        ValueMonitorPool valueMonitorPool = new ValueMonitorPool();
        // Use this for initialization
        void Start()
        {
            xuetiao();
        }

        void xuetiao()
        {

            valueMonitorPool.Clear();
            console = this.GetComponent<UIPanel>().ui;
            //roleController =parent.GetComponentInParent<RoleController>();
            roleController = parent.GetComponent<RoleController>();

            GProgressBar progressBar_hp = console.asProgress;//GetChild("ProgressBar_HP").asProgress;



            {
                valueMonitorPool.AddMonitor((System.Func<float>)(() =>
                           {
                               return (float)roleController.RoleData.FinalHpMax;
                           }), (System.Action<float, float>)((float from, float to) =>
                           {
                               progressBar_hp.max = roleController.RoleData.FinalHpMax;
                           }));

                valueMonitorPool.AddMonitor((System.Func<float>)(() =>
                           {
                               return (float)roleController.RoleData.Hp;
                           }), (System.Action<float, float>)((float from, float to) =>
                           {
                               progressBar_hp.value = roleController.RoleData.Hp;
                           }));
            }
        }





        // Update is called once per frame
        void Update()
        {

        }
        void FixedUpdate()
        {
            valueMonitorPool.Update();
        }
    }
}
