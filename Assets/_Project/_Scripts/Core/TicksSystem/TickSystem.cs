using System;
using System.Collections.Generic;
using Core.ServiceLocatorDI;
using UnityEngine;

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

        public void Construct()
        {
            ServiceLocator.Register(this);
            
            _tickInterval = 1f / _ticksPerSecond;
            _accumulator = 0f;

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
        }
        
        public void StopTicks() => _isTicksStarted = false;
        
        public void Register(ITickable tickable) => _registerQueue.Enqueue(tickable);
        public void Unregister(ITickable tickable) => _unregisterQueue.Enqueue(tickable);
        
        private void Update()
        {
            if (!_isConstruct)
                return;
            
            _accumulator += Time.deltaTime;
            while (_accumulator >= _tickInterval)
            {
                Tick(_tickInterval);
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
            }
        }
    }
}