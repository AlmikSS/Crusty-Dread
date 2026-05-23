using System;
using System.Collections.Generic;
using Core.ServiceLocatorDI;
using UnityEngine;

namespace Core.EventSystem
{
    public sealed class EventBus : IService, IDisposable
    {
        private readonly Dictionary<Type, List<Delegate>> _handlers = new();
        
        public EventBus() { ServiceLocator.Register(this); }

        public void Dispose() { ServiceLocator.Unregister(this); }

        public void Register<T>(Action<T> handler) where T : IGameEvent
        {
            if (handler == null)
                return;
            
            var type = typeof(T);
            if (!_handlers.ContainsKey(type))
                _handlers.Add(type, new List<Delegate>());

            if (_handlers[type].Contains(handler))
            {
                Debug.LogWarning($"[EventBus] { handler.Method.Name } is already registered");
                return;
            }
            
            _handlers[type].Add(handler);
            Debug.Log($"[EventBus] { handler.Method.Name } registered");
        }

        public void Unregister<T>(Action<T> handler) where T : IGameEvent
        {
            if (handler == null)
                return;
            
            var type = typeof(T);
            if (!_handlers.ContainsKey(type))
                return;
            
            _handlers[type].Remove(handler);
            if (_handlers[type].Count == 0)
                _handlers.Remove(type);
            
            Debug.Log($"[EventBus] { handler.Method.Name } unregistered");
        }

        public void Publish<T>(T @event) where T : IGameEvent
        {
            if (@event == null)
                return;
            
            var type = typeof(T);
            if (!_handlers.ContainsKey(type))
                return;
            
            var snapshot = _handlers[type].ToArray();
            foreach (var handler in snapshot)
            {
                try
                {
                    var action = handler as Action<T>;
                    action?.Invoke(@event);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[EventBus] Failed to invoke handler. Error: { ex.Message }");
                }
            }
        }
    }
}