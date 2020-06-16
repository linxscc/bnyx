using System.Collections;
using System.Collections.Generic;
using Tang;
using UnityEngine;

public class ShakeController : PlacementController
{
   public float JudgeMoveSpeed = 9;
   public override void OnTriggerIn(TriggerEvent evt)
   {
      JudgeSpeed(evt);
   }
   public override void OnTriggerKeep(TriggerEvent evt)
   {
   
   }
   public override void OnTriggerOut(TriggerEvent evt)
   {
      MainAnimator.SetInteger("state",0);
   }

   public void JudgeSpeed(TriggerEvent evt)
   {
      if (MainAnimator&&evt.otherTriggerController.transform.parent.tag == "Player")
      {
         float NowSpeed = Mathf.Abs(evt.otherTriggerController.transform.parent.GetComponent<HumanController>().Speed.x);
         bool CompareDir = evt.colider.bounds.center.x < evt.colidePoint.x;
         bool CompareSize = NowSpeed > JudgeMoveSpeed;
         if (CompareDir)
         {
            if (CompareSize)
            {
               MainAnimator.SetInteger("state",4);
            }
            else
            {
               MainAnimator.SetInteger("state",3);
            }
         }
         else
         {
            if (CompareSize)
            {
               MainAnimator.SetInteger("state",2);
            }
            else
            {
               MainAnimator.SetInteger("state",1);
            }
         }
      }
   }

   
}
