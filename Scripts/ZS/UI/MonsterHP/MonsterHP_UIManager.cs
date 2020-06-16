using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FairyGUI;
using Tang;
using UnityEngine;

public class MonsterHP_UIManager : MonoBehaviour
{
    
    ValueMonitorPool valueMonitorPool = new ValueMonitorPool();
    private RoleController roleController;
    
    private GProgressBar HPBar;
    private GTextField HPText;
    private GComponent ui;
    private GComponent UI => ui ?? (ui = GetComponent<UIPanel>().ui);

    private Tweener HPTweener;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    
    private void Init()
    {
        HPBar = UI.asProgress;
        HPText = UI.GetChild("MonsterHPText").asTextField;
        roleController = transform.parent.transform.GetComponent<RoleController>();
       valueMonitorPool.AddMonitor(() => roleController._roleData.Hp, (f, f1) =>
       {
           SetHP(roleController._roleData.Hp);
       },true); 
    }
    
    private void SetHP(float hpend)
    {
        HPBar.max = roleController._roleData.HpMax;
        HPTweener?.Kill();
        HPBar.DoValue(hpend, ((float) HPBar.value - hpend) / roleController._roleData.HpMax);
        HPTweener.OnComplete(() => { HPTweener = null;});

        HPText.text = hpend.ToString();
    }

    private void Update()
    {
       valueMonitorPool.Update();
    }
}
