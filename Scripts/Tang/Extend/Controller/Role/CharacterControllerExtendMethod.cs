using UnityEngine;


namespace Tang
{
    public static class CharacterControllerExtendMethod
    {
        public static void MoveTo(this CharacterController target, Vector3 pos)
        {
            Vector3 offset = pos - target.gameObject.transform.position;
            target.Move(offset);
        }
    }
}