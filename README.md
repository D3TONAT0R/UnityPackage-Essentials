# UnityEssentials

Collection of essential scripts and tools suitable for any type of Unity Project.

## Features

#### New Property Attributes
- **ShowIf** - Only shows the property if a given condition is true
- **EnabledIf** - Makes the property editable only if a given condition is true
- **Prefix / Suffix** - Apply prefixes and/or suffixes to value fields
- **Button** - Invoke methods via a button press
- **CharRestriction** - Restrict characters in string
- **CreateAssetButton** - Creates a new asset and assigns it to the assigned property
- **CurveUsage** - Customize AnimationCurve range and color
- **EnumButtons** - Display enum values as horizontal buttons
- **Expand** - Force-expands a property to always show its children
- **HelpBox** - Shows a help box above the property
- **IntPopup** - Select a predetermined range of values via a popup
- **LayerPopup** - Select layers via a popup
- **TagPopup** - Select tags via a popup
- **MaterialPropertyHint**
- **MinMaxRange** - Display a float or int with a min / max slider
- **Monospace** - Draws a text field using a monospace font
- **Polymorphic** - Adds support for polymorphism (?)
- **Prefab** - Forces a GameObject property to only accept prefab assets
- **ProgressBar** - Draws an interactive progress bar
- **ReadOnly** - Shows a property as a read-only field
- **Required** - Shows an error message if no object is assigned or a component is missing
- **Scene** - Shows a scene popup for int (build index) or string (scene name)
- **Separator** - Draws a separator line above the property
- **ShowInInspector** - Apply to C# properties to make them visible with PropertyView structs
- **PropertyView** - Specifies which group of marked C# properties to show when categorized using `ShowInInspector`.
- **TexturePreview** - Draws a preview of the assigned texture or sprite below the property

#### **New Menu Items**
- **Recompile Scripts** - Forces recompilation of scripts
- **Run Build** - Lists and runs a built executable when placed in the "Builds" directory.

#### Transform Inspector Enhancements
- New extra properties such as world position, direction normals, and more
- Toolbar with controls to copy and paste specific properties between Transforms (same as right click -> "Copy/...")
  - Both can be hidden via the project settings
- Apply position / rotation / scale on a parent transform

#### New Component Context Menu Items
- **Cut Component** - "Copy Component" and "Remove Component" in one command
- **Separate Component** - Separates the selected component into an existing child, a new child object, or parent
- **Search Similar** - Searches for similar components in the hierarchy
- **MonoBehaviour: Replace Script** - Replace the selected MonoBehaviour with a new script
- **Mesh Filter: Export Mesh as Asset** - Extracts the current Mesh as a new asset in the project folder
- **Mesh Renderer: Instantiate Materials** - Creates instances of the renderer's materials to make them unique for this renderer.

#### New Script Templates
- New templates for script other than MonoBehaviours
  
#### Scene View Tools
- Prefab placement tool
- Distance measurement tool
- Randomize transform tool
- Convex mesh builder
- Reflection probe bounds tool

#### Extension Methods
- Basic math
- Colors
- Vectors (includes math and shader-like swizzling methods)
- Matrix4x4
- Transforms
- Textures
- Terrains
- Exceptions (add a custom message to an already thrown exception)

- **Extension methods** - Extensions for many common types, including editor GUI related ones
- **Property drawer utility** - Helper class for working with SerializedProperties
- **Repeating Timer** - A simple object useful for timing events at a specific interval or frame rate
- **Update Loop** - New event injection points on top of unity's own update loop and ability to run static methods in any update loop using attributes or event subscriptions
	- PreUpdate
	- PreLateUpdate
	- PostLateUpdate
	- PostFixedUpdate
- **Coroutine Tools** - Helper class for managing coroutines and delayed invocation of functions, including support for static coroutines
- **Extra Gizmos** - Various new gizmo shapes (circles, cylinders, arrows, texts and more)
- **Pooling System** - Reuse the same game objects instead of spawning and destroying them
- **Random Utilities** - Miscellaneous new random functions, including weighted distribution functions
- **Debug Utilities** - Helper functions to aid in debugging, including temporary gizmos

### Runtime

- **Property Attributes** - A plethora of new attributes for fields in the inspector, including, but not limited to:
	- Show if / Enabled if (conditionally show / enable fields)
	- Read only
	- Progress bar
	- Buttons to call a specific method
	- Popups for Layers, Tags or even integers
	- AnimationCurve parameters (set limits and color)
	- Monospace text fields
- **Collections** - Includes lists that support polymorphism and serializable dictionaries
- **New Data Types** - Various new data types that are commonly used:
	- Float range (min / max value)
	- Material property name
	- Nullables for many common value types
- **Mesh Generators** - Scripts to simplify procedural mesh generation
	- Convex mesh generator (using MIConvexHull)
	- Mesh builder (for triangle, line and voxel meshes)
	- Topology converter (convert triangle meshes into lines and/or point clouds)
- **Extension Methods** - Tons of new extension methods, available for:
	- Basic math
	- Colors
	- Vectors (includes math and shader-like swizzling methods)
	- Matrix4x4
	- Transforms
	- Textures
	- Terrains
	- Exceptions (add a custom message to an already thrown exception)
- **Repeating Timer** - A simple object useful for timing events at a specific interval or frame rate
- **Update Loop** - New event injection points on top of unity's own update loop and ability to run static methods in any update loop using attributes or event subscriptions
	- PreUpdate
	- PreLateUpdate
	- PostLateUpdate
	- PostFixedUpdate
- **Coroutine Tools** - Helper class for managing coroutines and delayed invocation of functions, including support for static coroutines
- **Extra Gizmos** - Various new gizmo shapes (circles, cylinders, arrows, texts and more)
- **Pooling System** - Reuse the same game objects instead of spawning and destroying them
- **Random Utilities** - Miscellaneous new random functions, including weighted distribution functions
- **Debug Utilities** - Helper functions to aid in debugging, including temporary gizmos

## Installation

> [!WARNING]
> It is highly recommended to target a specific release when installing to avoid breaking changes when unity reinstalls the package. This can be done by appending '#(tag-name)' at the end of the repo URL, such as '#1.0.0'.
> A list of currently available tags can be found [here](https://github.com/D3TONAT0R/UnityPackage-Essentials/tags).

### Option 1: Unity Package Manager

Open the Package Manager window, click on "Add Package from Git URL ...", then enter the following:
```
https://github.com/d3tonat0r/unitypackage-essentials.git#1.0.0-preview-2
```
OR to target the latest commit:
```
https://github.com/d3tonat0r/unitypackage-essentials.git
```

### Option 2: Manually Editing packages.json

Add the following line to your project's `Packages/manifest.json`:

```json
"com.github.d3tonat0r.essentials": "https://github.com/d3tonat0r/unitypackage-essentials.git"
```
