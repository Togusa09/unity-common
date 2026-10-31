using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;

namespace Anarksoft.SharedCore.UI
{
    public class UIToolkitUIManager : MonoBehaviour
    {
        private readonly Dictionary<Type, IUIWindow> windows = new Dictionary<Type, IUIWindow>();

        public void Register<T>(T window) where T : IUIWindow
        {
            var type = typeof(T);
            if (!windows.ContainsKey(type))
            {
                windows.Add(type, window);
            }
        }

        public void Open<T>() where T : IUIWindow
        {
            if (windows.TryGetValue(typeof(T), out var window))
            {
                window.Open();
            }
        }

        public void Close<T>() where T : IUIWindow
        {
            if (windows.TryGetValue(typeof(T), out var window))
            {
                window.Close();
            }
        }
    }
}