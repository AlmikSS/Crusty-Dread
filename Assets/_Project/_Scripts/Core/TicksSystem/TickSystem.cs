using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.TicksSystem
{
    public class TickSystem : MonoBehaviour
    {
        [SerializeField] private int _ticksPerSecond = 60;

        private readonly SortedDictionary<TickPhase, List<ITickable>> _tickables = new();
        private readonly Queue<ITickable> _registerQueue = new();
        private readonly Queue<ITickable> _unregisterQueue = new();
        
        private float _tickInterval;
        private float _accumulator;
        private bool _isConstruct;

        public void Construct()
        {
            //TODO register to Service locator
            
            _tickInterval = 1f / _ticksPerSecond;
            _accumulator = 0f;

            foreach (TickPhase phase in Enum.GetValues(typeof(TickPhase)))
            {
                _tickables[phase] = new List<ITickable>();
            }
            
            _isConstruct = true;
        }

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
                    tickable?.OnTick(deltaTime);
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