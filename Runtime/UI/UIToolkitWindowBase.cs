using UnityEngine;
using UnityEngine.UIElements;
using System;
using Anarksoft.SharedCore.DI;

namespace Anarksoft.SharedCore.UI
{
    [ExecuteAlways] // Allows the component to run in Edit Mode for previewing
    [RequireComponent(typeof(PanelRenderer))]
    public abstract class UIToolkitWindowBase : MonoBehaviour, IUIWindow
    {
        [field: SerializeField] public string WindowID { get; private set; }
        
        [Header("Editor Preview")]
        [Tooltip("Toggle this in the inspector to preview the window open/closed while editing scenes.")]
        [SerializeField] private bool previewOpenInEditor = false;

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
            
            if (Application.isPlaying)
            {
                SetDisplayState(false, instant: true);
            }
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
            if (Application.isPlaying)
            {
                var uiManager = ServiceLocator.Container.Resolve<UIToolkitUIManager>();
                uiManager?.Register(this);
            }
        }

        protected virtual void OnDisable()
        {
            if (Application.isPlaying)
            {
                var uiManager = ServiceLocator.Container.Resolve<UIToolkitUIManager>();
                uiManager?.Unregister(this);
            }
        }

        // Handles updates when tweaking values in the Inspector during Edit Mode
        protected virtual void OnValidate()
        {
            if (!Application.isPlaying)
            {
                // If panel renderer isn't cached yet in edit mode, grab it
                if (panelRenderer == null) panelRenderer = GetComponent<PanelRenderer>();
                
                if (rootElement != null)
                {
                    SetDisplayState(previewOpenInEditor, instant: true);
                }
                // Force display state update based on inspector boolean
                else if (panelRenderer != null && panelRenderer.visualTreeAsset != null)
                {
                    // In edit mode, we can evaluate the root directly if available
                    // Note: PanelRenderer manages its own visual tree evaluation in edit mode
                }
            }
        }

        // Triggered natively when PanelRenderer loads or reloads the UXML tree
        private void OnUIReload(PanelRenderer renderer, VisualElement root, int version)
        {
            if (currentUIVersion == version) return;
            currentUIVersion = version;
            rootElement = root;

            // Apply initial visibility based on state
            if (Application.isPlaying)
            {
                SetDisplayState(IsOpen, instant: true);
            }
            else
            {
                // In edit mode, obey the inspector preview toggle
                SetDisplayState(previewOpenInEditor, instant: true);
            }
            
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