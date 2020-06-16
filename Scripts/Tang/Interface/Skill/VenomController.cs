namespace Tang
{
    public class VenomController : StaticSkillController
    {
        public override void Awake()
        {
            //base.Awake();
            //var player1 = GameObject.Find("Player1");
            //InitPos(new Vector3(player1.transform.position.x, 0, player1.transform.position.z));
        }
        private void Start()
        {
            base.Awake();
            //var player1 = owner.GetComponent<RoleAIController>().TargetController;
            //InitPos(new Vector3(player1.transform.position.x, 0, player1.transform.position.z));
        }

    }
}

