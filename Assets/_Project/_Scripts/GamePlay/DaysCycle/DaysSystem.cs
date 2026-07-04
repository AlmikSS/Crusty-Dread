using System.Collections.Generic;
using Core.EventSystem;
using Core.ServiceLocatorDI;
using GamePlay.Props;
using GamePlay.Tools;
using UnityEngine;

namespace Core.DaysCycle
{
    public sealed class DaysSystem : MonoBehaviour, IService
    {
        [SerializeField] private List<DayDataSO> _days = new();
        
        private EventBus _eventBus;
        private UIPopupShower _popupShower;
        private int _currentDay;
        
        public void Construct()
        {
            ServiceLocator.Register(this);
            _eventBus = ServiceLocator.Get<EventBus>();
            _popupShower = ServiceLocator.Get<UIPopupShower>();
            _eventBus.Register<FishingSuccessEvent>(OnFishingSuccess);
            _eventBus.Register<FishingFailedEvent>(OnFishingFailed);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister(this);
            _eventBus.Unregister<FishingSuccessEvent>(OnFishingSuccess);
            _eventBus.Unregister<FishingFailedEvent>(OnFishingFailed);
        }

        private void OnFishingSuccess(FishingSuccessEvent fishingSuccessEvent)
        {
            _popupShower.ShowColor(Color.green);
        }

        private void OnFishingFailed(FishingFailedEvent fishingFailedEvent)
        {
            _popupShower.ShowColor(Color.red);
        }
    }
}