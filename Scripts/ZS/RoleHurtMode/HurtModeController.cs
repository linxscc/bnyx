using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Spine.Unity;
using Tang;
using UnityEngine;
using ZS;

[Serializable]
public class HurtModeController : MonoBehaviour
{
    public List<HurtMode> HurtModes = new List<HurtMode>();
    
    private Dictionary<string, HurtMode> HurtModeDic = new Dictionary<string, HurtMode>();
    private Dictionary<string,HurtPart> HurtPartDic = new Dictionary<string, HurtPart>();
    
    private Dictionary<string, GameObject> HurtModeObjectDic = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> HurtPartObjectDic = new Dictionary<string, GameObject>(); 
    
    private string CurrentHurtModeName;    
    private SkeletonRenderer skeletonRenderer;

    private void Start()
    {
        skeletonRenderer = GetComponentInChildren<SkeletonRenderer>();
        InitHurtModeDic();
        InitHurtModeObj();
        InitHurtPartDic();
        InitHurtPartObjDic();
    }


    private void InitHurtModeDic()
    {
        HurtModeDic = HurtModes.ToDictionary(i => i.Name, i => i);
        
        GameObject obj = gameObject.GetChild("HurtModesObject");

        HurtModeObjectDic = HurtModes.ToDictionary(mode => mode.Name, mode => obj.GetChild(mode.Name));

    }
    
    private void InitHurtModeObj()
    {
        
        if (HurtModes.Count != 0)
        {
            foreach (var item in HurtModeObjectDic)
            {
                if (item.Key == HurtModes[0].Name)
                {
                    CurrentHurtModeName = item.Key;
                    item.Value.SetActive(true);
                }

                item.Value.SetActive(false);
            }

            HurtModeObjectDic[HurtModes[0].Name].SetActive(true);
        }

        
    }

    private void InitHurtPartDic()
    {
        if (HurtModes.Count == 0) return;
        for (int i = 0; i < HurtModes.Count; i++)
        {
            for (int j = 0; j < HurtModes[i].HurtPartList.Count; j++)
            {
                HurtPartDic.Add(HurtModes[i].Name+"_"+HurtModes[i].HurtPartList[j].Name,HurtModes[i].HurtPartList[j]);
            }
        }
    }
    
    private void InitHurtPartObjDic()
    {
        foreach (var hurtMode in HurtModes)
        {
            GameObject hurtModeObject = GetModeObject(hurtMode.Name);

            foreach (var hurtPart in hurtMode.HurtPartList)
            {
                GameObject hurtPartObject = hurtModeObject.GetChild(hurtPart.Name);
                HurtPartObjectDic.Add(hurtMode.Name+"_"+ hurtPart.Name, hurtPartObject);
            }
        }
    }

    public GameObject GetPartObject(string name)
    {
        GameObject hurtHurtObject;
        if (HurtPartObjectDic.TryGetValue(CurrentHurtModeName+"_"+ name, out hurtHurtObject))
        {
            return hurtHurtObject;
        }
        return null;
    }


    public GameObject GetModeObject(string name)
    {
        GameObject hurtModeObject;
        if (HurtModeObjectDic.TryGetValue(name, out hurtModeObject))
        {
            return hurtModeObject;
        }
        return null;
    }

    public HurtMode GetHurtMode(string name)
    {
        HurtMode hurtMode;
        if (HurtModeDic.TryGetValue(name, out hurtMode))
        {
            return hurtMode;
        }

        return null;
    }
    
    public HurtPart GetHurtPart(string  name)
    {
        string HurtPartName = CurrentHurtModeName + "_" + name;

        HurtPart hurtPart;
        if (HurtPartDic.TryGetValue(HurtPartName,out hurtPart))
        {
            return hurtPart;
        }
        return null;
    }

    
    public void SetHurtMode(string name)
    {
        HurtModeObjectDic[CurrentHurtModeName].SetActive(false);
        HurtModeObjectDic[name].SetActive(true);
        CurrentHurtModeName = name;
    }

    private void LateUpdate()
    {
        if (string.IsNullOrWhiteSpace(CurrentHurtModeName)) return;
        
        HurtMode hurtMode = GetHurtMode(CurrentHurtModeName);

        foreach (var hurtPart in hurtMode.HurtPartList)
        {
            if (hurtPart.IsFollowSlot)
            {
                GameObject hurtPartObject = GetPartObject(hurtPart.Name);
                
                Vector3 worldPosition;
                Vector3 worldScale;
                Quaternion worldRotation;
                if (skeletonRenderer.TryGetSlotAttachmentCube(hurtPart.FollowSlotName, out worldPosition, out worldScale, out
                    worldRotation))
                {
                    hurtPartObject.SetActive(true);
                    
                    hurtPartObject.transform.position = worldPosition;
                    hurtPartObject.transform.localScale = worldScale;
                    hurtPartObject.transform.localRotation = worldRotation;
                }
                else
                {
                    hurtPartObject.SetActive(false);
                    
                    Debug.Log("找不到slot:" + hurtPart.FollowSlotName);
                }
            }
        }
    }
}
