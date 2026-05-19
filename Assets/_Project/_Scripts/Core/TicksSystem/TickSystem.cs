using System;
using System.Collections.Generic;
using System.Diagnostics;
using Core.ServiceLocatorDI;
using Tools.DevConsole;
using TriInspector;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Core.TicksSystem
{
    public class TickSystem : MonoBehaviour, IService
    {
        [SerializeField] private int _ticksPerSecond = 60;

        private readonly SortedDictionary<TickPhase, List<ITickable>> _tickables = new();
        private readonly Queue<ITickable> _registerQueue = new();
        private readonly Queue<ITickable> _unregisterQueue = new();
        
        private float _tickInterval;
        private float _accumulator;
        private bool _isConstruct;
        private bool _isTicksStarted;
        private int _tickCounter;
        private float _tickTimer;
        private Stopwatch _stopwatch = new Stopwatch();
        
        public int TargetTickRate => _ticksPerSecond;
        public int RealTickRate { get; private set; }
        public float TickExecutionTimeMs { get; private set; }

        public void Construct()
        {
            ServiceLocator.Register(this);
            
            SetTickSettings();

            foreach (TickPhase phase in Enum.GetValues(typeof(TickPhase)))
            {
                _tickables[phase] = new List<ITickable>();
            }
            
            _isConstruct = true;
        }

        public void StartTicks()
        {
            if (!_isConstruct)
                return;

            _isTicksStarted = true;
            Debug.Log("Ticks started");
        }
        
        public void StopTicks()
        {
            _isTicksStarted = false;
            Debug.Log("Ticks stopped");
        }
        
        public void Register(ITickable tickable) => _registerQueue.Enqueue(tickable);
        public void Unregister(ITickable tickable) => _unregisterQueue.Enqueue(tickable);
        
        private void Update()
        {
            if (!_isConstruct)
                return;
            
            _tickTimer += Time.unscaledDeltaTime;
            if (_tickTimer >= 1f)
            {
                RealTickRate = _tickCounter;
                _tickCounter = 0;
                _tickTimer -= 1f;
            }

            _accumulator += Time.deltaTime;
            while (_accumulator >= _tickInterval)
            {
                _stopwatch.Restart();
                
                Tick(_tickInterval);
                _tickCounter++;
                
                _stopwatch.Stop();
                TickExecutionTimeMs = (float)_stopwatch.Elapsed.TotalMilliseconds;
                
                _accumulator -= _tickInterval;
            }
        }

        private void Tick(float deltaTime)
        {
            foreach (var tickables in _tickables.Values)
            {
                foreach (var tickable in tickables)
                {
                    if (tickable.Phase is TickPhase.InputPhase or TickPhase.SystemPhase)
                    {
                        tickable.OnTick(deltaTime);
                        continue;
                    }
                    
                    if (_isTicksStarted)
                        tickable.OnTick(deltaTime);
                }
            }
            
            ReleaseRegisterQueue();
            ReleaseUnregisterQueue();
        }

        private void ReleaseRegisterQueue()
        {
            while (_registerQueue.Count > 0)
            {
                var newTickable = _registerQueue.Dequeue();
                var phase = newTickable.Phase;
                
                if (_tickables[phase].Contains(newTickable))
                    continue;
                
                _tickables[phase].Add(newTickable);
                Debug.Log($"Registered tickable {newTickable.GetType().Name}");
            }
        }

        private void ReleaseUnregisterQueue()
        {
            while (_unregisterQueue.Count > 0)
            {
                var tickable = _unregisterQueue.Dequeue();
                var phase = tickable.Phase;
                
                if (!_tickables[phase].Contains(tickable))
                    continue;
                
                _tickables[phase].Remove(tickable);
                Debug.Log($"Unregistered tickable {tickable.GetType().Name}");
            }
        }
        
        [Button(ButtonSizes.Medium, "Recalculate ticks")]
        private void SetTickSettings()
        {
            _tickInterval = 1f / _ticksPerSecond;
            _accumulator = 0f;
        }

        [Command("change_tickRate", "Changes game tick rate")]
        private void SetTickRate(int tickRate)
        {
            _ticksPerSecond = tickRate;
            SetTickSettings();
            Debug.Log("Tick rate changed");
        }
    }
}