using UnityEngine;

namespace Tang
{
    class AnimatorStateRecorder : MonoBehaviour
    {
        private int lastStateNameHash = int.MinValue;
        private float lastStateNormalizedTime = 0;
        private AnimatorControllerParameter[] animatorControllerParameters;
        Animator animator;

        bool LazyInit()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
                animatorControllerParameters = animator.parameters;
            }

            if (animator == false)
                return false;

            return true;
        }

        private void OnEnable()
        {
            if (LazyInit())
            {
                if (lastStateNameHash != int.MinValue)
                {
                    SetAnimatorControllerParameters();

                    animator.Play(lastStateNameHash, 0, lastStateNormalizedTime);
                    animator.Update(0);
                }
            }
        }

        void RecordAnimatorControllerParameter()
        {
            foreach (var animatorControllerParameter in animatorControllerParameters)
            {
                switch (animatorControllerParameter.type)
                {
                    case AnimatorControllerParameterType.Float:
                        animatorControllerParameter.defaultFloat = animator.GetFloat(animatorControllerParameter.name);
                        break;
                    case AnimatorControllerParameterType.Int:
                        animatorControllerParameter.defaultInt = animator.GetInteger(animatorControllerParameter.name);
                        break;
                    case AnimatorControllerParameterType.Bool:
                        animatorControllerParameter.defaultBool = animator.GetBool(animatorControllerParameter.name);
                        break;
                }
            }
        }

        void SetAnimatorControllerParameters()
        {
            foreach (var animatorControllerParameter in animatorControllerParameters)
            {
                switch (animatorControllerParameter.type)
                {
                    case AnimatorControllerParameterType.Float:
                        animator.SetFloat(animatorControllerParameter.name, animatorControllerParameter.defaultFloat);
                        break;
                    case AnimatorControllerParameterType.Int:
                        animator.SetInteger(animatorControllerParameter.name, animatorControllerParameter.defaultInt);
                        break;
                    case AnimatorControllerParameterType.Bool:
                        animator.SetBool(animatorControllerParameter.name, animatorControllerParameter.defaultBool);
                        break;
                }
            }
        }

        //private void OnDisable()
        //{
        //    if (LazyInit())
        //    {
        //        lastStateNameHash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        //    }
        //}

        //private void Update()
        //{
        //    if (LazyInit())
        //    {
        //        lastStateNameHash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        //        //Debug.Log("animator.GetCurrentAnimatorStateInfo(0).fullPathHash = " + animator.GetCurrentAnimatorStateInfo(0).fullPathHash);
        //    }
        //}

        private void LateUpdate()
        {
            lastStateNameHash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
            lastStateNormalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            RecordAnimatorControllerParameter();
        }
    }
}
