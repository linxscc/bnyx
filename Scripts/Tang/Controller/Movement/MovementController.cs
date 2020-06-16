using System;
using UnityEngine;




namespace Tang
{
    public class MovementController
    {
        Vector3 position = Vector3.zero;
        public Vector3 Position { get { return position; } set { position = value; } }
        Vector3 speed = Vector3.zero;
        public Vector3 Speed { get { return speed; } set { speed = value; } }
        Vector3 acceleratedSpeed = Vector3.zero;
        public Vector3 AcceleratedSpeed { get { return acceleratedSpeed; } set { acceleratedSpeed = value; } }




        public Action<Vector3> positionSetter;

        public void Init(Vector3 position, Action<Vector3> positionSetter)
        {
            this.position = position;
            this.positionSetter = positionSetter;
        }

        public void Update()
        {
            speed += acceleratedSpeed * Time.deltaTime;
            position += speed * Time.deltaTime;
            if (positionSetter != null)
                positionSetter(position);
        }
    }
}