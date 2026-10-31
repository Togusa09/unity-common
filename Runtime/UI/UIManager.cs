using System.Collections.Generic;
using UnityEngine;

namespace Anarksoft.SharedCore.UI
{
    public class UIManager : MonoBehaviour
    {
        private readonly Dictionary<string, IUIWindow> registeredWindows = new Dictionary<string, IUIWindow>();
        private readonly Stack<IUIWindow> openWindowsStack = new Stack<IUIWindow>();

        public void RegisterWindow(IUIWindow window)
        {
            if (!registeredWindows.ContainsKey(window.WindowID))
            {
                registeredWindows.Add(window.WindowID, window);
                window.OnWindowOpened += HandleWindowOpened;
                window.OnWindowClosed += HandleWindowClosed;
            }
        }

        public void UnregisterWindow(IUIWindow window)
        {
            if (registeredWindows.ContainsKey(window.WindowID))
            {
                window.OnWindowOpened -= HandleWindowOpened;
                window.OnWindowClosed -= HandleWindowClosed;
                registeredWindows.Remove(window.WindowID);
            }
        }

        public void OpenWindow(string windowID)
        {
            if (registeredWindows.TryGetValue(windowID, out var window))
            {
                window.Open();
            }
            else
            {
                Debug.LogWarning($"[UIManager] Window with ID '{windowID}' not found.");
            }
        }

        public void CloseWindow(string windowID)
        {
            if (registeredWindows.TryGetValue(windowID, out var window))
            {
                window.Close();
            }
        }

        public void CloseTopWindow()
        {
            if (openWindowsStack.Count > 0)
            {
                var topWindow = openWindowsStack.Peek();
                topWindow.Close();
            }
        }

        private void HandleWindowOpened(IUIWindow window)
        {
            if (!openWindowsStack.Contains(window))
            {
                openWindowsStack.Push(window);
            }
        }

        private void HandleWindowClosed(IUIWindow window)
        {
            // Rebuild stack if a window lower down closes out of order
            if (openWindowsStack.Contains(window))
            {
                var tempStack = new Stack<IUIWindow>();
                while (openWindowsStack.Count > 0)
                {
                    var current = openWindowsStack.Pop();
                    if (current == window) break;
                    tempStack.Push(current);
                }
                while (tempStack.Count > 0)
                {
                    openWindowsStack.Push(tempStack.Pop());
                }
            }
        }
    }
}