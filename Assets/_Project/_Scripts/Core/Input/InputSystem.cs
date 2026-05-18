using Core.ServiceLocatorDI;
using Core.TicksSystem;
using UnityEngine;

namespace Core.Input
{
    public class InputSystem : MonoBehaviour, ITickable, IService
    {
        private InputActions _inputActions;
        private InputSnapshot _snapshot;
        private bool _isUseInput;
        private bool _isConstruct;
        
        public TickPhase Phase => TickPhase.InputPhase;
        public InputSnapshot Snapshot => _snapshot;

        public void Construct()
        {
            ServiceLocator.Register(this);
            _inputActions = new InputActions();
            _isConstruct = true;
        }
        
        public void OnTick(float deltaTime)
        {
            var moveInput = _inputActions.Player.Move.ReadValue<Vector2>();
            var lookInput = _inputActions.Player.Look.ReadValue<Vector2>();

            _snapshot = new InputSnapshot(
                moveInput,
                lookInput,
                _isUseInput);
            
            _isUseInput = false;
        }

        private void Update()
        {
            if (!_isConstruct) 
                return;
            
            if (_inputActions.Player.Interact.WasPressedThisFrame())
                _isUseInput = true;
        }
        
        private void OnEnable()
        {
            _inputActions.Enable();
        }

        private void OnDisable()
        {
            _inputActions.Disable();
        }
    }
}