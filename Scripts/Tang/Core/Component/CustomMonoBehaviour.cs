using System;


namespace Tang
{
    public class CustomMonoBehaviour : MyMonoBehaviour
    {
        public Action OnActionAwake { get; set; }
        public Action OnActionOnEnable { get; set; }
        public Action OnActionOnDisable { get; set; }
        public Action OnActionStart { get; set; }
        public Action OnActionSpawned { get; set; }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            if (OnActionAwake != null)
            {
                OnActionAwake();
            }
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable()
        {
            if (OnActionOnEnable != null)
            {
                OnActionOnEnable();
            }
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        void OnDisable()
        {
            if (OnActionOnDisable != null)
            {
                OnActionOnDisable();
            }
        }

        void Start()
        {
            if (OnActionStart != null)
            {
                OnActionStart();
            }
        }

        void OnSpawned()
        {
            if (OnActionSpawned != null)
            {
                OnActionSpawned();
            }
        }
    }
}