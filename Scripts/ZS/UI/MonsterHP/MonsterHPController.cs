using System;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using Tang;
using UnityEngine;

public class MonsterHPController : MonoBehaviour
{
   
    
    private float Height;
    private Camera camera;
    private GameObject HPObj;
    void Start()
    {
        Init();
    }

    private void Init()
    {
        Get();
        LookAtCamera(HPObj);
        SetHPPos(HPObj);
    }
    
    private void Get()
    {
        Height = transform.GetChild(0).GetComponent<MeshFilter>().mesh.bounds.size.y*transform.lossyScale.y;
        camera = GameObject.Find("All").GetComponent<Camera>();
        GameObject HP = Resources.Load<GameObject>("UIPrefab/FGUI/MonsterHP");
        HPObj = Instantiate(HP);
    }

   
    void LookAtCamera(GameObject go)
    {
        go.transform.forward = camera.transform.forward;
        go.transform.rotation = camera.transform.rotation;
    }

    void SetHPPos(GameObject go)
    {
        go.transform.parent = transform;
        float width = go.GetComponent<UIPanel>().ui.width/100;
        float height = go.GetComponent<UIPanel>().ui.height/100;
        go.transform.localPosition = new Vector3(0, Height, 0)+ new Vector3(-width,height,0);
    }
   }
