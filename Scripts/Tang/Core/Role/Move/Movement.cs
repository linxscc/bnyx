using UnityEngine;

namespace Tang
{
    public class Movement
    {
        public MovementType type;
        public Transform transform;
        protected Vector3 speed;

        public void move(Vector3 moveVec3)
        {
            transform.transform.localPosition += moveVec3;
        }

        public void update(float deltaTime)
        {
            move(speed * deltaTime);
        }
    }
}