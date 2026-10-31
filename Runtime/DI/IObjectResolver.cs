namespace Anarksoft.SharedCore.DI
{
    public interface IObjectResolver
    {
        T Resolve<T>();
        bool TryResolve<T>(out T service);
    }
}