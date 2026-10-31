# How-To Guide: UI Toolkit Windows & Management

This guide explains how to create, register, and manage in-game UI windows (such as inventories, stores, container interfaces, and pause menus) using Unity's UI Toolkit and the shared core package (com.anarksoft.sharedcore).

## 1. Overview of the Architecture

- `IUIWindow`: A lightweight interface defining basic states and methods (`Open()`, `Close()`, `Toggle()`, `IsOpen`).

- `UIToolkitWindowBase`: An abstract base component that handles UI Toolkit root visibility (`DisplayStyle.Flex` vs `DisplayStyle.None`) and automatically registers/unregisters the window with the UI manager.

- `UIToolkitUIManager`: A centralized service (managed via your Service Locator) that tracks active windows and manages state.

## 2. Step-by-Step: Creating a New UI Window

### Step A: Create the UI Document & Stylesheet

1. In your Unity project, create a new UI Document asset (`.uxml`) and a Style Sheet (`.uss`).

2. Design your window layout inside the UI Builder. Ensure your root element has an appropriate layout (e.g., absolute positioning to overlay the screen, or centered alignment).

### Step B: Write the Window Controller Script

Create a new script for your specific window that inherits from UIToolkitWindowBase. Implement any custom data-binding or button click handlers inside the lifecycle hooks.

```C#
using Anarksoft.SharedCore.UI;
using UnityEngine.UIElements;

namespace MyGame.UI
{
    public class InventoryWindow : UIToolkitWindowBase
    {
        private Button closeButton;

        protected override void OnOpened()
        {
            base.OnOpened();
            
            // Query elements from rootElement (provided by the base class)
            closeButton = rootElement.Q<Button>("CloseButton");
            if (closeButton != null)
            {
                closeButton.clicked += Close;
            }

            // TODO: Populate inventory slot data here
        }

        protected override void OnClosed()
        {
            base.OnClosed();

            if (closeButton != null)
            {
                closeButton.clicked -= Close;
            }
        }
    }
}
```

### Step C: Setup the GameObject in Your Scene

   1. Create an empty GameObject in your UI scene (or UI prefab) and attach a UIDocument component.
   2. Assign your .uxml asset to the Visual Tree Asset field.
   3. Attach your custom window script (e.g., InventoryWindow) to the same GameObject.
   4. (Optional) Set the WindowID field in the inspector if you prefer string-based lookups.

## 3. Opening and Closing Windows

Because UIToolkitWindowBase automatically registers itself with the UIToolkitUIManager during OnEnable() and unregisters during OnDisable(), you can trigger windows from anywhere in your gameplay code using your service locator:

```C#

using Anarksoft.SharedCore.DI;
using Anarksoft.SharedCore.UI;
using UnityEngine;

namespace MyGame.Gameplay
{
    public class PlayerInteraction : MonoBehaviour
    {
        public void ToggleInventory()
        {
            // Option 1: Open/Close using type-safe resolution (Recommended)
            var uiManager = ServiceLocator.Container.Resolve<UIToolkitUIManager>();
            
            // Assuming your manager supports type-based opening, or fallback to your registered lookup:
            // uiManager.Open<InventoryWindow>();
        }
    }
}
```

## 4. Best Practices & Tips

- Zero GameObject Toggling: Never use `gameObject.SetActive(false)` to hide UI Toolkit windows. The base class automatically toggles `rootElement.style.display = DisplayStyle.None`, which keeps the hierarchy light and avoids unnecessary layout rebuild passes.
- Automatic Lifecycle Handling: Leaving the window GameObject active or spawning it dynamically via code handles registration automatically via `OnEnable()`. Just make sure your `UIToolkitUIManager` is initialized during boot via your `SharedCoreSystem`.

- `UIToolkitUIManager` Initialization: Ensure that your `UIToolkitUIManager` is properly initialized during the boot process via your `SharedCoreSystem` to avoid any runtime issues with window registration.