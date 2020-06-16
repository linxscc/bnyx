using UnityEngine;

namespace Tang
{
    public class BloodParticleController : MonoBehaviour
    {
        public ParticleSystem mainParticleSystem;
        ParticleSystem.Particle[] arrPar;

        public int Action_OnParticleCollision_CallTimes = 0;
        public System.Action Action_OnParticleCollision;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            mainParticleSystem = GetComponent<ParticleSystem>();
            arrPar = new ParticleSystem.Particle[mainParticleSystem.main.maxParticles];
        }

        /// <summary>
        /// OnParticleCollision is called when a particle hits a collider.
        /// </summary>
        /// <param name="other">The GameObject hit by the particle.</param>
        void OnParticleCollision(GameObject other)
        {
            if (Action_OnParticleCollision != null)
            {
                Action_OnParticleCollision_CallTimes++;
                Action_OnParticleCollision();
            }
            // // int arrParCount = particleSystem.GetParticles(arrPar);
            // // for (int i = 0; i < arrParCount; i++)
            // // {
            // //     var par = arrPar[i];
            // //     par.rotation3D = new Vector3(180, 0, 0);
            // // }
            // AddBloodPiece(other);
        }

        // int leftCount = 1;
        // void AddBloodPiece(GameObject other)
        // {
        //     // if (leftCount > 0)
        //     // {
        //     //     leftCount--;

        //     //     var bloodPiece = GameObjectManager.Instance.Create("BloodPiece1");
        //     //     bloodPiece. other
        //     // }
        // }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            if (mainParticleSystem.isStopped)
                Tools.Destroy(gameObject);
        }
    }
}