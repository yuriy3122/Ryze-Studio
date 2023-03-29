using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace RyzeEditor
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.inspectorPanel = new System.Windows.Forms.Panel();
            this.Inspector = new RyzeEditor.Controls.InspectorControl();
            this.ObjectHierarchyControl = new RyzeEditor.Controls.ObjectHierarchyControl();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.tsmiWorldMap = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.compileStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.tbUndo = new System.Windows.Forms.ToolStripButton();
            this.tbRedo = new System.Windows.Forms.ToolStripButton();
            this.tbPlacement = new System.Windows.Forms.ToolStripButton();
            this.tbSelect = new System.Windows.Forms.ToolStripButton();
            this.tbCustomSelect = new System.Windows.Forms.ToolStripButton();
            this.tbTranslate = new System.Windows.Forms.ToolStripButton();
            this.tbRotate = new System.Windows.Forms.ToolStripButton();
            this.tbPointLight = new System.Windows.Forms.ToolStripButton();
            this.tbVehicle = new System.Windows.Forms.ToolStripButton();
            this.tbCollision = new System.Windows.Forms.ToolStripButton();
            this.leftPanel = new System.Windows.Forms.Panel();
            this.inspectorPanel.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // inspectorPanel
            // 
            this.inspectorPanel.Controls.Add(this.Inspector);
            this.inspectorPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.inspectorPanel.Location = new System.Drawing.Point(752, 24);
            this.inspectorPanel.Name = "inspectorPanel";
            this.inspectorPanel.Size = new System.Drawing.Size(400, 543);
            this.inspectorPanel.TabIndex = 0;
            // 
            // Inspector
            // 
            this.Inspector.AutoSize = true;
            this.Inspector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Inspector.Location = new System.Drawing.Point(0, 0);
            this.Inspector.Name = "Inspector";
            this.Inspector.Selection = null;
            this.Inspector.Size = new System.Drawing.Size(400, 543);
            this.Inspector.TabIndex = 0;
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiWorldMap,
            this.tsmiLevel,
            this.selectToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1152, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip1";
            // 
            // tsmiWorldMap
            // 
            this.tsmiWorldMap.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveAsToolStripMenuItem});
            this.tsmiWorldMap.Name = "tsmiWorldMap";
            this.tsmiWorldMap.Size = new System.Drawing.Size(40, 20);
            this.tsmiWorldMap.Text = "FILE";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.saveAsToolStripMenuItem.Text = "Save WorldMap As..";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // tsmiLevel
            // 
            this.tsmiLevel.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.compileStripMenuItem});
            this.tsmiLevel.Name = "tsmiLevel";
            this.tsmiLevel.Size = new System.Drawing.Size(50, 20);
            this.tsmiLevel.Text = "LEVEL";
            // 
            // compileStripMenuItem
            // 
            this.compileStripMenuItem.Name = "compileStripMenuItem";
            this.compileStripMenuItem.Size = new System.Drawing.Size(99, 22);
            this.compileStripMenuItem.Text = "Pack";
            this.compileStripMenuItem.Click += new System.EventHandler(this.compileStripMenuItem_Click);
            // 
            // selectToolStripMenuItem
            // 
            this.selectToolStripMenuItem.Name = "selectToolStripMenuItem";
            this.selectToolStripMenuItem.Size = new System.Drawing.Size(12, 20);
            // 
            // toolStrip
            // 
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbUndo,
            this.tbRedo,
            this.tbPlacement,
            this.tbSelect,
            this.tbCustomSelect,
            this.tbTranslate,
            this.tbRotate,
            this.tbPointLight,
            this.tbVehicle,
            this.tbCollision});
            this.toolStrip.Location = new System.Drawing.Point(215, 24);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(372, 39);
            this.toolStrip.TabIndex = 2;
            this.toolStrip.Text = "toolStrip1";
            // 
            // tbUndo
            // 
            this.tbUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbUndo.Image = ((System.Drawing.Image)(resources.GetObject("tbUndo.Image")));
            this.tbUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbUndo.Name = "tbUndo";
            this.tbUndo.Size = new System.Drawing.Size(36, 36);
            this.tbUndo.Text = "Undo";
            this.tbUndo.Click += new System.EventHandler(this.tbUndo_Click);
            // 
            // tbRedo
            // 
            this.tbRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbRedo.Image = ((System.Drawing.Image)(resources.GetObject("tbRedo.Image")));
            this.tbRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbRedo.Name = "tbRedo";
            this.tbRedo.Size = new System.Drawing.Size(36, 36);
            this.tbRedo.Text = "Redo";
            this.tbRedo.Click += new System.EventHandler(this.tbRedo_Click);
            // 
            // tbPlacement
            // 
            this.tbPlacement.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbPlacement.Image = ((System.Drawing.Image)(resources.GetObject("tbPlacement.Image")));
            this.tbPlacement.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbPlacement.Name = "tbPlacement";
            this.tbPlacement.Size = new System.Drawing.Size(36, 36);
            this.tbPlacement.Text = "Placement";
            this.tbPlacement.Click += new System.EventHandler(this.tbPlacement_Click);
            // 
            // tbSelect
            // 
            this.tbSelect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbSelect.Image = ((System.Drawing.Image)(resources.GetObject("tbSelect.Image")));
            this.tbSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbSelect.Name = "tbSelect";
            this.tbSelect.Size = new System.Drawing.Size(36, 36);
            this.tbSelect.Text = "Select";
            this.tbSelect.Click += new System.EventHandler(this.tbSelect_Click);
            // 
            // tbCustomSelect
            // 
            this.tbCustomSelect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbCustomSelect.Image = ((System.Drawing.Image)(resources.GetObject("tbCustomSelect.Image")));
            this.tbCustomSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbCustomSelect.Name = "tbCustomSelect";
            this.tbCustomSelect.Size = new System.Drawing.Size(36, 36);
            this.tbCustomSelect.Tag = "";
            this.tbCustomSelect.Text = "CustomSelect";
            this.tbCustomSelect.Click += new System.EventHandler(this.tbCustomSelect_Click);
            // 
            // tbTranslate
            // 
            this.tbTranslate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbTranslate.Image = ((System.Drawing.Image)(resources.GetObject("tbTranslate.Image")));
            this.tbTranslate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbTranslate.Name = "tbTranslate";
            this.tbTranslate.Size = new System.Drawing.Size(36, 36);
            this.tbTranslate.Tag = "";
            this.tbTranslate.Text = "Translate";
            this.tbTranslate.Click += new System.EventHandler(this.tbTranslate_Click);
            // 
            // tbRotate
            // 
            this.tbRotate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbRotate.Image = ((System.Drawing.Image)(resources.GetObject("tbRotate.Image")));
            this.tbRotate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbRotate.Name = "tbRotate";
            this.tbRotate.Size = new System.Drawing.Size(36, 36);
            this.tbRotate.Tag = "";
            this.tbRotate.Text = "Rotate";
            this.tbRotate.Click += new System.EventHandler(this.tbRotate_Click);
            // 
            // tbPointLight
            // 
            this.tbPointLight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbPointLight.Image = ((System.Drawing.Image)(resources.GetObject("tbPointLight.Image")));
            this.tbPointLight.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbPointLight.Name = "tbPointLight";
            this.tbPointLight.Size = new System.Drawing.Size(36, 36);
            this.tbPointLight.Text = "Point Light";
            this.tbPointLight.Click += new System.EventHandler(this.tbPointLight_Click);
            // 
            // tbVehicle
            // 
            this.tbVehicle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbVehicle.Image = ((System.Drawing.Image)(resources.GetObject("tbVehicle.Image")));
            this.tbVehicle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbVehicle.Name = "tbVehicle";
            this.tbVehicle.Size = new System.Drawing.Size(36, 36);
            this.tbVehicle.Text = "Vehicle";
            this.tbVehicle.Click += new System.EventHandler(this.tbVehicle_Click);
            // 
            // tbCollision
            // 
            this.tbCollision.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbCollision.Image = ((System.Drawing.Image)(resources.GetObject("tbCollision.Image")));
            this.tbCollision.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbCollision.Name = "tbCollision";
            this.tbCollision.Size = new System.Drawing.Size(36, 36);
            this.tbCollision.Tag = "";
            this.tbCollision.Text = "Collision";
            this.tbCollision.Click += new System.EventHandler(this.tbCollision_Click);
            // 
            // leftPanel
            // 
            this.leftPanel.Controls.Add(ObjectHierarchyControl);
            this.leftPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftPanel.Location = new System.Drawing.Point(0, 24);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Size = new System.Drawing.Size(215, 543);
            this.leftPanel.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1152, 567);
            this.Controls.Add(this.leftPanel);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.inspectorPanel);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "RyzeEditor";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.inspectorPanel.ResumeLayout(false);
            this.inspectorPanel.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

        #endregion

        private Panel inspectorPanel;
		private MenuStrip menuStrip;
		private ToolStripMenuItem tsmiWorldMap;
        private ToolStripMenuItem tsmiLevel;
        private ToolStrip toolStrip;
		public Controls.InspectorControl Inspector;
        public Controls.ObjectHierarchyControl ObjectHierarchyControl;
		private ToolStripButton tbTranslate;
		private ToolStripButton tbCustomSelect;
		private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem compileStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
		private ToolStripMenuItem selectToolStripMenuItem;
		private ToolStripButton tbSelect;
		private ToolStripButton tbPlacement;
        private ToolStripButton tbUndo;
        private ToolStripButton tbRedo;
        private ToolStripButton tbRotate;
        private ToolStripButton tbCollision;
        private ToolStripButton tbPointLight;
        private ToolStripButton tbVehicle;
        private Panel leftPanel;
    }
}

