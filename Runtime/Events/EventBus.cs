using System;
using System.Collections.Generic;

namespace Anarksoft.SharedCore.Events
{
    /// <summary>
    /// Minimal event bus implementation to serve as a starting point for development
    /// </summary>
    public class EventBus
    {
        private readonly Dictionary<Type, Delegate> _handlers = new();

        public void Subscribe<T>(Action<T> handler) where T : IBusEvent
        {
            _handlers.TryGetValue(typeof(T), out var existing);
            _handlers[typeof(T)] = existing == null ? handler : Delegate.Combine(existing, handler);
        }

        public void Unsubscribe<T>(Action<T> handler) where T : IBusEvent
        {
            if (!_handlers.TryGetValue(typeof(T), out var existing))
            {
                return;
            }

            var combined = Delegate.Remove(existing, handler);
            if (combined == null)
            {
                _handlers.Remove(typeof(T));
            }
            else
            {
                _handlers[typeof(T)] = combined;
            }
        }

        public void Publish<T>(T eventData) where T : IBusEvent
        {
            if (_handlers.TryGetValue(typeof(T), out var existing))
            {
                ((Action<T>)existing)?.Invoke(eventData);
            }
        }
    }
}