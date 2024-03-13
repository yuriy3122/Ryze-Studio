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
            this.ConsoleOutputControl = new RyzeEditor.Controls.ConsoleOutputControl();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.tsmiWorldMap = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.compileStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.simulateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.settingStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.buttomPanel = new System.Windows.Forms.Panel();
            this.buttomTabControl = new System.Windows.Forms.TabControl();
            this.assetsTabPage = new System.Windows.Forms.TabPage();
            this.consoleTabPage = new System.Windows.Forms.TabPage();
            this.inspectorPanel.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.leftPanel.SuspendLayout();
            this.buttomPanel.SuspendLayout();
            this.buttomTabControl.SuspendLayout();
            this.consoleTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // inspectorPanel
            // 
            this.inspectorPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.inspectorPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.inspectorPanel.Controls.Add(this.Inspector);
            this.inspectorPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.inspectorPanel.Location = new System.Drawing.Point(285, 24);
            this.inspectorPanel.Margin = new System.Windows.Forms.Padding(0);
            this.inspectorPanel.Name = "inspectorPanel";
            this.inspectorPanel.Size = new System.Drawing.Size(400, 365);
            this.inspectorPanel.TabIndex = 0;
            // 
            // Inspector
            // 
            this.Inspector.AutoSize = true;
            this.Inspector.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Inspector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Inspector.Location = new System.Drawing.Point(0, 0);
            this.Inspector.Margin = new System.Windows.Forms.Padding(0);
            this.Inspector.Name = "Inspector";
            this.Inspector.Padding = new System.Windows.Forms.Padding(0, 38, 0, 0);
            this.Inspector.Selection = null;
            this.Inspector.Size = new System.Drawing.Size(400, 365);
            this.Inspector.TabIndex = 0;
            // 
            // ObjectHierarchyControl
            // 
            this.ObjectHierarchyControl.AutoSize = true;
            this.ObjectHierarchyControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ObjectHierarchyControl.Location = new System.Drawing.Point(0, 0);
            this.ObjectHierarchyControl.Margin = new System.Windows.Forms.Padding(6);
            this.ObjectHierarchyControl.Name = "ObjectHierarchyControl";
            this.ObjectHierarchyControl.Size = new System.Drawing.Size(215, 326);
            this.ObjectHierarchyControl.TabIndex = 0;
            // 
            // ConsoleOutputControl
            // 
            this.ConsoleOutputControl.AutoSize = true;
            this.ConsoleOutputControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ConsoleOutputControl.Location = new System.Drawing.Point(2, 2);
            this.ConsoleOutputControl.Margin = new System.Windows.Forms.Padding(0);
            this.ConsoleOutputControl.Name = "ConsoleOutputControl";
            this.ConsoleOutputControl.Size = new System.Drawing.Size(58, 112);
            this.ConsoleOutputControl.TabIndex = 5;
            // 
            // menuStrip
            // 
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiWorldMap,
            this.tsmiLevel,
            this.tsmiSettings,
            this.selectToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(3, 1, 0, 1);
            this.menuStrip.Size = new System.Drawing.Size(685, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip";
            // 
            // tsmiWorldMap
            // 
            this.tsmiWorldMap.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveAsToolStripMenuItem});
            this.tsmiWorldMap.Name = "tsmiWorldMap";
            this.tsmiWorldMap.Size = new System.Drawing.Size(40, 22);
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
            this.compileStripMenuItem,
            this.simulateToolStripMenuItem});
            this.tsmiLevel.Name = "tsmiLevel";
            this.tsmiLevel.Size = new System.Drawing.Size(54, 22);
            this.tsmiLevel.Text = "SCENE";
            // 
            // compileStripMenuItem
            // 
            this.compileStripMenuItem.Name = "compileStripMenuItem";
            this.compileStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.compileStripMenuItem.Text = "Pack";
            this.compileStripMenuItem.Click += new System.EventHandler(this.compileStripMenuItem_Click);
            // 
            // simulateToolStripMenuItem
            // 
            this.simulateToolStripMenuItem.Name = "simulateToolStripMenuItem";
            this.simulateToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.simulateToolStripMenuItem.Text = "Simulate/Stop";
            this.simulateToolStripMenuItem.Click += new System.EventHandler(this.simulateToolStripMenuItem_Click);
            // 
            // tsmiSettings
            // 
            this.tsmiSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingStripMenuItem});
            this.tsmiSettings.Name = "tsmiSettings";
            this.tsmiSettings.Size = new System.Drawing.Size(69, 22);
            this.tsmiSettings.Text = "SETTINGS";
            // 
            // settingStripMenuItem
            // 
            this.settingStripMenuItem.Name = "settingStripMenuItem";
            this.settingStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.settingStripMenuItem.Text = "Settings";
            // 
            // selectToolStripMenuItem
            // 
            this.selectToolStripMenuItem.Name = "selectToolStripMenuItem";
            this.selectToolStripMenuItem.Size = new System.Drawing.Size(12, 22);
            // 
            // toolStrip
            // 
            this.toolStrip.BackColor = System.Drawing.SystemColors.ControlLight;
            this.toolStrip.CanOverflow = false;
            this.toolStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
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
            this.toolStrip.Location = new System.Drawing.Point(0, 24);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Padding = new System.Windows.Forms.Padding(0);
            this.toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStrip.Size = new System.Drawing.Size(285, 39);
            this.toolStrip.TabIndex = 2;
            this.toolStrip.Text = "toolStrip";
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
            this.leftPanel.Controls.Add(this.ObjectHierarchyControl);
            this.leftPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftPanel.Location = new System.Drawing.Point(0, 63);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Size = new System.Drawing.Size(215, 326);
            this.leftPanel.TabIndex = 3;
            // 
            // buttomPanel
            // 
            this.buttomPanel.Controls.Add(this.buttomTabControl);
            this.buttomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttomPanel.Location = new System.Drawing.Point(215, 244);
            this.buttomPanel.Margin = new System.Windows.Forms.Padding(0);
            this.buttomPanel.Name = "buttomPanel";
            this.buttomPanel.Size = new System.Drawing.Size(70, 145);
            this.buttomPanel.TabIndex = 4;
            // 
            // buttomTabControl
            // 
            this.buttomTabControl.Controls.Add(this.assetsTabPage);
            this.buttomTabControl.Controls.Add(this.consoleTabPage);
            this.buttomTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttomTabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttomTabControl.Location = new System.Drawing.Point(0, 0);
            this.buttomTabControl.Margin = new System.Windows.Forms.Padding(0);
            this.buttomTabControl.Name = "buttomTabControl";
            this.buttomTabControl.SelectedIndex = 0;
            this.buttomTabControl.Size = new System.Drawing.Size(70, 145);
            this.buttomTabControl.TabIndex = 0;
            // 
            // assetsTabPage
            // 
            this.assetsTabPage.Location = new System.Drawing.Point(4, 25);
            this.assetsTabPage.Margin = new System.Windows.Forms.Padding(2);
            this.assetsTabPage.Name = "assetsTabPage";
            this.assetsTabPage.Padding = new System.Windows.Forms.Padding(2);
            this.assetsTabPage.Size = new System.Drawing.Size(62, 116);
            this.assetsTabPage.TabIndex = 0;
            this.assetsTabPage.Text = "Assets";
            this.assetsTabPage.UseVisualStyleBackColor = true;
            // 
            // consoleTabPage
            // 
            this.consoleTabPage.Controls.Add(this.ConsoleOutputControl);
            this.consoleTabPage.Location = new System.Drawing.Point(4, 25);
            this.consoleTabPage.Margin = new System.Windows.Forms.Padding(2);
            this.consoleTabPage.Name = "consoleTabPage";
            this.consoleTabPage.Padding = new System.Windows.Forms.Padding(2);
            this.consoleTabPage.Size = new System.Drawing.Size(62, 116);
            this.consoleTabPage.TabIndex = 1;
            this.consoleTabPage.Text = "Console";
            this.consoleTabPage.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(685, 389);
            this.Controls.Add(this.buttomPanel);
            this.Controls.Add(this.leftPanel);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.inspectorPanel);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Ryze Studio";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.inspectorPanel.ResumeLayout(false);
            this.inspectorPanel.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.leftPanel.ResumeLayout(false);
            this.leftPanel.PerformLayout();
            this.buttomPanel.ResumeLayout(false);
            this.buttomTabControl.ResumeLayout(false);
            this.consoleTabPage.ResumeLayout(false);
            this.consoleTabPage.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

        #endregion

        private Panel inspectorPanel;
		private MenuStrip menuStrip;
		private ToolStripMenuItem tsmiWorldMap;
        private ToolStripMenuItem tsmiLevel;
        private ToolStripMenuItem tsmiSettings;
        private ToolStrip toolStrip;
		public Controls.InspectorControl Inspector;
        public Controls.ObjectHierarchyControl ObjectHierarchyControl;
        public Controls.ConsoleOutputControl ConsoleOutputControl;
		private ToolStripButton tbTranslate;
		private ToolStripButton tbCustomSelect;
		private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem compileStripMenuItem;
        private ToolStripMenuItem settingStripMenuItem;
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
        private Panel buttomPanel;
        private TabControl buttomTabControl;
        private TabPage assetsTabPage;
        private TabPage consoleTabPage;
        private ToolStripMenuItem simulateToolStripMenuItem;
    }
}

