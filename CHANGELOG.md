# Changelog

## [1.2.0] - 2025-02-25

### Added
- Zoom in/out on the diagram panel with Ctrl+MouseWheel (25%–300%)
- Explicit horizontal and vertical scrollbars on the diagram panel
- Mouse wheel scrolls the diagram vertically without Ctrl
- Zoom level displayed in the status bar
- Zoom resets to 100% on Clear All

### Changed
- Replaced AutoScroll with dedicated HScrollBar/VScrollBar controls for better visibility and control
- Drag, resize, and click hit-testing now work correctly at any zoom level
- Scrollbar ranges scale with zoom factor

## [1.1.0] - 2025-02-25

### Added
- Resizable entity boxes: drag the bottom-right corner handle to resize width and height
- Resize grip indicator drawn on each entity box
- Cursor changes to diagonal resize arrow when hovering the resize handle
- Minimum size enforcement (120px width, 66px height)
- Manual sizes persist across diagram refreshes and attribute changes
- Clear All resets custom sizes along with positions

### Changed
- Increased PNG export margin from 20px to 40px for better spacing

## [1.0.0] - 2025-02-24

### Added
- Initial release
- Dataverse entity metadata visualization as UML class diagrams
- Entity selection with search/filter
- Attribute and relationship detail panels
- Drag-to-move entity boxes
- Auto-layout with BFS-based positioning
- Relationship lines with labels
- PNG export
