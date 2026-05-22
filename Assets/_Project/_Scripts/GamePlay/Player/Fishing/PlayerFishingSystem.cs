using Core.Input;
using Core.ServiceLocatorDI;
using Core.TicksSystem;
using UnityEngine;

namespace GamePlay.Player
{
    public class PlayerFishingSystem : MonoBehaviour, ITickable
    {
        [SerializeField] private GameObject _fishingRod;

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
            _fishingRod.SetActive(_isFishingRodInHands);
        }
        
        public void OnTick(float deltaTime)
        {
            if (!_inFishZone)
                return;

            if (_isFishingRodInHands && _inputSystem.Snapshot.UseInput)
            {
                Debug.Log("Fishing");
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
                _inFishZone = false;
        }
    }
}