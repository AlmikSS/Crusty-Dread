using Core.Input;
using Core.ServiceLocatorDI;
using Core.TicksSystem;
using Tools.DevConsole;
using UnityEngine;

namespace GamePlay.Tools
{
    public class DevConsoleExecutor : MonoBehaviour, ITickable
    {
        [SerializeField] private DevConsoleUI _devConsoleUI;
        
        private InputSystem _inputSystem;
        
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
        
        public void OnTick(float deltaTime)
        {
            if (_inputSystem == null)
                return;
            
            var snapshot = _inputSystem.Snapshot;
            if (snapshot.OpenConsole)
            {
                if (!_devConsoleUI.IsOpened)
                    Open();
                else
                    Close();
            }
        }

        private void Open()
        {
            _devConsoleUI.Open();
            _inputSystem.OpenUI();
        }
        
        private void Close()
        {
            _devConsoleUI.Close();
            _inputSystem.CloseUI();
        }
    }
}