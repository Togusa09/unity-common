using System;

namespace Anarksoft.SharedCore.UI
{
    public interface IUIWindow
    {
        string WindowID { get; }
        bool IsOpen { get; }

        public event Action<IUIWindow> OnWindowOpened;
        public event Action<IUIWindow> OnWindowClosed;
        
        void Open();
        void Close();
        void Toggle();
    }
}