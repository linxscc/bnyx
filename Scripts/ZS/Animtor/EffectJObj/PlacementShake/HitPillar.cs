using System.Collections;
using System.Collections.Generic;
using Tang;
using UnityEngine;

public class HitPillar : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (string.Equals ("Cube", collider.gameObject.name))
        {
            print("击中Pillar");
        }
    }
}
