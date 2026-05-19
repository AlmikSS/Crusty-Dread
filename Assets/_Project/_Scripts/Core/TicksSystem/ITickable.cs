namespace Core.TicksSystem
{
    public interface ITickable
    {
        TickPhase Phase { get; }
        void OnTick(float deltaTime);
    }
}