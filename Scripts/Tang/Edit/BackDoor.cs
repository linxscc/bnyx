

using UnityEngine;
#if UNITY_EDITOR

#endif


namespace Tang
{
    public class BackDoor : BackWall
    {
        public override string PathInScene
        {
            get { return "Portals"; }
        }

        void Awake()
        {
            if (gameObject.GetComponent<PortalController>() == null)
            {
                gameObject.AddComponent<PortalController>();
            }
        }

        void Start()
        {
            gameObject.GetComponent<Collider>().enabled = false;
        }
    }
}