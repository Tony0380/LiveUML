using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using LiveUML.Models;
using LiveUML.Services;
using LiveUML.Rendering;
using XrmToolBox.Extensibility;

namespace LiveUML
{
    public partial class LiveUMLPluginControl : PluginControlBase
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);
        private const int EM_SETCUEBANNER = 0x1501;

        private IMetadataService _metadataService;
        private readonly IDiagramService _diagramService = new DiagramService();
        private readonly UmlRenderer _renderer = new UmlRenderer();

        private List<EntityMetadataModel> _allEntities = new List<EntityMetadataModel>();
        private List<EntityMetadataModel> _filteredEntities = new List<EntityMetadataModel>();
        private EntityMetadataModel _focusedEntity;
        private DiagramLayout _currentLayout;
        private Timer _debounceTimer;
        private bool _suppressCheckEvents;

        // Drag state
        private readonly Dictionary<string, Point> _manualPositions = new Dictionary<string, Point>();
        private EntityBox _dragBox;
        private Point _dragOffset;
        private Point _dragStartScreen;
        private bool _isDragging;

        // Detail load queue
        private readonly Queue<EntityMetadataModel> _detailLoadQueue = new Queue<EntityMetadataModel>();
        private bool _isLoadingDetail;

        public LiveUMLPluginControl()
        {
            InitializeComponent();
            _debounceTimer = new Timer { Interval = 150 };
            _debounceTimer.Tick += DebounceTimer_Tick;
        }

        private void LiveUMLPluginControl_Load(object sender, EventArgs e)
        {
            SendMessage(txtSearch.Handle, EM_SETCUEBANNER, 0, "Filter entities...");
        }

        #region Load Metadata

        private void BtnLoadMetadata_Click(object sender, EventArgs e)
        {
            ExecuteMethod(LoadAllEntities);
        }

        private void LoadAllEntities()
        {
            _metadataService = new MetadataService(Service);

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading entity metadata...",
                Work = (worker, args) =>
                {
                    args.Result = _metadataService.LoadAllEntitySummaries();
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    _allEntities = (List<EntityMetadataModel>)args.Result;
                    _filteredEntities = new List<EntityMetadataModel>(_allEntities);
                    _manualPositions.Clear();
                    _currentLayout = null;
                    _focusedEntity = null;
                    txtSearch.Text = string.Empty;
                    PopulateEntityList();
                    ClearDetailPanels();
                    diagramPanel.Invalidate();
                    lblStatus.Text = _allEntities.Count + " entities loaded";
                }
            });
        }

        #endregion

        #region Clear All / Export PNG

        private void BtnClearAll_Click(object sender, EventArgs e)
        {
            foreach (var entity in _allEntities)
            {
                entity.IsSelected = false;
                foreach (var attr in entity.Attributes)
                    attr.IsSelected = false;
            }

            _manualPositions.Clear();
            _currentLayout = null;
            _focusedEntity = null;
            PopulateEntityList();
            ClearDetailPanels();
            diagramPanel.AutoScrollMinSize = Size.Empty;
            diagramPanel.Invalidate();
            lblStatus.Text = _allEntities.Count > 0 ? _allEntities.Count + " entities loaded" : "";
        }

        private void BtnExportPng_Click(object sender, EventArgs e)
        {
            // Force a fresh layout build so export is always up-to-date
            _debounceTimer.Stop();
            RefreshDiagram();

            if (_currentLayout == null || _currentLayout.EntityBoxes.Count == 0)
            {
                MessageBox.Show("No diagram to export. Select some entities first.",
                    "Export PNG", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var dlg = new SaveFileDialog())
            {
                dlg.Filter = "PNG Image|*.png";
                dlg.DefaultExt = "png";
                dlg.FileName = "LiveUML_Diagram";

                if (dlg.ShowDialog() != DialogResult.OK)
                    return;

                // Compute actual bounding box from entity boxes
                int minX = int.MaxValue, minY = int.MaxValue;
                int maxX = 0, maxY = 0;
                foreach (var box in _currentLayout.EntityBoxes)
                {
                    if (box.Bounds.Left < minX) minX = box.Bounds.Left;
                    if (box.Bounds.Top < minY) minY = box.Bounds.Top;
                    if (box.Bounds.Right > maxX) maxX = box.Bounds.Right;
                    if (box.Bounds.Bottom > maxY) maxY = box.Bounds.Bottom;
                }

                int margin = 20;
                int width = maxX - minX + margin * 2;
                int height = maxY - minY + margin * 2;

                using (var bitmap = new Bitmap(Math.Max(1, width), Math.Max(1, height)))
                {
                    using (var g = Graphics.FromImage(bitmap))
                    {
                        g.Clear(Color.White);
                        // Translate so that the top-left box starts at the margin
                        g.TranslateTransform(-minX + margin, -minY + margin);
                        _renderer.Render(g, _currentLayout);
                    }
                    bitmap.Save(dlg.FileName, ImageFormat.Png);
                }

                lblStatus.Text = "Diagram exported to PNG";
            }
        }

        #endregion

        #region Entity List

        private void PopulateEntityList()
        {
            _suppressCheckEvents = true;
            lstEntities.Items.Clear();

            foreach (var entity in _filteredEntities)
            {
                lstEntities.Items.Add(
                    new ListEntry(entity, entity.DisplayName + " (" + entity.LogicalName + ")"),
                    entity.IsSelected);
            }

            _suppressCheckEvents = false;
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            var filter = txtSearch.Text.Trim().ToLowerInvariant();

            if (string.IsNullOrEmpty(filter))
                _filteredEntities = new List<EntityMetadataModel>(_allEntities);
            else
                _filteredEntities = _allEntities
                    .Where(ent => ent.LogicalName.ToLowerInvariant().Contains(filter)
                              || ent.DisplayName.ToLowerInvariant().Contains(filter))
                    .ToList();

            PopulateEntityList();
        }

        private void LstEntities_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_suppressCheckEvents) return;

            var entity = GetModel<EntityMetadataModel>(lstEntities, e.Index);
            bool willBeChecked = (e.NewValue == CheckState.Checked);
            entity.IsSelected = willBeChecked;

            if (willBeChecked)
                QueueDetailLoad(entity);

            StartDebouncedRefresh();
        }

        private void LstEntities_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstEntities.SelectedIndex < 0) return;

            var entity = GetModel<EntityMetadataModel>(lstEntities, lstEntities.SelectedIndex);
            FocusEntity(entity);
        }

        private void CheckEntityInList(string logicalName)
        {
            var entity = _allEntities.FirstOrDefault(ent => ent.LogicalName == logicalName);
            if (entity == null) return;

            entity.IsSelected = true;
            QueueDetailLoad(entity);

            SyncEntityCheckbox(entity, true);
        }

        private void UncheckEntityInList(string logicalName)
        {
            var entity = _allEntities.FirstOrDefault(ent => ent.LogicalName == logicalName);
            if (entity == null) return;

            entity.IsSelected = false;

            SyncEntityCheckbox(entity, false);
        }

        private void SyncEntityCheckbox(EntityMetadataModel entity, bool isChecked)
        {
            // If entity isn't visible due to active search filter, clear the filter
            bool found = false;
            for (int i = 0; i < lstEntities.Items.Count; i++)
            {
                if (GetModel<EntityMetadataModel>(lstEntities, i) == entity)
                {
                    found = true;
                    break;
                }
            }

            if (!found && isChecked)
            {
                txtSearch.Text = string.Empty;
                // TxtSearch_TextChanged rebuilds the list; entity now visible with correct state
            }

            _suppressCheckEvents = true;
            for (int i = 0; i < lstEntities.Items.Count; i++)
            {
                if (GetModel<EntityMetadataModel>(lstEntities, i) == entity)
                {
                    lstEntities.SetItemChecked(i, isChecked);
                    if (isChecked)
                        lstEntities.TopIndex = Math.Max(0, i - 3);
                    break;
                }
            }
            _suppressCheckEvents = false;
        }

        #endregion

        #region Detail Panels (Attributes + Relationships)

        private void FocusEntity(EntityMetadataModel entity)
        {
            if (entity == _focusedEntity) return;
            _focusedEntity = entity;

            if (!entity.IsDetailLoaded)
            {
                lblAttributes.Text = "Attributes (loading...)";
                lblRelationships.Text = "Relationships (loading...)";
                lstAttributes.Items.Clear();
                lstRelationships.Items.Clear();

                QueueDetailLoad(entity);
                return;
            }

            PopulateDetailPanels(entity);
        }

        private void ClearDetailPanels()
        {
            _focusedEntity = null;
            lblAttributes.Text = "Attributes";
            lblRelationships.Text = "Relationships";
            lstAttributes.Items.Clear();
            lstRelationships.Items.Clear();
        }

        private void PopulateDetailPanels(EntityMetadataModel entity)
        {
            _suppressCheckEvents = true;

            lblAttributes.Text = "Attributes - " + entity.DisplayName;
            lstAttributes.Items.Clear();
            foreach (var attr in entity.Attributes)
            {
                string prefix = attr.IsPrimaryId ? "[PK] " : attr.IsPrimaryName ? "[Name] " : "";
                lstAttributes.Items.Add(
                    new ListEntry(attr, prefix + attr.DisplayName + " (" + attr.LogicalName + ") : " + attr.DataType),
                    attr.IsSelected);
            }

            lblRelationships.Text = "Relationships - " + entity.DisplayName;
            lstRelationships.Items.Clear();
            foreach (var group in entity.Relationships.GroupBy(r => r.Type).OrderBy(g => g.Key))
            {
                foreach (var rel in group.OrderBy(r => r.SchemaName))
                {
                    string target = GetRelationshipTarget(rel, entity.LogicalName);
                    var targetEntity = _allEntities.FirstOrDefault(ent => ent.LogicalName == target);
                    string targetDisplay = targetEntity != null ? targetEntity.DisplayName : target;
                    bool inDiagram = targetEntity != null && targetEntity.IsSelected;

                    string typeTag;
                    switch (rel.Type)
                    {
                        case RelationshipType.OneToMany: typeTag = "1:N"; break;
                        case RelationshipType.ManyToOne: typeTag = "N:1"; break;
                        case RelationshipType.ManyToMany: typeTag = "N:N"; break;
                        default: typeTag = "?"; break;
                    }

                    lstRelationships.Items.Add(
                        new ListEntry(rel, "[" + typeTag + "] " + targetDisplay + " (" + target + ")"),
                        inDiagram);
                }
            }

            _suppressCheckEvents = false;
        }

        private void LstAttributes_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_suppressCheckEvents) return;

            var attr = GetModel<AttributeMetadataModel>(lstAttributes, e.Index);
            attr.IsSelected = (e.NewValue == CheckState.Checked);
            StartDebouncedRefresh();
        }

        private void LstRelationships_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_suppressCheckEvents || _focusedEntity == null) return;

            var rel = GetModel<RelationshipMetadataModel>(lstRelationships, e.Index);
            string target = GetRelationshipTarget(rel, _focusedEntity.LogicalName);
            bool willBeChecked = (e.NewValue == CheckState.Checked);

            if (willBeChecked)
                CheckEntityInList(target);
            else
                UncheckEntityInList(target);

            StartDebouncedRefresh();
        }

        private static string GetRelationshipTarget(RelationshipMetadataModel rel, string currentEntityName)
        {
            return rel.ReferencedEntity == currentEntityName
                ? rel.ReferencingEntity
                : rel.ReferencedEntity;
        }

        #endregion

        #region Detail Load Queue

        private void QueueDetailLoad(EntityMetadataModel entity)
        {
            if (entity.IsDetailLoaded) return;
            if (_detailLoadQueue.Contains(entity)) return;
            _detailLoadQueue.Enqueue(entity);
            ProcessDetailLoadQueue();
        }

        private void ProcessDetailLoadQueue()
        {
            if (_isLoadingDetail || _detailLoadQueue.Count == 0) return;
            if (_metadataService == null) return;

            var entity = _detailLoadQueue.Dequeue();
            if (entity.IsDetailLoaded)
            {
                // If this was the focused entity waiting for load, populate now
                if (_focusedEntity == entity)
                    PopulateDetailPanels(entity);
                ProcessDetailLoadQueue();
                return;
            }

            _isLoadingDetail = true;
            lblStatus.Text = "Loading " + entity.DisplayName + "...";

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading " + entity.DisplayName + "...",
                Work = (w, a) => _metadataService.LoadEntityDetail(entity),
                PostWorkCallBack = (a) =>
                {
                    _isLoadingDetail = false;

                    if (a.Error != null)
                    {
                        lblStatus.Text = "Error loading " + entity.DisplayName;
                    }
                    else
                    {
                        // If focused entity just loaded, populate panels
                        if (_focusedEntity == entity)
                            PopulateDetailPanels(entity);

                        int remaining = _detailLoadQueue.Count;
                        lblStatus.Text = remaining > 0
                            ? "Loading... (" + remaining + " remaining)"
                            : "Ready";
                        StartDebouncedRefresh();
                    }

                    ProcessDetailLoadQueue();
                }
            });
        }

        #endregion

        #region Diagram

        private void StartDebouncedRefresh()
        {
            _debounceTimer.Stop();
            _debounceTimer.Start();
        }

        private void DebounceTimer_Tick(object sender, EventArgs e)
        {
            _debounceTimer.Stop();
            RefreshDiagram();
        }

        private void RefreshDiagram()
        {
            _currentLayout = _diagramService.BuildLayout(_allEntities, _manualPositions);

            diagramPanel.AutoScrollMinSize = (_currentLayout != null && _currentLayout.TotalSize != Size.Empty)
                ? _currentLayout.TotalSize
                : Size.Empty;

            diagramPanel.Invalidate();
            RefreshRelationshipCheckStates();
        }

        private void RefreshRelationshipCheckStates()
        {
            if (_focusedEntity == null) return;

            _suppressCheckEvents = true;
            for (int i = 0; i < lstRelationships.Items.Count; i++)
            {
                var rel = GetModel<RelationshipMetadataModel>(lstRelationships, i);
                if (rel == null) continue;
                string target = GetRelationshipTarget(rel, _focusedEntity.LogicalName);
                var targetEntity = _allEntities.FirstOrDefault(ent => ent.LogicalName == target);
                bool inDiagram = targetEntity != null && targetEntity.IsSelected;
                lstRelationships.SetItemChecked(i, inDiagram);
            }
            _suppressCheckEvents = false;
        }

        private void DiagramPanel_Paint(object sender, PaintEventArgs e)
        {
            if (_currentLayout == null || _currentLayout.EntityBoxes.Count == 0)
                return;

            e.Graphics.TranslateTransform(diagramPanel.AutoScrollPosition.X, diagramPanel.AutoScrollPosition.Y);
            _renderer.Render(e.Graphics, _currentLayout);
        }

        #endregion

        #region Drag & Click on Box

        private Point ScreenToCanvas(Point screenPoint)
        {
            return new Point(
                screenPoint.X - diagramPanel.AutoScrollPosition.X,
                screenPoint.Y - diagramPanel.AutoScrollPosition.Y);
        }

        private EntityBox HitTestBox(Point canvasPoint)
        {
            if (_currentLayout == null) return null;

            for (int i = _currentLayout.EntityBoxes.Count - 1; i >= 0; i--)
            {
                if (_currentLayout.EntityBoxes[i].Bounds.Contains(canvasPoint))
                    return _currentLayout.EntityBoxes[i];
            }
            return null;
        }

        private void DiagramPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || _currentLayout == null) return;

            var canvasPoint = ScreenToCanvas(e.Location);
            var box = HitTestBox(canvasPoint);
            if (box == null) return;

            _dragBox = box;
            _dragOffset = new Point(canvasPoint.X - box.Bounds.X, canvasPoint.Y - box.Bounds.Y);
            _dragStartScreen = e.Location;
            _isDragging = false;
        }

        private void DiagramPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragBox != null && !_isDragging)
            {
                int dx = e.X - _dragStartScreen.X;
                int dy = e.Y - _dragStartScreen.Y;
                if (Math.Abs(dx) > 4 || Math.Abs(dy) > 4)
                    _isDragging = true;
            }

            if (_isDragging && _dragBox != null)
            {
                var canvasPoint = ScreenToCanvas(e.Location);
                int newX = Math.Max(0, canvasPoint.X - _dragOffset.X);
                int newY = Math.Max(0, canvasPoint.Y - _dragOffset.Y);

                _dragBox.Bounds = new Rectangle(newX, newY, _dragBox.Bounds.Width, _dragBox.Bounds.Height);
                Rendering.LayoutEngine.RecomputeLines(_currentLayout);
                diagramPanel.Invalidate();
                diagramPanel.Cursor = Cursors.SizeAll;
                return;
            }

            if (_currentLayout != null && _dragBox == null)
            {
                var canvasPoint = ScreenToCanvas(e.Location);
                diagramPanel.Cursor = HitTestBox(canvasPoint) != null ? Cursors.Hand : Cursors.Default;
            }
        }

        private void DiagramPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (_dragBox == null) return;

            if (_isDragging)
            {
                _manualPositions[_dragBox.EntityLogicalName] = _dragBox.Bounds.Location;
                UpdateScrollSize();
                _isDragging = false;
                _dragBox = null;
                diagramPanel.Cursor = Cursors.Default;
                diagramPanel.Invalidate();
            }
            else
            {
                // Click on box → focus entity in detail panels
                var clickedBox = _dragBox;
                _dragBox = null;
                diagramPanel.Cursor = Cursors.Default;

                var entity = _allEntities.FirstOrDefault(ent => ent.LogicalName == clickedBox.EntityLogicalName);
                if (entity != null)
                    FocusEntity(entity);
            }
        }

        private void UpdateScrollSize()
        {
            if (_currentLayout == null) return;
            int maxRight = 0, maxBottom = 0;
            foreach (var box in _currentLayout.EntityBoxes)
            {
                if (box.Bounds.Right > maxRight) maxRight = box.Bounds.Right;
                if (box.Bounds.Bottom > maxBottom) maxBottom = box.Bounds.Bottom;
            }
            diagramPanel.AutoScrollMinSize = new Size(maxRight + 30, maxBottom + 30);
        }

        #endregion

        #region Author Link

        private void LnkAuthor_Click(object sender, EventArgs e)
        {
            Process.Start("https://antoniocolamartino.it");
        }

        #endregion

        #region Helpers

        private static T GetModel<T>(CheckedListBox list, int index) where T : class
        {
            return ((ListEntry)list.Items[index]).Model as T;
        }

        private class ListEntry
        {
            public object Model { get; }
            private readonly string _text;

            public ListEntry(object model, string text)
            {
                Model = model;
                _text = text;
            }

            public override string ToString() => _text;
        }

        #endregion
    }
}
