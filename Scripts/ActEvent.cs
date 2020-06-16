using UnityEngine;

public class ActEvent : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AtkEvt()
    {
        Util.Log("ctrol---AtkEvt:" + gameObject.name);
    }
}
