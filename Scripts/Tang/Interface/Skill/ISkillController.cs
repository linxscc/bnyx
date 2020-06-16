using System.Collections.Generic;
using UnityEngine;

namespace Tang
{
    public interface ISkillController
    {
        GameObject Owner { get; set; }
        string TeamId { get; set; }

        void InitSkill(SkillData skillData);
        
        void SetIgnoreList(List<int> ignoreList);
        List<int> GetIgnoreList();
        
        void Cast();

        Direction Direction { set; get; }
        Vector3 Speed { set; get; }
        
        // 飞到某个位置, add by TangJian 2019/4/22 10:44
        void FlyTo(Vector3 pos);
        
        // 朝向某个位置, add by TangJian 2019/4/22 10:44
        void TowardTo(Vector3 pos);
    }
}