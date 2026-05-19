using Core.Input;
using Core.ServiceLocatorDI;
using Core.TicksSystem;
using Tools.DevConsole;
using TriInspector;
using UnityEngine;

namespace GamePlay.Player
{
    public class PlayerCamera : MonoBehaviour, ITickable
    {
        [Title("Dependencies")]
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private Transform _orientationTransform;
        [SerializeField] private Transform _lookRoot;
        [SerializeField] private Transform _effectsRoot;
        
        [Title("Base options")]
        [SerializeField, Slider(0f, 100f)] private float _sensitivity;
        [SerializeField, Slider(0, 90f)] private float _xRotationClamp;
        [SerializeField, Slider(0, 1f)] private float _movementVelocityStopThreshold;
        
        [Title("Camera bob options")]
        [SerializeField, Slider(0, 0.1f)] private float _bobAmplitudeX;
        [SerializeField, Slider(0, 0.1f)] private float _bobAmplitudeY;
        [SerializeField, Slider(0, 5)] private float _bobFrequency;
        
        [Title("Movement tilt options")]
        [SerializeField, Slider(0, 15)] private float _movementTiltAmount;
        [SerializeField, Slider(0, 15)] private float _movementTiltSmoothness;
        [SerializeField, Slider(0, 30)] private float _movementTiltClamp;
        
        private InputSystem _inputSystem;
        private Vector3 _lookRotation;
        private Vector3 _movementTiltRotation;
        private Vector3 _bobPosition;
        private float _bobCycle;
        private float _currentMovementTilt;
        
        public TickPhase Phase => TickPhase.PostPhase;

        private void Start()
        {
            _inputSystem = ServiceLocator.Get<InputSystem>();
            ServiceLocator.Get<TickSystem>().Register(this);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnDestroy()
        {
            ServiceLocator.Get<TickSystem>().Unregister(this);
        }
        
        public void OnTick(float deltaTime)
        {
            if (_inputSystem == null || _inputSystem.Snapshot.Context != InputContext.GamePlay)
                return;
            
            var lookInput = _inputSystem.Snapshot.LookInput;
            CalculateBaseMouseLook(deltaTime, lookInput);
            CalculateCameraBob(deltaTime);
            CalculateMovementTilt(deltaTime);
            
            var effectsRotation = _movementTiltRotation;
            var effectsPosition = _bobPosition;
            
            _orientationTransform.rotation = Quaternion.Euler(0f, _lookRotation.y, 0f);
            _lookRoot.localRotation = Quaternion.Euler(_lookRotation);
            _effectsRoot.localRotation = Quaternion.Euler(effectsRotation);
            _effectsRoot.localPosition = effectsPosition;
        }

        private void CalculateBaseMouseLook(float deltaTime, Vector2 lookInput)
        {
            var x = lookInput.x * _sensitivity * deltaTime;
            var y = lookInput.y * _sensitivity * deltaTime;
            
            _lookRotation.x = Mathf.Clamp(_lookRotation.x - y, -_xRotationClamp, _xRotationClamp);
            _lookRotation.y += x;
        }

        private void CalculateCameraBob(float deltaTime)
        {
            var velocity = _playerMovement.Velocity;
            var speed = velocity.magnitude;

            if (speed < _movementVelocityStopThreshold)
                return;
            
            _bobCycle += deltaTime * speed * _bobFrequency;
            
            var bobX = Mathf.Cos(_bobCycle * 0.5f) * _bobAmplitudeX;
            var bobY = Mathf.Sin(_bobCycle) * _bobAmplitudeY;

            _bobPosition = new Vector3(bobX, bobY, 0);
        }

        private void CalculateMovementTilt(float deltaTime)
        {
            var localVelocity = _orientationTransform.InverseTransformDirection(_playerMovement.Velocity);
            var targetTilt = -localVelocity.x * _movementTiltAmount;
            targetTilt = Mathf.Clamp(targetTilt, -_movementTiltClamp, _movementTiltClamp);
            _currentMovementTilt = Mathf.Lerp(_currentMovementTilt, targetTilt, deltaTime * _movementTiltSmoothness);
            _movementTiltRotation = new Vector3(0f, 0f, _currentMovementTilt);
        }

        [Command("set_camera_sens", "Changes camera sensitivity")]
        private void SetSensitivity(float sensitivity)
        {
            _sensitivity = sensitivity;
        }

        [Command("set_camera_clamp", "Changes camera X clamp")]
        private void ChangeCameraClamp(float clamp)
        {
            _xRotationClamp = clamp;
        }
    }
}