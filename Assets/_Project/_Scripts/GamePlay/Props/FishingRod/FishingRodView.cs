using Core.EventSystem;
using Core.ServiceLocatorDI;
using TriInspector;
using UnityEngine;

namespace GamePlay.Props
{
    public class FishingRodView : MonoBehaviour
    {
        [Title("Animations")]
        [SerializeField] private Animator _animator;
        [SerializeField, AnimatorParameter(nameof(_animator), AnimatorControllerParameterType.Trigger)] private string _startFishingTrigger;
        [SerializeField, AnimatorParameter(nameof(_animator), AnimatorControllerParameterType.Trigger)] private string _endFishingTrigger;

        [Title("Hook")]
        [SerializeField] private Rigidbody _hookRigidbody;
        [SerializeField] private Transform _baseHookOrigin;
        [SerializeField] private Transform _onFishingHookOrigin;
        [SerializeField] private Transform _hookParent;

        private EventBus _eventBus;
        
        private void Start()
        {
            _eventBus = ServiceLocator.Get<EventBus>();
            _eventBus.Register<FishingStartedEvent>(StartFishing);
            _eventBus.Register<FishingEndedEvent>(EndFishing);
        }

        private void OnDestroy()
        {
            _eventBus.Unregister<FishingStartedEvent>(StartFishing);
            _eventBus.Unregister<FishingEndedEvent>(EndFishing);
        }

        private void StartFishing(FishingStartedEvent _)
        {
            _animator.SetTrigger(_startFishingTrigger);
            _hookRigidbody.isKinematic = true;
            _hookRigidbody.transform.SetParent(null);
            _hookRigidbody.transform.position = _onFishingHookOrigin.position;
        }

        private void EndFishing(FishingEndedEvent _)
        {
            _animator.SetTrigger(_endFishingTrigger);
            _hookRigidbody.isKinematic = false;
            _hookRigidbody.transform.SetParent(_hookParent);
            _hookRigidbody.transform.position = _baseHookOrigin.position;
        }
    }
}