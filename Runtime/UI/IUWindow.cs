namespace Anarksoft.SharedCore.UI
{
    public interface IUIWindow
    {
        string WindowID { get; }
        bool IsOpen { get; }
        
        void Open();
        void Close();
        void Toggle();
    }
}