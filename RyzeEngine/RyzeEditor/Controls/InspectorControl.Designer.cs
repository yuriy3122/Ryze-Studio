using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace RyzeEditor.Controls
{
	partial class InspectorControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.tabControl = new System.Windows.Forms.TabControl();
            this.gameObjectTab = new System.Windows.Forms.TabPage();
            this.layoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.tabControl.SuspendLayout();
            this.gameObjectTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.gameObjectTab);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(400, 445);
            this.tabControl.TabIndex = 4;
            // 
            // gameObjectTab
            // 
            this.gameObjectTab.Controls.Add(this.layoutPanel);
            this.gameObjectTab.Location = new System.Drawing.Point(4, 22);
            this.gameObjectTab.Name = "gameObjectTab";
            this.gameObjectTab.Padding = new System.Windows.Forms.Padding(3);
            this.gameObjectTab.Size = new System.Drawing.Size(392, 419);
            this.gameObjectTab.TabIndex = 0;
            this.gameObjectTab.Text = "Object Properties";
            this.gameObjectTab.UseVisualStyleBackColor = true;
            // 
            // layoutPanel
            // 
            this.layoutPanel.AutoScroll = true;
            this.layoutPanel.AutoSize = true;
            this.layoutPanel.ColumnCount = 1;
            this.layoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutPanel.Location = new System.Drawing.Point(3, 3);
            this.layoutPanel.Name = "layoutPanel";
            this.layoutPanel.Padding = new System.Windows.Forms.Padding(0, 0, 17, 0);
            this.layoutPanel.RowCount = 1;
            this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutPanel.Size = new System.Drawing.Size(386, 413);
            this.layoutPanel.TabIndex = 4;
            // 
            // InspectorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.tabControl);
            this.Name = "InspectorControl";
            this.Size = new System.Drawing.Size(400, 445);
            this.tabControl.ResumeLayout(false);
            this.gameObjectTab.ResumeLayout(false);
            this.gameObjectTab.PerformLayout();
            this.ResumeLayout(false);

		}

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage gameObjectTab;
        public System.Windows.Forms.TableLayoutPanel layoutPanel;
    }
}
