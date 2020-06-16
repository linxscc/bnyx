using UnityEngine;
using  FairyGUI;
using  DG.Tweening;
using Tang;



public class UIHurtAni : MonoBehaviour
{
    private GameObject CriObj;
    private GameObject OriObj;

    private Camera LookatCam;
    //public GameObject cube;
    public void  Init ()
    {
        FontManager.RegisterFont(FontManager.GetFont("Lithos Pro Regular"),"Lithos Pro Regular");
        CriObj = Resources.Load<GameObject>("UIPrefab/Blood/UICricital");
        OriObj = Resources.Load<GameObject>("UIPrefab/Blood/UIOrdinary");
        LookatCam = GameObject.Find("All").GetComponent<Camera>();
    }


    private static UIHurtAni _instance;

    public static UIHurtAni Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UIHurtAni();
                _instance.Init();
                return _instance;
            }

            return _instance;
        }
    }
    
    public void PlayUIHurt(GameObject go,HurtAniEnum hurtenum,string HurtNum)
    {
        float H = go.transform.GetChild(0).GetComponent<MeshFilter>().mesh.bounds.size.y*go.transform.lossyScale.y;

        switch (hurtenum)
        {
            case HurtAniEnum.Cricital:
                PlayCriAni(go.transform.position,H,HurtNum);
                break;
            case HurtAniEnum.Ordinary:
                PlayOriAni(go.transform.position,H,HurtNum);
                break;
        }
    }

    
       
    private void PlayCriAni(Vector3 Vc,float H,string HurtNum)
    {
        GameObject Cri = Instantiate(CriObj);
        Cri.transform.position = Vc+ new Vector3(-1,H,0);

        #region //让飘字预设物始终看向相机 2019.3.26

        Cri.transform.forward = LookatCam.transform.forward;
        Cri.transform.rotation = LookatCam.transform.rotation;

        #endregion
        GComponent ui = Cri.GetComponent<UIPanel>().ui;
        ui.GetChild("criticalText").text = HurtNum;
        
        Transition t = ui.GetTransition("t0");
        t.Play(() =>
        {
            Destroy(Cri);
        });

        Cri.transform.DOMoveY(
            Cri.transform.position.y+H,0.67f).SetEase(Ease.OutCirc);
       
    }

    private void PlayOriAni(Vector3 Vc,float H,string HurtNum)
    {
        GameObject Ori = Instantiate(OriObj);
        Ori.transform.position = Vc+ new Vector3(0,H,0);
        #region //让飘字预设物始终看向相机 2019.3.26

        Ori.transform.forward = LookatCam.transform.forward;
        Ori.transform.rotation = LookatCam.transform.rotation;

        #endregion
        GComponent ui = Ori.GetComponent<UIPanel>().ui;
        ui.GetChild("ordinaryText").text = HurtNum;
        Transition t = ui.GetTransition("t0");


        Ori.Dobezier(Ori.transform.position,(Ori.transform.position+new Vector3(0,2,0)),
                Ori.transform.position+new Vector3(Random.Range(-2,2) ,Random.Range(0,-H),0),0.67f,0.67f);
        t.Play(() =>
        {
            Destroy(Ori);
        });
    }
    
}
