using UnityEngine;

namespace Tang
{
    public class RoleSetTouch : RoleBaseStateMachineBehaviour
    {
        public float animspeed = 1f;
        public float Angle = 45f;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            lazyInit(animator);
            
            float angle = animator.GetFloat("MagicAngle");
            float angle_z;


//            float x = new Random().Next(-20, 21);
            System.Random y_y = new System.Random();
            int y = y_y.Next(-2100, 2100);
            System.Random z_z = new System.Random();
            int z = z_z.Next(-2100, 2100);
            angle =(float) (y*0.01);
            angle_z = (float) (z*0.01);

            Quaternion quaternion = Quaternion.Euler(0, angle, z:angle_z);
            
            var localPosition = quaternion * Vector3.right;
            localPosition.z *= -1;
            localPosition.x *= RoleController.GetDirectionInt();
            RoleController.SetLocalPointPosition(localPosition);
        }
//       
//        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//        {
//            base.OnStateUpdate(animator, stateInfo, layerIndex);
//        }
//        
//        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//        {
//            base.OnStateExit(animator, stateInfo, layerIndex);
//        }
    }
}
