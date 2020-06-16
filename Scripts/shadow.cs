using UnityEngine;

public class shadow : MonoBehaviour {

    CharacterController controller;

	// Use this for initialization
	void Start () {
        controller = this.GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 velocity = Vector3.zero;
        
        GameObject ob = null;
        Transform t = transform.root.Find("role");
        if (t != null)
        {
            ob = t.gameObject;
            transform.position = ob.transform.position;
        }
        velocity.y = -1000;
        controller.Move(velocity * Time.deltaTime);
	}
}
