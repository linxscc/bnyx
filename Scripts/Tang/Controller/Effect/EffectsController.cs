using System.Collections.Generic;
using UnityEngine;

namespace Tang
{
    public class EffectsController
    {
        private Dictionary<int, EffectData> effects = new Dictionary<int, EffectData>();
        private List<int> willRemoveList = new List<int>();

        public System.Action<EffectData> OnBegin;
        public System.Action<EffectData> OnUpdate;
        public System.Action<EffectData> onEnd;

        void addEffect(EffectData effectData)
        {
            effects.Add(Tools.getOnlyId(), effectData);
            OnBegin(effectData);
        }
        void removeEffect(int id)
        {
            if (effects.ContainsKey(id))
            {
                var effectData = effects[id];
                onEnd(effectData);
                effects.Remove(id);
            }
        }
        void Update()
        {
            foreach (var kv in effects)
            {
                var effect = kv.Value;
                effect.time += Time.deltaTime;
                if (effect.time >= effect.duration)
                {
                    willRemoveList.Add(kv.Key);
                }
                else
                {
                    OnUpdate(effect);
                }
            }
            foreach (var id in willRemoveList)
            {
                removeEffect(id);
            }
            willRemoveList.Clear();
        }
    }
}