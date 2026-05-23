using Core.EventSystem;

namespace GamePlay.Props
{
    public class FishingEvent : IGameEvent
    {
        public readonly bool IsStarted;

        public FishingEvent(bool isStarted)
        {
            IsStarted = isStarted;
        }
    }
}