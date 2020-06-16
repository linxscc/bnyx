using UnityEngine;
using Tang.Anim;

namespace Tang
{
    public class BloodEffectController : AnimEffectvirtualClass
    {
        private GameObjectData gameObjectData = new GameObjectData();
        private SpriteRenderer spriteRenderer;
        private Rigidbody mainRigidbody;

        public System.Action OnCollisionEnterAction;

        bool isCollisided = false;

        Renderer mainRenderer;
        GameObject rendererGameObject;

        public void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            mainRigidbody = GetComponent<Rigidbody>();

            var a = mainRigidbody.mass;

        }

        public void OnEnable()
        {
            rendererGameObject = gameObject.GetChild("Renderer");
            isCollisided = false;

            // rigidbody.
        }

        void OnSpawned()
        {
            //gameObject.DOFade(1, 0);
            //transform.localScale = new Vector3(1, 1, 1) * 0.1f;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (isCollisided)
                return;
            isCollisided = true;



            transform.eulerAngles = new Vector3(90, 0, 0);

            if (OnCollisionEnterAction != null)
                OnCollisionEnterAction();
        }

        public void SetAlpha(float alpha)
        {
            rendererGameObject.RecursiveComponent<Renderer>((Renderer r, int depth) =>
            {
                Color oldColor = r.material.GetColor("_Color");
                oldColor.a = 1;
                r.material.SetColor("_Color", oldColor);

            }, 2, 999);
        }
    }
}