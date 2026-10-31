using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace Anarksoft.SharedCore.UI
{
    [RequireComponent(typeof(UIDocument))]
    public abstract class UIToolkitWindowBase : MonoBehaviour, IUIWindow
    {
        [field: SerializeField] public string WindowID { get; private set; }
        
        public bool IsOpen { get; private set; } = false;

        protected UIDocument uiDocument;
        protected VisualElement rootElement;

        public event Action<IUIWindow> OnWindowOpened;
        public event Action<IUIWindow> OnWindowClosed;

        protected virtual void Awake()
        {
            uiDocument = GetComponent<UIDocument>();
            if (uiDocument != null && uiDocument.rootVisualElement != null)
            {
                rootElement = uiDocument.rootVisualElement;
            }
            
            // Start closed by default
            CloseInternal(instant: true);
        }

        public virtual void Open()
        {
            if (IsOpen) return;

            IsOpen = true;
            SetDisplayState(true);
            OnWindowOpened?.Invoke(this);
            
            OnOpened();
        }

        public virtual void Close()
        {
            if (!IsOpen) return;

            IsOpen = false;
            SetDisplayState(false);
            OnWindowClosed?.Invoke(this);
            
            OnClosed();
        }

        public virtual void Toggle()
        {
            if (IsOpen) Close();
            else Open();
        }

        private void CloseInternal(bool instant)
        {
            IsOpen = false;
            SetDisplayState(false);
        }

        private void SetDisplayState(bool show)
        {
            if (rootElement == null)
            {
                if (uiDocument != null) rootElement = uiDocument.rootVisualElement;
                if (rootElement == null) return;
            }

            // UI Toolkit display toggle
            rootElement.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        }

        // Extension hooks for derived projects (e.g., binding data to elements, starting open/close animations via USS classes)
        protected virtual void OnOpened() { }
        protected virtual void OnClosed() { }
    }
}