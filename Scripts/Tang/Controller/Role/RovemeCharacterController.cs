using System;
using UnityEngine;

namespace Tang
{
    public class RemoveCharacterController : MonoBehaviour
    {
        private void Start()
        {
            GameObject.Find(name).GetComponent<CharacterController>().enabled = false;
            GameObject.Find(name).GetComponent<RemoveCharacterController>().enabled = false;
        }
    }
}