---

# Anarksoft Unity Common (`com.anarks2.unity-common`)

A modular shared codebase containing common gameplay structures, utilities, and Unity Editor tools designed for multi-project solo development.

---

## Installation Guide

Depending on your workflow for a specific game project, you can install or link this package in one of two ways.

### Option A: Git Submodule + Local Package (Recommended for Active Development)

Use this approach if you want to make live edits to the shared code directly inside your active game project workspace and easily push updates back to the shared repository.

1. Open a terminal in the **root directory of your main Unity game project** (where your `.git` folder is located).
2. Add this repository as a submodule inside a root-level `LocalPackages/` directory:
```bash
git submodule add https://github.com/Togusa09/unity-common.git LocalPackages/com.anarks2.unity-common
git submodule update --init --recursive

```


3. Open your game project's `Packages/manifest.json` file in a text editor.
4. Add the local package reference under the `dependencies` block:
```json
{
  "dependencies": {
    "com.anarks2.unity-common": "file:../LocalPackages/com.anarks2.unity-common"
  }
}

```


5. Return to Unity. The package will automatically load under the **"In Project"** section of the Package Manager.

> *Note on cloning:* When cloning your game project onto a new machine, remember to initialize submodules using `git submodule update --init --recursive`.

---

### Option B: Install via Git URL (For Consumer Projects / No Modifications)

Use this approach if you are building a game project that only needs to *consume* the shared core code as a read-only dependency without altering its source code.

1. Open your Unity game project.
2. Navigate to **Window > Package Manager**.
3. Click the **`+`** icon in the top-left corner of the window.
4. Select **"Add package from git url..."**.
5. Paste your GitHub repository URL (`[https://github.com/Togusa09/unity-common.git](https://github.com/Togusa09/unity-common.git)`) and click **Add**.

> ⚠️ **Important requirement for Git URL installs:** Because Unity treats packages pulled via Git URLs as immutable/read-only system caches, all asset `.meta` files **must** be fully tracked and committed in the remote repository. If a `.meta` file is missing, Unity will throw import errors and ignore the assets.

---

## Project Structure

* **`Runtime/`**: Gameplay logic, core systems, and components (Assembly: `com.anarks2.unity-common`)
* **`Editor/`**: Custom editor windows and tooling (Assembly: `com.anarks2.unity-common.editor`)
* **`Tests/`**: Automated EditMode and PlayMode NUnit test suites (Assembly: `com.anarks2.unity-common.tests`)