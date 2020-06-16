using UnityEngine;
using Spine.Unity;
namespace Tang
{
	public enum SpeedType
	{
		Fixed = 0,//固定动画速度
		Fixedanimatorspeed = 1,//固定第一次animator的动画速度
		animatorspeed = 2//速度跟随animator的动画速度
	}
	public class FrameEventPlayAnimSpeedController : MonoBehaviour 
	{
		ValueMonitorPool valueMonitorPool=new ValueMonitorPool();
		public Animator animator;
		public float speed=0f;
		public SpeedType speedType=SpeedType.Fixedanimatorspeed;
		public SkeletonAnimation skeletonAnimation;
		// Use this for initialization
		void Start () 
		{
			valueMonitorPool.Clear();
			skeletonAnimation=GetComponent<SkeletonAnimation>();
			Spine.TrackEntry trackEntry=skeletonAnimation.state.GetCurrent(0);
			switch (speedType)
			{
				case SpeedType.Fixedanimatorspeed :
					trackEntry.TimeScale=animator.speed;
				break;
				case SpeedType.Fixed :
					trackEntry.TimeScale=speed;
				break;
				case SpeedType.animatorspeed :
					trackEntry.TimeScale=animator.speed;
					valueMonitorPool.AddMonitor(()=>
					{
						return animator.speed;
					},(float from ,float to)=>
					{
						trackEntry.TimeScale=to;
					});
				break;
				default:
				break;
			}
            valueMonitorPool.AddMonitor(()=> 
            {
                return trackEntry.TrackTime ;
            },(float from,float to)=> 
            {
                float dsda = trackEntry.AnimationEnd;
                if (to >= dsda)
                {
                    Tools.Destroy(gameObject);
                }
            });
		}
		
		// Update is called once per frame
		void Update () 
		{
			valueMonitorPool.Update();
		}
	}
}

