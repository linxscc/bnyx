using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Tang;
using UnityEngine;
using Event = Tang.Event;
using Object = System.Object;

public class RoleHPForOther : MonoBehaviour, ITriggerDelegate
{
    ValueMonitorPool valueMonitorPool = new ValueMonitorPool();
    private Object hitObject;

    private void Start()
    {
        hitObject = GetComponent<ITriggerDelegate>();
        Init();
    }
    
    public void Init()
    {
//        valueMonitorPool.AddMonitor(()=>hitObject.ToString()== SceneManager.Instance.CurrSceneId, (f, t) => { }
    }

    private void OnDisable()
    {
        UIManager.Instance.DoDelay(0.00001f).OnComplete(() =>
        {
            valueMonitorPool.Update(); 
        });
    }

    private void Update()
    {
        valueMonitorPool.Update();
    }

    public bool OnEvent(Event evt)
    {
        throw new NotImplementedException();
    }

    public GameObject GetGameObject()
    {
        GameObject gameObject = GetComponent<ITriggerDelegate>().gameObject;
        return gameObject;
    } 

    public void OnTriggerIn(TriggerEvent evt)
    {
        throw new NotImplementedException();
    }

    public void OnTriggerOut(TriggerEvent evt)
    {
        throw new NotImplementedException();
    }

    public void OnTriggerKeep(TriggerEvent evt)
    {
        throw new NotImplementedException();
    }
}
