# UnityEssentials

Collection of essential scripts and tools suitable for any type of Unity Project.

## Features

### Editor

- **In-editor texture creation** - Create blank textures, gradients and checkerboard textures directly in the editor
- **Mesh builder** - Generate meshes via script
- **Extended transform inspector** - Show additional information about transform objects
- **New menu items** - Toggle asset refresh, apply transforms to children, run builds, and more
- **Script templates** - New script templates with automatic namespace generation
- **(Optional) Reorganized asset menu** - Gone are the days of having a cluttered create asset menu
- **New scene view tools**
	- Prefab placement tool
	- Distance measurement tool
	- Randmize transform tool
	- Convex mesh builder
	- Reflection probe bounds tool
- **Extension methods** - Extensions for easier editor GUI creation
- **(Optional) Disabled Playmode shortcuts** - Disable common edit commands during playmode
- **Property drawer utility** - Helper class for working with SerializedProperties

### Runtime

- **TODO** - Feature descriptions go here

## Installation

### Option 1: Unity Package Manager

> Make sure to always target a specific tag of the package to avoid breaking changes.
>
> The latest tags can be found [here](/tags).

Open the Package Manager window, click on "Add Package from Git URL ...", then enter the following:
```
https://github.com/d3tonat0r/unitypackage-essentials.git#0.9.2
```

### Option 2: Manually Editing packages.json

Add the following line to your project's `Packages/manifest.json`:

```json
"com.example.package": "https://github.com/d3tonat0r/unitypackage-essentials.git#0.9.2"
```

### Option 3: Manual Installation (not recommended)

You can also download this repository and extract the ZIP file anywhere inside your project's Assets folder.
