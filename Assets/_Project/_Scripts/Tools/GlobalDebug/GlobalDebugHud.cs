using System.Text;
using Core.Input;
using Core.ServiceLocatorDI;
using Core.TicksSystem;
using TMPro;
using Tools.DevConsole;
using UnityEngine;

namespace Tools.GlobalDebug
{
    public class GlobalDebugHud : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _debugText;
        [SerializeField] private GameObject _panelRoot;
        [SerializeField] private float _updateInterval = 0.2f;

        private TickSystem _tickSystem;
        private InputSystem _inputSystem;
        
        private StringBuilder _sb = new();
        private float _updateTimer;
        private bool _isVisible;

        private int _framesCount;
        private float _fpsAccumulator;
        private int _currentFPS;

        private void Start()
        {
            _tickSystem = ServiceLocator.Get<TickSystem>();
            _inputSystem = ServiceLocator.Get<InputSystem>();

            if (_panelRoot != null)
                _panelRoot.SetActive(false);
            else if (_debugText != null)
                _debugText.enabled = false;
        }

        private void Update()
        {
            _framesCount++;
            _fpsAccumulator += Time.unscaledDeltaTime;
            
            if (_fpsAccumulator >= _updateInterval)
            {
                _currentFPS = Mathf.RoundToInt(_framesCount / _fpsAccumulator);
                _framesCount = 0;
                _fpsAccumulator = 0f;
            }

            if (!_isVisible)
                return;

            _updateTimer += Time.unscaledDeltaTime;
            if (_updateTimer >= _updateInterval)
            {
                UpdateDebugText();
                _updateTimer = 0f;
            }
        }

        private void UpdateDebugText()
        {
            if (_debugText == null) return;

            _sb.Clear();

            var frameTimeMs = Time.unscaledDeltaTime * 1000f;
            var allocatedMemoryMB = System.GC.GetTotalMemory(false) / 1048576f;

            _sb.AppendLine("<color=yellow>--- TECHNICAL ---</color>");
            _sb.AppendLine($"FPS: {_currentFPS} ({frameTimeMs:F1} ms)");
            _sb.AppendLine($"Memory: {allocatedMemoryMB:F1} MB");
            _sb.AppendLine();

            _sb.AppendLine("<color=green>--- TICKS ---</color>");
            if (_tickSystem != null)
            {
                _sb.AppendLine($"Target: {_tickSystem.TargetTickRate} Hz");
                _sb.AppendLine($"Real: {_tickSystem.RealTickRate} Hz");
                _sb.AppendLine($"Execution Time: {_tickSystem.TickExecutionTimeMs:F2} ms");
            }
            else
            {
                _sb.AppendLine("TickSystem not found.");
            }
            _sb.AppendLine();

            _sb.AppendLine("<color=blue>--- INPUT ---</color>");
            if (_inputSystem != null)
            {
                var snap = _inputSystem.Snapshot;
                _sb.AppendLine($"Context: {snap.Context}");
                _sb.AppendLine($"Move Input: [X: {snap.MoveInput.x:F1}, Y: {snap.MoveInput.y:F1}]");
                _sb.AppendLine($"Look Input: [X: {snap.LookInput.x:F1}, Y: {snap.LookInput.y:F1}]");
            }
            else
            {
                _sb.AppendLine("InputSystem not found.");
            }

            _debugText.text = _sb.ToString();
        }

        [Command("toggle_debug_hud", "Toggles the global debug information panel")]
        private void ToggleDebugHUD(bool enable)
        {
            _isVisible = enable;
            if (_panelRoot != null)
            {
                _panelRoot.SetActive(_isVisible);
            }
            else if (_debugText != null)
            {
                _debugText.enabled = _isVisible;
            }
            Debug.Log($"Debug HUD {(_isVisible ? "Enabled" : "Disabled")}");
        }
    }
}