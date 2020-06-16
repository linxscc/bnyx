using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Tang;
using UnityEngine;

public class RoleHPController : MonoBehaviour
{
    ValueMonitorPool valueMonitorPool = new ValueMonitorPool();
    private RoleController roleController;

    private void Start()
    {
        roleController = GetComponent<RoleController>();
        Init();
    }
    
    public void Init()
    {
        valueMonitorPool.AddMonitor(()=>roleController.SceneId == SceneManager.Instance.CurrSceneId, (f, t) =>
        {
            if (t)
            {
                MessageManager.Instance.Dispatch(MessageName.SHOW_BOSS_HP_UI);
            }
            else
            {
                MessageManager.Instance.Dispatch(MessageName.HIDE_BOSS_HP_UI);
            }
        },true);

        valueMonitorPool.AddMonitor(()=>roleController._roleData.Hp, (f, t) =>
        {
            MessageManager.Instance.Dispatch(MessageName.SET_PLAYER1_BOSS_HP,new object []{roleController.Hp,roleController.HpMax});
        },true);

        valueMonitorPool.AddMonitor(()=>roleController._roleData.HpMax, (f, t) =>
        {
            MessageManager.Instance.Dispatch(MessageName.SET_PLAYER1_BOSS_HP,new object []{roleController.Hp,roleController.HpMax});
        },true);
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

}
