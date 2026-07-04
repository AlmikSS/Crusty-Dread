using UnityEngine;

namespace GamePlay.Player
{
    public class FishingRodPlaceInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private bool _hasHint;
        [SerializeField] private Sprite _hintSprite;

        public bool HasHint => _hasHint;
        public Sprite HintSprite => _hintSprite;
        
        public void Interact(GameObject interactor)
        {
            if (!interactor.TryGetComponent(out PlayerFishingSystem playerFishingSystem))
            {
                Debug.LogWarning($"PlayerFishingSystem on {interactor.name} not found");
                return;
            }
            
            playerFishingSystem.PickFishingRod();
        }
    }
}