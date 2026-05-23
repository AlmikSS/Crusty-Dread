using Core.Input;
using Core.ServiceLocatorDI;
using Core.TicksSystem;
using GamePlay.Props;
using UnityEngine;

namespace GamePlay.Player
{
    public class PlayerFishingSystem : MonoBehaviour, ITickable
    {
        [SerializeField] private FishingRod _fishingRod;

        private InputSystem _inputSystem;
        private bool _isFishingRodInHands;
        private bool _inFishZone;
        
        public TickPhase Phase => TickPhase.MainPhase;

        private void Start()
        {
            _inputSystem = ServiceLocator.Get<InputSystem>();
            ServiceLocator.Get<TickSystem>().Register(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Get<TickSystem>().Unregister(this);
        }

        public void FishingRod()
        {
            _isFishingRodInHands = !_isFishingRodInHands;
            _fishingRod.gameObject.SetActive(_isFishingRodInHands);
        }
        
        public void OnTick(float deltaTime)
        {
            if (!_inFishZone)
                return;

            var snapshot = _inputSystem.Snapshot;
            if (snapshot.Context != InputContext.GamePlay)
                return;

            if (_isFishingRodInHands && snapshot.UseInput)
            {
                _fishingRod.Release();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("FishingZone"))
                _inFishZone = true;
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("FishingZone"))
            {
                _inFishZone = false;
                _fishingRod.StopFishing();
            }
        }
    }
}