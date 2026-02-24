# LiveUML

<p align="center">
  <img src="logo.png" alt="LiveUML Logo" width="200" />
</p>

<p align="center">
  <strong>Visualize Dataverse entity metadata as live UML class diagrams</strong>
</p>

<p align="center">
  <a href="https://github.com/Tony0380/LiveUML/releases"><img src="https://img.shields.io/github/v/release/Tony0380/LiveUML" alt="GitHub Release" /></a>
  <a href="https://www.nuget.org/packages/LiveUML"><img src="https://img.shields.io/nuget/v/LiveUML" alt="NuGet" /></a>
</p>

---

## Overview

LiveUML is an [XrmToolBox](https://www.xrmtoolbox.com) plugin that lets you explore Dataverse entity metadata and generate UML class diagrams in real time. Select the entities you care about, pick which attributes and relationships to show, and get a clear visual representation of your data model.

## Features

- **Load entity metadata** from any connected Dataverse environment
- **Select entities, attributes, and relationships** to include in the diagram
- **Real-time UML rendering** — diagram updates live as you change selections
- **Smart auto-layout** — BFS-based grid positioning places related entities near each other
- **Drag-and-drop** — manually reposition entity boxes on the canvas
- **Search and filter** — quickly find entities by logical or display name
- **Export as PNG** — save the current diagram to an image file
- **Fully async** — all Dataverse calls run in the background, no UI freezing

## Installation

### From XrmToolBox Tool Library

1. Open XrmToolBox
2. Go to **Tool Library**
3. Search for **LiveUML**
4. Click **Install**

### Manual

1. Download `LiveUML.dll` from [GitHub Releases](https://github.com/Tony0380/LiveUML/releases)
2. Copy it to your XrmToolBox Plugins folder:
   ```
   %APPDATA%\MscrmTools\XrmToolBox\Plugins\
   ```
3. Restart XrmToolBox

## Usage

1. Open XrmToolBox and connect to a Dataverse environment
2. Launch **LiveUML** from the tool list
3. Click **Load Metadata** to retrieve all entities
4. Check the entities you want to visualize
5. Use the detail panel to select specific attributes and relationships
6. Drag entity boxes to adjust the layout
7. Click **Export PNG** to save the diagram

## Architecture

```
LiveUML/
├── Models/        # POCOs decoupled from SDK types
├── Services/      # Dataverse metadata queries (IMetadataService)
├── Rendering/     # GDI+ renderer + grid layout engine
└── Extensions/    # SDK-to-model mapping
```

- **Models** define entities, attributes, relationships, and diagram layout as plain C# objects
- **Services** handle all communication with the Dataverse SDK
- **Rendering** takes care of GDI+ drawing and automatic entity positioning
- **Extensions** map SDK metadata types to the internal model

## Tech Stack

- .NET Framework 4.8
- WinForms (XrmToolBox `PluginControlBase`)
- GDI+ (`System.Drawing`)
- XrmToolBoxPackage SDK

## Building from Source

**Prerequisites:** .NET SDK 8.0+

```bash
git clone https://github.com/Tony0380/LiveUML.git
cd LiveUML
dotnet build LiveUML.sln -c Release
```

The output DLL will be at `LiveUML/bin/Release/net48/LiveUML.dll`.

## Author

**Antonio Colamartino** — [GitHub](https://github.com/Tony0380)
