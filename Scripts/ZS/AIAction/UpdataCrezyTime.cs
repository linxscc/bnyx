using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using  Spine;
using Spine.Unity;
using Action = BehaviorDesigner.Runtime.Tasks.Action;
using Time = Tang.Time;

namespace CrezyTime
{
//    public class UpdataCrezyTime : Action
//    {
//        private float CrezyTime;
//        private object timer;
//        
//        System.Threading.Timer threadTimer;
//
////        private void InitTimer()
////        {
////            threadTimer = new System.Threading.Timer(new TimerCallback(TimerUp), null, 
////                Timeout.Infinite, 1000);
////        }
////        private void btn_Start_Click(object sender, EventArgs e)
////        { //开始计时
////            this.timer.Start();
////        }
////        private void btn_Stop_Click(object sender, EventArgs e)
////        {//停止计时
////            this.timer.Stop();
////        }
////
////        private void ChangeTime(object sender, EventArgs e)
////        {
////            threadTimer.Change(0, 1000);
////        }
//
//
//        public override void OnLateUpdate()
//        {
//            while (CrezyTime > -1)
//            {
//                CrezyTime++;
//            }
//        }
//    }
}


