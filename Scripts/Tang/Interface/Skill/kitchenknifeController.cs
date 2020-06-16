using UnityEngine;

namespace Tang
{
	public class kitchenknifeController : FlySkillController 
	{
		public override void OnFinished()
		{
			// Speed = Vector3.zero;
			Destroy(gameObject);
		}
		public override void Startup()
		{
			State = 1;

            DelayFunc("Destory", () =>
            {
				Speed = Vector3.zero;
                State = 2;
            }, 0.35f);
		}
	}
}

