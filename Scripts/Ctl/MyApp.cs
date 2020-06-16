using UnityEngine;

public class MyApp : MonoBehaviour {
    // Use this for initialization

    void Awake()
    {
        GameCtl.Instance.init(this.gameObject);
    }

	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
