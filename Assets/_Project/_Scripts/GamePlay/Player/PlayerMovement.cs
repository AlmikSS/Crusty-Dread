using Core.Input;
using Core.ServiceLocatorDI;
using Core.TicksSystem;
using UnityEngine;

namespace GamePlay.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour, ITickable
    {
        [SerializeField] private Transform _orientationTransform;
        [SerializeField] private float _walkSpeed;
        [SerializeField] private float _gravityScale; 
        
        private CharacterController _cc;
        private InputSystem _inputSystem;
        private Vector3 _moveDirection;
        
        public TickPhase Phase => TickPhase.MainPhase;

        private void Start()
        {
            _cc = GetComponent<CharacterController>();
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

            var moveInput = _inputSystem.Snapshot.MoveInput;
            var direction = _orientationTransform.TransformDirection(new Vector3(moveInput.x, 0, moveInput.y));
            _moveDirection = Vector3.Lerp(_moveDirection, direction, deltaTime * _walkSpeed);

            if (_cc.isGrounded)
                _moveDirection.y = -2f;
            else
                _moveDirection.y += _gravityScale * deltaTime;
            
            _cc.Move(_moveDirection * _walkSpeed * deltaTime);
        }
    }
}