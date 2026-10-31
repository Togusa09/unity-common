using UnityEngine;
using UnityEngine.UIElements;
using System;
using Anarksoft.SharedCore.DI;

namespace Anarksoft.SharedCore.UI
{
    [RequireComponent(typeof(PanelRenderer))]
    public abstract class UIToolkitWindowBase : MonoBehaviour, IUIWindow
    {
        [field: SerializeField] public string WindowID { get; private set; }
        
        public bool IsOpen { get; private set; } = false;

        protected PanelRenderer panelRenderer;
        protected VisualElement rootElement;
        private int currentUIVersion = -1;

        public event Action<IUIWindow> OnWindowOpened;
        public event Action<IUIWindow> OnWindowClosed;

        protected virtual void Awake()
        {
            panelRenderer = GetComponent<PanelRenderer>();
            panelRenderer.RegisterUIReloadCallback(OnUIReload);
            
            // Start closed by default via display style
            SetDisplayState(false, instant: true);
        }

        protected virtual void OnDestroy()
        {
            if (panelRenderer != null)
            {
                panelRenderer.UnregisterUIReloadCallback(OnUIReload);
            }
        }

        protected virtual void OnEnable()
        {
            var uiManager = ServiceLocator.Container.Resolve<UIToolkitUIManager>();
            uiManager?.Register(this);
        }

        protected virtual void OnDisable()
        {
            var uiManager = ServiceLocator.Container.Resolve<UIToolkitUIManager>();
            uiManager?.Unregister(this);
        }

        // Triggered natively when PanelRenderer loads or reloads the UXML tree
        private void OnUIReload(PanelRenderer renderer, VisualElement root, int version)
        {
            if (currentUIVersion == version) return;
            currentUIVersion = version;
            rootElement = root;

            // Apply initial visibility based on state
            SetDisplayState(IsOpen, instant: true);
            
            // Allow derived windows to bind buttons/elements safely
            OnUIPrepared(rootElement);
        }

        public virtual void Open()
        {
            if (IsOpen) return;

            IsOpen = true;
            SetDisplayState(true, instant: false);
            OnWindowOpened?.Invoke(this);
            OnOpened();
        }

        public virtual void Close()
        {
            if (!IsOpen) return;

            IsOpen = false;
            SetDisplayState(false, instant: false);
            OnWindowClosed?.Invoke(this);
            OnClosed();
        }

        public virtual void Toggle()
        {
            if (IsOpen) Close();
            else Open();
        }

        private void SetDisplayState(bool show, bool instant)
        {
            if (rootElement == null) return;
            rootElement.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        }

        // Extension hooks for derived window classes
        protected virtual void OnUIPrepared(VisualElement root) { }
        protected virtual void OnOpened() { }
        protected virtual void OnClosed() { }
    }
}