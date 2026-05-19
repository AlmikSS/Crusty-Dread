using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.ServiceLocatorDI
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new();

        public static void Register<T>(T service) where T : class, IService
        {
            var type = typeof(T);
            
            if (_services.ContainsKey(type))
            {
                Debug.LogWarning("Try to register existing service");
                return;
            }
            
            _services[type] = service;
        }

        public static void Unregister<T>(T service) where T : class, IService
        {
            var type = typeof(T);
            
            if (!_services.ContainsKey(type))
            {
                Debug.LogWarning("Try to unregister non existing service");
                return;
            }
            
            _services.Remove(type);
        }

        public static T Get<T>() where T : class, IService
        {
            if (_services.TryGetValue(typeof(T), out var service))
                return (T)service;
            
            Debug.LogWarning("Try to get non existing service");
            return null;
        }

        public static void Clear()
        {
            _services.Clear();
        }
    }
}