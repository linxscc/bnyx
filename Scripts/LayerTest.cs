using UnityEngine;

public class LayerTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Util.Log("Start:" + gameObject.name);
    }
	
	// Update is called once per frame
	void Update () {
        Util.Log("Update:" + gameObject.name);
    }

    void Awake()
    {
        Util.Log("Awake:" + gameObject.name);
    }

    public void ev1()
    {
        Util.Log("ev1:"+ gameObject.name);
    }
    //public void ev2(string s, int i, float f)
    public void ev2(string s)
    {
        //Util.Log("ev2:"+ gameObject.name+"  s="+s+"i="+i+" f="+f);
        Util.Log("ev2:" + gameObject.name + "  s=" + s);
    }
    public void ev3()
    {
        Util.Log("ev3:"+ gameObject.name);
    }
    public void ev4()
    {
        Util.Log("ev4:"+ gameObject.name);
    }
}
