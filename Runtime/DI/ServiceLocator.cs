using System;
using System.Collections.Generic;
using UnityEngine;

namespace Anarksoft.SharedCore.DI
{
    /// <summary>
    /// Minimal simple service locator for use until I get a proper DI container integrated.
    /// </summary>
    public class ServiceLocator : IObjectResolver
    {
        private static ServiceLocator instance;
        public static ServiceLocator Container => instance ??= new ServiceLocator();

        private readonly Dictionary<Type, object> services = new Dictionary<Type, object>();

        // Mimics container configuration/registration
        public void Register<T>(T service)
        {
            var type = typeof(T);
            if (services.ContainsKey(type))
            {
                Debug.LogWarning($"[ServiceLocator] Service of type {type.FullName} is already registered. Overwriting.");
                services[type] = service;
            }
            else
            {
                services.Add(type, service);
            }
        }

        // Mimics VContainer's Resolve API
        public T Resolve<T>()
        {
            var type = typeof(T);
            if (services.TryGetValue(type, out var service))
            {
                return (T)service;
            }

            throw new KeyNotFoundException($"[ServiceLocator] No service of type {type.FullName} has been registered.");
        }

        public bool TryResolve<T>(out T service)
        {
            var type = typeof(T);
            if (services.TryGetValue(type, out var rawService))
            {
                service = (T)rawService;
                return true;
            }

            service = default;
            return false;
        }

        public void Unregister<T>()
        {
            var type = typeof(T);
            if (services.ContainsKey(type))
            {
                services.Remove(type);
            }
        }

        // Optional: Reset container between scenes if needed
        public static void Reset()
        {
            instance = null;
        }
    }
}