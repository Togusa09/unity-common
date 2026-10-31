# How to Use It (And Why it Transitions Easily to VContainer)

## Registering Services (During Bootstrap)

In your game startup or boot sequence (e.g., your SharedCoreSystem or a scene bootstrap script), you register your core systems:
```c#

using Anarksoft.SharedCore.DI;
using Anarksoft.SharedCore.UI;
using UnityEngine;

namespace Anarksoft.SharedCore
{
    public static class SharedCoreSystem
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            // Register your global shared services
            ServiceLocator.Container.Register<UIToolkitUIManager>(new UIToolkitUIManager());
            
            Debug.Log("[SharedCore] SharedCoreSystem initialized with ServiceLocator.");
        }
    }
}
```

## Resolving & Consuming Services

When writing gameplay, UI, or interaction code, you fetch dependencies using the exact same Resolve<T>() method call structure you would use in VContainer:
```c#

using Anarksoft.SharedCore.DI;
using UnityEngine;

namespace Anarksoft.SharedCore.Gameplay
{
    public class ChestInteraction : MonoBehaviour
    {
        public void OpenContainer()
        {
            // Clean API matching VContainer resolver patterns
            var uiManager = ServiceLocator.Container.Resolve<UIToolkitUIManager>();
            uiManager.OpenWindow("LootContainer");
        }
    }
}
```

Why this makes switching to VContainer easy later:

When you eventually transition your game project to VContainer, your classes won't need to change their lookup signatures. Because both use a resolver pattern (Resolve<T>()), you will only need to swap how dependencies are injected (moving from ServiceLocator.Container.Resolve<T>() to constructor injection or ObjectResolver parameters provided by VContainer's lifetime scopes). Your shared package code remains completely decoupled!