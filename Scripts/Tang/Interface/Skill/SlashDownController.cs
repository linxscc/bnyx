using UnityEngine;
namespace Tang
{
	public class SlashDownController : FlySkillController 
	{
		public override void OnHit()
		{
			MainDamageController.gameObject.SetActive(false);
			Speed = Vector3.zero;
			Destroy(this.gameObject);
		}

	}
}

