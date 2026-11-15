# ServiceLocator Tests

This directory contains edit mode unit tests for the `ServiceLocator` class in the com.anarks2.unity-common package.

## Test Files

- **ServiceLocatorTests.cs** - Comprehensive test suite for the ServiceLocator class covering:
  - Singleton pattern verification
  - Service registration and resolution
  - Exception handling for unregistered services
  - Service overwriting behavior
  - TryResolve safe resolution
  - Service unregistration
  - Multiple service types
  - Container reset functionality
  - Value type support

## Running Tests

### In Unity Editor
1. Open the project in Unity (version 6000.5.0 or compatible)
2. Go to **Window > General > Test Runner**
3. Click on the **EditMode** tab
4. The tests under **Anarksoft.SharedCore.Tests > ServiceLocatorTests** should be visible
5. Click **Run All** or select individual tests to run

### Via Command Line
You can run the tests using Unity's command-line testing tools:

```bash
"C:\Program Files\Unity\Hub\Editor\6000.5.0f1\Editor\Unity.exe" -projectPath "C:\Source\Unity\PackageTestProject" -testCategory editmode -runTests -batchmode -nographics
```

## Test Coverage

The test suite includes:

- **Singleton Pattern Tests**
  - Verifies the Container returns the same instance
  
- **Registration Tests**
  - Basic service registration
  - Overwriting existing services
  - Multiple service types registration

- **Resolution Tests**
  - Resolving registered services
  - Resolving unregistered services (exception handling)
  - Type casting verification

- **Safe Resolution Tests** (TryResolve)
  - Successfully resolving registered services
  - Safely handling unregistered services
  - Return value verification

- **Unregistration Tests**
  - Removing registered services
  - Handling unregistration of non-existent services

- **Reset Tests**
  - Clearing all services from the container
  - Verifying new instance after reset

- **Type Support Tests**
  - Value type support
  - Interface implementation support
  - Multiple unrelated types

## Assembly Definition

The tests are configured through `com.anarksoft.sharedcore.tests.asmdef` with:
- **Editor-only** execution (Unity edit mode only)
- **NUnit.Framework** dependency
- **Reference to Runtime assembly** (com.anarksoft.sharedcore)
- **UNITY_INCLUDE_TESTS** define constraint

## Notes

- Tests use NUnit framework as per Unity standards
- Each test has Setup/TearDown to ensure isolation (Container is reset between tests)
- Tests verify both happy path and error conditions
- The singleton pattern is reset after each test to prevent test contamination
