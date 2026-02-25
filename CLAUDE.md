# LiveUML - XrmToolBox Plugin

## Project Overview
XrmToolBox plugin that visualizes Dataverse entity metadata as live UML class diagrams. Users select entities, attributes, and relationships from a tree view, and the diagram updates in real-time.

## Tech Stack
- .NET Framework 4.8 (required by XrmToolBox)
- WinForms (PluginControlBase)
- XrmToolBoxPackage NuGet (SDK + Extensibility)
- GDI+ for UML rendering (System.Drawing)

## Architecture
- **Models/** - POCOs decoupled from SDK types (EntityMetadataModel, AttributeMetadataModel, RelationshipMetadataModel, DiagramLayout)
- **Services/** - IMetadataService/MetadataService (Dataverse queries), IDiagramService/DiagramService (layout building)
- **Rendering/** - UmlRenderer (GDI+ paint), LayoutEngine (grid positioning)
- **Extensions/** - MetadataExtensions (SDK-to-model mapping)

## Coding Conventions
- All comments and documentation in English
- Follow C# best practices and SOLID principles
- Decouple models from SDK types; only Services reference Microsoft.Xrm.Sdk.Metadata
- Use XrmToolBox patterns: ExecuteMethod() for connection checks, WorkAsync() for background work
- Use interfaces for services (IMetadataService, IDiagramService)

## Post-Build Deploy
After every successful build, manually copy the output DLL to the XrmToolBox Plugins folder:
```
cp LiveUML/bin/Debug/net48/LiveUML.dll "/mnt/c/Users/colam/AppData/Roaming/MscrmTools/XrmToolBox/Plugins/"
```

## Git Policy
- All commits and pushes must be authored exclusively under the repository owner's account
- No Co-Authored-By headers or any AI attribution in commits
- No mention of Claude, AI, or automated generation anywhere in the repository (code, comments, commit messages, or metadata)

## Release Management

### Branching
- **`main`** - production branch, always stable and releasable
- **`feature/*`** - feature branches for new functionality, merged into `main`

### Versioning (Semantic Versioning)
Format: `MAJOR.MINOR.PATCH` (e.g. `1.0.0`)
- **PATCH** (1.0.x): bug fixes, minor UI corrections
- **MINOR** (1.x.0): new features (new export types, layout algorithms, etc.)
- **MAJOR** (x.0.0): breaking changes, architectural rewrites

Version must be updated in **two places** (must match):
- `LiveUML.csproj`: `<Version>`, `<AssemblyVersion>`, `<FileVersion>`
- `LiveUML.nuspec`: `<version>`

`Version` is 3-part (e.g. `1.0.0`), `AssemblyVersion`/`FileVersion` are 4-part (e.g. `1.0.0.0`).

### Tags
- Format: `v{MAJOR}.{MINOR}.{PATCH}` (e.g. `v1.0.0`)
- Always use annotated tags: `git tag -a v1.0.0 -m "description"`

### Release Process
1. Update version in `LiveUML.csproj` (`Version` + `AssemblyVersion` + `FileVersion`)
2. Update version in `LiveUML.nuspec` (`<version>`)
3. Commit: `"Release v1.0.0: short description"`
4. Annotated tag: `git tag -a v1.0.0 -m "Release 1.0.0: description"`
5. Push: `git push origin master:main --tags`
6. CI/CD handles the rest automatically (build, pack, NuGet push, GitHub Release)

### CI/CD Pipeline
- **`.github/workflows/release.yml`** triggers on tag push (`v*`)
- Builds Release, packs `.nupkg`, pushes to nuget.org, creates GitHub Release
- NuGet API key stored as repo secret `NUGET_API_KEY`
- XrmToolBox Tool Library picks up updates from nuget.org automatically
