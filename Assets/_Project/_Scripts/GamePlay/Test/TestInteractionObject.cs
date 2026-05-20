using GamePlay.Player;
using UnityEngine;

namespace GamePlay.Test
{
    public class TestInteractionObject : MonoBehaviour, IInteractable
    {
        [SerializeField] private bool _hasHint;
        [SerializeField] private Sprite _hintSprite;

        public bool HasHint => _hasHint;
        public Sprite HintSprite => _hintSprite;
        
        public void Interact()
        {
            Debug.Log("Interact with " + gameObject.name);
        }
    }
}