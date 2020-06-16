using UnityEngine;
using DG.Tweening;
using System;
using System.IO;

namespace ZS
{
  public class shotMoveAni 
  {

      public static shotMoveAni _Instance;

      public static shotMoveAni Instance
      {
          get
          {
              if (_Instance == null)
              {
                  _Instance = new shotMoveAni();
                  _Instance.LoadAsset();
                  return _Instance;
              }

              return _Instance;
          }
      }
      

   
    public  void MoveAni(GameObject go,string Name,Action CallBack = null){

        for (int i = 0; i < shotDatas.shotAniDatas.Count ; i++)
        {
            if (shotDatas.shotAniDatas[i].Name == Name)
            {
                go.transform.position = shotDatas.shotAniDatas[i].StartPos;
                shotMoveAnimation(go,shotDatas.shotAniDatas[i].StartPos
                ,shotDatas.shotAniDatas[i].EndPos,
                shotDatas.shotAniDatas[i].ExecutionTime,
                shotDatas.shotAniDatas[i].ease);
                break;
            }
        }
       
    }

    public void shotMoveAnimation(GameObject go,Vector3 Startvec,Vector3 Endvec,float timer,Ease  ease =Ease.Linear,Action CallBack = null){
        
        Tween tween = DOTween.To(()=>go.transform.position,x => go.transform.position = x,
        Endvec,timer).SetEase(ease);
        
        tween.OnComplete(()=> {  
            if (CallBack !=null)  CallBack();   
            Tween t = DOTween.To(()=>go.transform.position,x => go.transform.position = x,
        Startvec,1);                                                           
        }       
        );
            
    }

    private string path = Application.dataPath + "/Resources/Manager/ZS/ShotAni/ShotAniDataManager.asset";
    private string AssetPath = "Assets/Resources_moved/Manager/ZS/ShotAni/ShotAniDataManager.asset";
    private ShotDatas shotDatas;
    private void LoadAsset()
    {
        if (File.Exists(path))
        {
            shotDatas =  Tang.AssetManager.LoadAssetAtPath<ShotDatas>(AssetPath);
            
        }
        else
        {
            Directory.CreateDirectory(path);
            shotDatas = new ShotDatas();             
        }

    }


}
  
}
