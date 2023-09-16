namespace RyzeEditor.Controls
{
    partial class ObjectHierarchyControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ObjectHierarchyControl));
            this.centerPanel = new System.Windows.Forms.Panel();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.objectHierarchyTab = new System.Windows.Forms.TabPage();
            this.HierarchyTreeView = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.centerPanel.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.objectHierarchyTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // centerPanel
            // 
            this.centerPanel.Controls.Add(this.tabControl);
            this.centerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.centerPanel.Location = new System.Drawing.Point(0, 0);
            this.centerPanel.Name = "centerPanel";
            this.centerPanel.Size = new System.Drawing.Size(251, 445);
            this.centerPanel.TabIndex = 0;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.objectHierarchyTab);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(251, 445);
            this.tabControl.TabIndex = 5;
            // 
            // objectHierarchyTab
            // 
            this.objectHierarchyTab.AutoScroll = true;
            this.objectHierarchyTab.Controls.Add(this.HierarchyTreeView);
            this.objectHierarchyTab.Location = new System.Drawing.Point(4, 25);
            this.objectHierarchyTab.Name = "objectHierarchyTab";
            this.objectHierarchyTab.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.objectHierarchyTab.Size = new System.Drawing.Size(243, 416);
            this.objectHierarchyTab.TabIndex = 0;
            this.objectHierarchyTab.Text = "Hierarchy";
            this.objectHierarchyTab.UseVisualStyleBackColor = true;
            // 
            // HierarchyTreeView
            // 
            this.HierarchyTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.HierarchyTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HierarchyTreeView.ImageIndex = 0;
            this.HierarchyTreeView.ImageList = this.imageList;
            this.HierarchyTreeView.Location = new System.Drawing.Point(3, 3);
            this.HierarchyTreeView.Name = "HierarchyTreeView";
            this.HierarchyTreeView.SelectedImageIndex = 0;
            this.HierarchyTreeView.Size = new System.Drawing.Size(237, 410);
            this.HierarchyTreeView.TabIndex = 0;
            this.HierarchyTreeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HierarchyTreeView_KeyDown);
            this.HierarchyTreeView.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.HierarchyTreeView_KeyPress);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "world.png");
            this.imageList.Images.SetKeyName(1, "gameObject.png");
            this.imageList.Images.SetKeyName(2, "vehicle.png");
            this.imageList.Images.SetKeyName(3, "camera.png");
            this.imageList.Images.SetKeyName(4, "sun2.png");
            // 
            // ObjectHierarchyControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.centerPanel);
            this.Name = "ObjectHierarchyControl";
            this.Size = new System.Drawing.Size(251, 445);
            this.centerPanel.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.objectHierarchyTab.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel centerPanel;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage objectHierarchyTab;
        private System.Windows.Forms.TreeView HierarchyTreeView;
        private System.Windows.Forms.ImageList imageList;
    }
}
