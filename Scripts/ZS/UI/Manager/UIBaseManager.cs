using System.Collections.Generic;
using UnityEngine;

public class UIBaseManager : MonoBehaviour
{
  
    protected Dictionary<string, UIInterface> uis = new Dictionary<string, UIInterface>();    

    protected Dictionary<string, GameObject> objs = new Dictionary<string, GameObject>();
    protected Dictionary<string, GameObject> insObjs = new Dictionary<string, GameObject>();
 
    public virtual void CreatFGUIObj(string Name)
    {   }
    public virtual void CreatSUIObj(string Name)
    {   }
    public virtual void DesUI(string Name)
    {   }
    public virtual void ShowUI(string Name) { }
    public virtual void HideUI(string Name) { }
    public virtual void DesIm(string Name) { }



    
}
