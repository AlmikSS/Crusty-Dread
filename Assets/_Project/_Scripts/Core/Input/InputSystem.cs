using Core.ServiceLocatorDI;
using Core.TicksSystem;
using UnityEngine;

namespace Core.Input
{
    public class InputSystem : MonoBehaviour, ITickable, IService
    {
        private InputActions _inputActions;
        private InputSnapshot _snapshot;
        private InputContext _context;
        private bool _isConstruct;
        private bool _useInput;
        private bool _openConsole;
        private bool _jumpInput;
        private int _uiOpenedCount;
        private Vector2 _currentMoveInput;
        private Vector2 _currentLookInput;
        
        public TickPhase Phase => TickPhase.InputPhase;
        public InputSnapshot Snapshot => _snapshot;

        public void Construct()
        {
            ServiceLocator.Register(this);
            ServiceLocator.Get<TickSystem>().Register(this);
            _inputActions = new InputActions();
            _inputActions.Enable();
            _isConstruct = true;
        }

        public void OpenUI()
        {
            _uiOpenedCount++;
            _context = InputContext.UI;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void CloseUI()
        {
            _uiOpenedCount--;
            
            if (_uiOpenedCount > 0)
                return;

            _context = InputContext.GamePlay;
            _uiOpenedCount = 0;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        public void OnTick(float deltaTime)
        {
            _useInput = false;
            _openConsole = false;
            _jumpInput = false;

            _snapshot = new InputSnapshot(
                _context,
                _currentMoveInput,
                _currentLookInput,
                _useInput,
                _openConsole,
                _jumpInput);
        }

        private void Update()
        {
            if (!_isConstruct) 
                return;
            
            if (_inputActions.Player.Interact.WasPressedThisFrame())
                _useInput = true;

            if (_inputActions.Player.OpenConsole.WasPressedThisFrame())
                _openConsole = true;

            if (_inputActions.Player.Jump.WasPressedThisFrame())
                _jumpInput = true;

            _currentMoveInput = _inputActions.Player.Move.ReadValue<Vector2>();
            _currentLookInput = _inputActions.Player.Look.ReadValue<Vector2>();

            _snapshot = new InputSnapshot(
                _context,
                _currentMoveInput,
                _currentLookInput,
                _useInput,
                _openConsole,
                _jumpInput);
        }

        private void OnDestroy()
        {
            _isConstruct = false;
            _inputActions.Disable();
            ServiceLocator.Get<TickSystem>().Unregister(this);
        }
    }
}