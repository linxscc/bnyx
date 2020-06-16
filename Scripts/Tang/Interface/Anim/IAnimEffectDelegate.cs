namespace Tang
{
    public interface IAnimEffectDelegate
    {
        AnimEffectData animEffectData { get; set; }
        void PlayAnim(AnimEffectData animEffectData);


    }
}

