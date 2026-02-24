namespace LiveUML
{
    partial class LiveUMLPluginControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _debounceTimer?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnLoadMetadata = new System.Windows.Forms.ToolStripButton();
            this.btnClearAll = new System.Windows.Forms.ToolStripButton();
            this.btnExportPng = new System.Windows.Forms.ToolStripButton();
            this.lblStatus = new System.Windows.Forms.ToolStripLabel();
            this.lnkAuthor = new System.Windows.Forms.ToolStripLabel();
            this.splitLeft = new System.Windows.Forms.SplitContainer();
            this.lstEntities = new System.Windows.Forms.CheckedListBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblEntities = new System.Windows.Forms.Label();
            this.splitRight = new System.Windows.Forms.SplitContainer();
            this.diagramPanel = new LiveUML.DoubleBufferedPanel();
            this.tableDetail = new System.Windows.Forms.TableLayoutPanel();
            this.pnlAttributes = new System.Windows.Forms.Panel();
            this.lstAttributes = new System.Windows.Forms.CheckedListBox();
            this.lblAttributes = new System.Windows.Forms.Label();
            this.pnlRelationships = new System.Windows.Forms.Panel();
            this.lstRelationships = new System.Windows.Forms.CheckedListBox();
            this.lblRelationships = new System.Windows.Forms.Label();
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitLeft)).BeginInit();
            this.splitLeft.Panel1.SuspendLayout();
            this.splitLeft.Panel2.SuspendLayout();
            this.splitLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitRight)).BeginInit();
            this.splitRight.Panel1.SuspendLayout();
            this.splitRight.Panel2.SuspendLayout();
            this.splitRight.SuspendLayout();
            this.tableDetail.SuspendLayout();
            this.pnlAttributes.SuspendLayout();
            this.pnlRelationships.SuspendLayout();
            this.SuspendLayout();
            //
            // toolStrip
            //
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.btnLoadMetadata,
                this.btnClearAll,
                this.btnExportPng,
                new System.Windows.Forms.ToolStripSeparator(),
                this.lblStatus,
                new System.Windows.Forms.ToolStripSeparator(),
                this.lnkAuthor});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(950, 25);
            this.toolStrip.TabIndex = 0;
            //
            // btnLoadMetadata
            //
            this.btnLoadMetadata.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnLoadMetadata.Name = "btnLoadMetadata";
            this.btnLoadMetadata.Size = new System.Drawing.Size(88, 22);
            this.btnLoadMetadata.Text = "Load Metadata";
            this.btnLoadMetadata.Click += new System.EventHandler(this.BtnLoadMetadata_Click);
            //
            // btnClearAll
            //
            this.btnClearAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Size = new System.Drawing.Size(56, 22);
            this.btnClearAll.Text = "Clear All";
            this.btnClearAll.Click += new System.EventHandler(this.BtnClearAll_Click);
            //
            // btnExportPng
            //
            this.btnExportPng.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnExportPng.Name = "btnExportPng";
            this.btnExportPng.Size = new System.Drawing.Size(74, 22);
            this.btnExportPng.Text = "Export PNG";
            this.btnExportPng.Click += new System.EventHandler(this.BtnExportPng_Click);
            //
            // lblStatus
            //
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 22);
            //
            // lnkAuthor
            //
            this.lnkAuthor.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lnkAuthor.IsLink = true;
            this.lnkAuthor.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lnkAuthor.Name = "lnkAuthor";
            this.lnkAuthor.Size = new System.Drawing.Size(150, 22);
            this.lnkAuthor.Text = "By Antonio Colamartino";
            this.lnkAuthor.Click += new System.EventHandler(this.LnkAuthor_Click);
            //
            // splitLeft (entities | rest)
            //
            this.splitLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitLeft.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitLeft.Location = new System.Drawing.Point(0, 25);
            this.splitLeft.Name = "splitLeft";
            this.splitLeft.Size = new System.Drawing.Size(950, 575);
            this.splitLeft.SplitterDistance = 250;
            this.splitLeft.TabIndex = 1;
            //
            // splitLeft.Panel1 - entities
            //
            this.splitLeft.Panel1.Controls.Add(this.lstEntities);
            this.splitLeft.Panel1.Controls.Add(this.txtSearch);
            this.splitLeft.Panel1.Controls.Add(this.lblEntities);
            //
            // splitLeft.Panel2 - diagram + detail
            //
            this.splitLeft.Panel2.Controls.Add(this.splitRight);
            //
            // lblEntities
            //
            this.lblEntities.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblEntities.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblEntities.Location = new System.Drawing.Point(0, 0);
            this.lblEntities.Name = "lblEntities";
            this.lblEntities.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.lblEntities.Size = new System.Drawing.Size(250, 22);
            this.lblEntities.TabIndex = 0;
            this.lblEntities.Text = "Entities";
            //
            // txtSearch
            //
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtSearch.Location = new System.Drawing.Point(0, 22);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(250, 20);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextChanged += new System.EventHandler(this.TxtSearch_TextChanged);
            //
            // lstEntities
            //
            this.lstEntities.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstEntities.FormattingEnabled = true;
            this.lstEntities.IntegralHeight = false;
            this.lstEntities.Location = new System.Drawing.Point(0, 42);
            this.lstEntities.Name = "lstEntities";
            this.lstEntities.Size = new System.Drawing.Size(250, 533);
            this.lstEntities.TabIndex = 2;
            this.lstEntities.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.LstEntities_ItemCheck);
            this.lstEntities.SelectedIndexChanged += new System.EventHandler(this.LstEntities_SelectedIndexChanged);
            //
            // splitRight (diagram | detail panels)
            //
            this.splitRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitRight.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitRight.Location = new System.Drawing.Point(0, 0);
            this.splitRight.Name = "splitRight";
            this.splitRight.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitRight.Size = new System.Drawing.Size(696, 575);
            this.splitRight.SplitterDistance = 370;
            this.splitRight.Panel2MinSize = 140;
            this.splitRight.TabIndex = 0;
            //
            // splitRight.Panel1 - diagram
            //
            this.splitRight.Panel1.Controls.Add(this.diagramPanel);
            //
            // splitRight.Panel2 - detail
            //
            this.splitRight.Panel2.Controls.Add(this.tableDetail);
            //
            // diagramPanel
            //
            this.diagramPanel.AutoScroll = true;
            this.diagramPanel.BackColor = System.Drawing.Color.White;
            this.diagramPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.diagramPanel.Location = new System.Drawing.Point(0, 0);
            this.diagramPanel.Name = "diagramPanel";
            this.diagramPanel.Size = new System.Drawing.Size(696, 370);
            this.diagramPanel.TabIndex = 0;
            this.diagramPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.DiagramPanel_Paint);
            this.diagramPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DiagramPanel_MouseDown);
            this.diagramPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DiagramPanel_MouseMove);
            this.diagramPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DiagramPanel_MouseUp);
            //
            // tableDetail
            //
            this.tableDetail.ColumnCount = 2;
            this.tableDetail.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableDetail.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableDetail.Controls.Add(this.pnlAttributes, 0, 0);
            this.tableDetail.Controls.Add(this.pnlRelationships, 1, 0);
            this.tableDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableDetail.Location = new System.Drawing.Point(0, 0);
            this.tableDetail.Name = "tableDetail";
            this.tableDetail.RowCount = 1;
            this.tableDetail.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableDetail.Size = new System.Drawing.Size(696, 201);
            this.tableDetail.TabIndex = 0;
            //
            // pnlAttributes
            //
            this.pnlAttributes.Controls.Add(this.lstAttributes);
            this.pnlAttributes.Controls.Add(this.lblAttributes);
            this.pnlAttributes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAttributes.Location = new System.Drawing.Point(3, 3);
            this.pnlAttributes.Name = "pnlAttributes";
            this.pnlAttributes.Size = new System.Drawing.Size(342, 195);
            this.pnlAttributes.TabIndex = 0;
            //
            // lblAttributes
            //
            this.lblAttributes.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblAttributes.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblAttributes.Location = new System.Drawing.Point(0, 0);
            this.lblAttributes.Name = "lblAttributes";
            this.lblAttributes.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.lblAttributes.Size = new System.Drawing.Size(342, 22);
            this.lblAttributes.TabIndex = 0;
            this.lblAttributes.Text = "Attributes";
            //
            // lstAttributes
            //
            this.lstAttributes.CheckOnClick = true;
            this.lstAttributes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstAttributes.FormattingEnabled = true;
            this.lstAttributes.IntegralHeight = false;
            this.lstAttributes.Location = new System.Drawing.Point(0, 22);
            this.lstAttributes.Name = "lstAttributes";
            this.lstAttributes.Size = new System.Drawing.Size(342, 173);
            this.lstAttributes.TabIndex = 1;
            this.lstAttributes.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.LstAttributes_ItemCheck);
            //
            // pnlRelationships
            //
            this.pnlRelationships.Controls.Add(this.lstRelationships);
            this.pnlRelationships.Controls.Add(this.lblRelationships);
            this.pnlRelationships.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRelationships.Location = new System.Drawing.Point(351, 3);
            this.pnlRelationships.Name = "pnlRelationships";
            this.pnlRelationships.Size = new System.Drawing.Size(342, 195);
            this.pnlRelationships.TabIndex = 1;
            //
            // lblRelationships
            //
            this.lblRelationships.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblRelationships.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblRelationships.Location = new System.Drawing.Point(0, 0);
            this.lblRelationships.Name = "lblRelationships";
            this.lblRelationships.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.lblRelationships.Size = new System.Drawing.Size(342, 22);
            this.lblRelationships.TabIndex = 0;
            this.lblRelationships.Text = "Relationships";
            //
            // lstRelationships
            //
            this.lstRelationships.CheckOnClick = true;
            this.lstRelationships.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstRelationships.FormattingEnabled = true;
            this.lstRelationships.IntegralHeight = false;
            this.lstRelationships.Location = new System.Drawing.Point(0, 22);
            this.lstRelationships.Name = "lstRelationships";
            this.lstRelationships.Size = new System.Drawing.Size(342, 173);
            this.lstRelationships.TabIndex = 1;
            this.lstRelationships.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.LstRelationships_ItemCheck);
            //
            // LiveUMLPluginControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitLeft);
            this.Controls.Add(this.toolStrip);
            this.Name = "LiveUMLPluginControl";
            this.Size = new System.Drawing.Size(950, 600);
            this.Load += new System.EventHandler(this.LiveUMLPluginControl_Load);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.splitLeft.Panel1.ResumeLayout(false);
            this.splitLeft.Panel1.PerformLayout();
            this.splitLeft.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitLeft)).EndInit();
            this.splitLeft.ResumeLayout(false);
            this.splitRight.Panel1.ResumeLayout(false);
            this.splitRight.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitRight)).EndInit();
            this.splitRight.ResumeLayout(false);
            this.tableDetail.ResumeLayout(false);
            this.pnlAttributes.ResumeLayout(false);
            this.pnlRelationships.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnLoadMetadata;
        private System.Windows.Forms.ToolStripButton btnClearAll;
        private System.Windows.Forms.ToolStripButton btnExportPng;
        private System.Windows.Forms.ToolStripLabel lblStatus;
        private System.Windows.Forms.ToolStripLabel lnkAuthor;
        private System.Windows.Forms.SplitContainer splitLeft;
        private System.Windows.Forms.Label lblEntities;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.CheckedListBox lstEntities;
        private System.Windows.Forms.SplitContainer splitRight;
        private DoubleBufferedPanel diagramPanel;
        private System.Windows.Forms.TableLayoutPanel tableDetail;
        private System.Windows.Forms.Panel pnlAttributes;
        private System.Windows.Forms.Label lblAttributes;
        private System.Windows.Forms.CheckedListBox lstAttributes;
        private System.Windows.Forms.Panel pnlRelationships;
        private System.Windows.Forms.Label lblRelationships;
        private System.Windows.Forms.CheckedListBox lstRelationships;
    }

    public class DoubleBufferedPanel : System.Windows.Forms.Panel
    {
        public DoubleBufferedPanel()
        {
            this.DoubleBuffered = true;
            SetStyle(System.Windows.Forms.ControlStyles.AllPaintingInWmPaint
                | System.Windows.Forms.ControlStyles.UserPaint
                | System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer, true);
        }
    }
}
