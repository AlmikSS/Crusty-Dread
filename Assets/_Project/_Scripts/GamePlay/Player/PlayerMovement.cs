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
        [SerializeField] private float _acceleration;
        [SerializeField] private float _gravityScale; 
        
        private CharacterController _cc;
        private InputSystem _inputSystem;
        private Vector3 _horizontalVelocity;
        private float _verticalVelocity;
        
        public TickPhase Phase => TickPhase.MainPhase;
        public Vector3 Velocity => _horizontalVelocity;

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
            if (_inputSystem == null || _inputSystem.Snapshot.Context != InputContext.GamePlay)
                return;

            var moveInput = _inputSystem.Snapshot.MoveInput;
            var input = new Vector3(moveInput.x, 0, moveInput.y);
            input = Vector3.ClampMagnitude(input, 1f);

            var worldDirection = _orientationTransform.TransformDirection(input);
            var targetVelocity = worldDirection * _walkSpeed;
            _horizontalVelocity = Vector3.Lerp(_horizontalVelocity, targetVelocity, _acceleration * deltaTime);

            if (_cc.isGrounded)
                _verticalVelocity = -2f;
            else
                _verticalVelocity += _gravityScale * deltaTime;
            
            var finalVelocity = _horizontalVelocity + Vector3.up * _verticalVelocity;
            _cc.Move(finalVelocity * deltaTime);
        }
    }
}