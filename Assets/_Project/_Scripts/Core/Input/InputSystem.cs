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
        private bool _useInput;
        private bool _openConsole;
        private bool _isConstruct;
        private int _uiOpenedCount;
        
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
            var moveInput = _inputActions.Player.Move.ReadValue<Vector2>();
            var lookInput = _inputActions.Player.Look.ReadValue<Vector2>();

            _snapshot = new InputSnapshot(
                _context,
                moveInput,
                lookInput,
                _useInput,
                _openConsole);
            
            _useInput = false;
            _openConsole = false;
        }

        private void Update()
        {
            if (!_isConstruct) 
                return;
            
            if (_inputActions.Player.Interact.WasPressedThisFrame())
            {
                Debug.Log("Interact");
                _useInput = true;
            }

            if (_inputActions.Player.OpenConsole.WasPressedThisFrame())
            {
                Debug.Log("OpenConsole");
                _openConsole = true;
            }
        }

        private void OnDestroy()
        {
            _isConstruct = false;
            _inputActions.Disable();
            ServiceLocator.Get<TickSystem>().Unregister(this);
        }
    }
}