using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Tang;
using UnityEngine.AddressableAssets;
using Random = UnityEngine.Random;


namespace ZS
{
    public class BothAnimManager
    {
//        [RuntimeInitializeOnLoadMethod]
        public static void Init()
        {
            instance = new BothAnimManager();
            instance.LoadJson();
        }

        private static BothAnimManager instance;
        public static BothAnimManager Instance
        {
            get
            {
                if (instance!=null)
                {
                    return instance;
                }
                else
                {
                    throw new Exception("BothAnimManager未实例化完成！");
                }
            }
        }
        
        public Dictionary<string, HitAndMatData> bothEffectAnims = new Dictionary<string, HitAndMatData>();
        public Dictionary<string, EffectAnim> EffectAnims = new Dictionary<string, EffectAnim>();
        public Dictionary<string,HitAndMatData> TerrainEffectAnims = new Dictionary<string, HitAndMatData>();
        
        private async void LoadJson()
        {
            var bothEffectAnimsTextAssetTask = AssetManager.LoadAssetAsync<TextAsset>("bothEffectAnims");
            var bothEffectAnimsTextAsset = await bothEffectAnimsTextAssetTask;
            bothEffectAnims = Tools.Json2Obj<Dictionary<string, HitAndMatData>>(bothEffectAnimsTextAsset.text);
            
            var effectsTextAssetTask = AssetManager.LoadAssetAsync<TextAsset>("effects");
            var effectsTextAsset = await effectsTextAssetTask;
            EffectAnims = Tools.Json2Obj<Dictionary<string, EffectAnim>>(effectsTextAsset.text);

            var terrainEffectTextAssetTask = AssetManager.LoadAssetAsync<TextAsset>("TerrainHurtEffect");
            var terrainEffectTextAsset = await terrainEffectTextAssetTask;
            TerrainEffectAnims = Tools.Json2Obj<Dictionary<string, HitAndMatData>>(terrainEffectTextAsset.text);

            foreach (var item in TerrainEffectAnims)
            {
                if (!bothEffectAnims.ContainsKey(item.Key))
                {
                    bothEffectAnims.Add(item.Key,item.Value);    
                }
            }
            TerrainEffectAnims.Clear(); 
        }

        public HitAndMatData GetHitAndMatData(HitEffectType hitEffectType, MatType matType)
        {
            string key = hitEffectType.ToString().ToLower() + "_" + matType.ToString().ToLower();
            return GetHitAndMatData(key);
        }
        
        public HitAndMatData GetHitAndMatData(string key)
        {
            HitAndMatData hitAndMatData;
            if (bothEffectAnims.TryGetValue(key, out hitAndMatData))
            {
                return hitAndMatData;
            }

            return null;
        }

        public EffectAnim GetEffectAnim(string name)
        {
            EffectAnim ret;
            if (EffectAnims.TryGetValue(name, out ret))
            {
                return ret;
            }

            return null;
        }
        
        public async void PlayHitEffect(HitEffectType hitEffectType, MatType matType, Vector3 pos, Direction direction, float rendererAngle)
        {
            rendererAngle = 0;

            HitAndMatData hitAndMatData = GetHitAndMatData(hitEffectType, matType);
            if (hitAndMatData == null) return;
            EffectAnim effectAnim = GetEffectAnim(hitAndMatData.HitEffectId);

            if (effectAnim == null) return;

            switch (effectAnim._AnimType)
            {
                case AnimType.spine:
                    GameObject animObject = await AnimManager.Instance.PlayAnim(Tools.getOnlyId().ToString(), 
                        Definition.SpineAssetPath + "/" + effectAnim.AnimName + ".asset", effectAnim.Anim1, false, pos, (int)direction);
                    animObject.transform.eulerAngles = new Vector3(animObject.transform.eulerAngles.x, animObject.transform.eulerAngles.y, rendererAngle);
                    break;
                case AnimType.effect:
                    if (direction == Direction.Right)
                    {
                        AnimManager.Instance.PlayAnimEffect(effectAnim.AnimName, pos, rendererAngle);
                    }
                    else
                    {
                        AnimManager.Instance.PlayAnimEffect(effectAnim.AnimName, pos, rendererAngle, true);
                    }
                    break;
                default:
                    Debug.LogError("不存在类型:" + effectAnim._AnimType);
                    break;
            }
        }

        public void PlayHurtEffect(HitEffectType hitEffectType, MatType matType, Vector3 pos,Direction direction, float rendererAngle, Vector3 force)
        {
            HitAndMatData hitAndMatData = GetHitAndMatData(hitEffectType, matType);
            if (hitAndMatData == null) return;

            EffectAnim effectAnim = GetEffectAnim(hitAndMatData.HurtEffectId);
            
            if (effectAnim == null) return;

            switch (effectAnim._AnimType)
            {
                case AnimType.spine:
                    switch (effectAnim.mode)
                    {    
                        case modeType.random:
                            AnimManager.Instance.PlayAnim(Tools.getOnlyId().ToString(),
                                Definition.SpineAssetPath + "/" + effectAnim.AnimName + ".asset", choiceAnim(effectAnim),
                                false, pos,(int)direction);
                            
                            break;
                        default:
                            AnimManager.Instance.PlayAnim(Tools.getOnlyId().ToString(),
                                Definition.SpineAssetPath + "/" + effectAnim.AnimName + ".asset", effectAnim.Anim1,
                                false, pos,(int)direction);
                            break;
                            
                    }
                    AnimManager.Instance.PlayAnim(Tools.getOnlyId().ToString(), Definition.SpineAssetPath + "/" + effectAnim.AnimName + ".asset", 
                        effectAnim.Anim1, false, pos,(int)direction);
                    break;
                case AnimType.effect:
                    if (direction == Direction.Right)
                    {
                        AnimManager.Instance.PlayAnimEffect(effectAnim.AnimName, pos, 0, false, Vector3.zero, null, 0,
                            controller =>
                            {

                            });
                    }
                    else
                    {
                        AnimManager.Instance.PlayAnimEffect(effectAnim.AnimName, pos,rendererAngle, true);
                    }
                    break;
                default:
                    Debug.LogError("不存在类型:" + effectAnim._AnimType);
                    break;
            }
        }

        private string choiceAnim(EffectAnim effectAnim)
        {
            int temp = Random.Range(1, 6);
            switch (temp)
            {
                case 1:
                    return effectAnim.Anim1;
                    break;
                case 2:
                    return effectAnim.Anim2;
                    break;
                case 3:
                    return effectAnim.Anim3;
                    break;
                default:
                    return effectAnim.Anim1;
            }
        }
    }
    

}