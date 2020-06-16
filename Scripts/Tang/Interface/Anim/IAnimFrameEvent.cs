using Spine.Unity;





namespace Tang
{
    using FrameEvent;
    
    public interface IAnimFrameEvent
    {
        SkeletonAnimation skeletonAnimation();
        void ReplaceEvent(FrameEventData eventData, FrameEventData srcEventData);
    }
}