namespace RyzeEditor.Controls
{
    partial class WheelControl
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
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.wheelSelectComboBox = new System.Windows.Forms.ComboBox();
            this.subMeshSelectButton = new System.Windows.Forms.Button();
            this.lblWhellSelect = new System.Windows.Forms.Label();
            this.lblSelectSubMesh = new System.Windows.Forms.Label();
            this.selectedMeshesListBox = new System.Windows.Forms.ListBox();
            this.deleteSelectedMeshButton = new System.Windows.Forms.Button();
            this.lblSelectedSubMeshes = new System.Windows.Forms.Label();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.AutoSize = true;
            this.tableLayoutPanel.ColumnCount = 3;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel.Controls.Add(this.wheelSelectComboBox, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.subMeshSelectButton, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.lblWhellSelect, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.lblSelectSubMesh, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.selectedMeshesListBox, 0, 5);
            this.tableLayoutPanel.Controls.Add(this.deleteSelectedMeshButton, 2, 5);
            this.tableLayoutPanel.Controls.Add(this.lblSelectedSubMeshes, 0, 4);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 6;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(276, 500);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // wheelSelectComboBox
            // 
            this.wheelSelectComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wheelSelectComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.wheelSelectComboBox.FormattingEnabled = true;
            this.wheelSelectComboBox.Items.AddRange(new object[] {
            "Front Left",
            "Front Right",
            "Back Left",
            "Back Right"});
            this.wheelSelectComboBox.Location = new System.Drawing.Point(3, 23);
            this.wheelSelectComboBox.Name = "wheelSelectComboBox";
            this.wheelSelectComboBox.Size = new System.Drawing.Size(236, 21);
            this.wheelSelectComboBox.TabIndex = 0;
            this.wheelSelectComboBox.SelectedValueChanged += new System.EventHandler(this.WheelSelectComboBox_SelectedValueChanged);
            // 
            // subMeshSelectButton
            // 
            this.subMeshSelectButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.subMeshSelectButton.FlatAppearance.BorderColor = System.Drawing.Color.IndianRed;
            this.subMeshSelectButton.Location = new System.Drawing.Point(3, 70);
            this.subMeshSelectButton.Name = "subMeshSelectButton";
            this.subMeshSelectButton.Size = new System.Drawing.Size(236, 21);
            this.subMeshSelectButton.TabIndex = 1;
            this.subMeshSelectButton.Text = "Select";
            this.subMeshSelectButton.Click += new System.EventHandler(this.SubMeshSelectButton_Click);
            // 
            // lblWhellSelect
            // 
            this.lblWhellSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblWhellSelect.AutoSize = true;
            this.lblWhellSelect.Location = new System.Drawing.Point(3, 7);
            this.lblWhellSelect.Name = "lblWhellSelect";
            this.lblWhellSelect.Size = new System.Drawing.Size(74, 13);
            this.lblWhellSelect.TabIndex = 3;
            this.lblWhellSelect.Text = "Select Wheel:";
            // 
            // lblSelectSubMesh
            // 
            this.lblSelectSubMesh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSelectSubMesh.AutoSize = true;
            this.lblSelectSubMesh.Location = new System.Drawing.Point(3, 54);
            this.lblSelectSubMesh.Name = "lblSelectSubMesh";
            this.lblSelectSubMesh.Size = new System.Drawing.Size(55, 13);
            this.lblSelectSubMesh.TabIndex = 4;
            this.lblSelectSubMesh.Text = "SubMesh:";
            // 
            // selectedMeshesListBox
            // 
            this.selectedMeshesListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectedMeshesListBox.FormattingEnabled = true;
            this.selectedMeshesListBox.Location = new System.Drawing.Point(3, 117);
            this.selectedMeshesListBox.Name = "selectedMeshesListBox";
            this.selectedMeshesListBox.Size = new System.Drawing.Size(236, 380);
            this.selectedMeshesListBox.TabIndex = 5;
            // 
            // deleteSelectedMeshButton
            // 
            this.deleteSelectedMeshButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.deleteSelectedMeshButton.Location = new System.Drawing.Point(250, 117);
            this.deleteSelectedMeshButton.Name = "deleteSelectedMeshButton";
            this.deleteSelectedMeshButton.Size = new System.Drawing.Size(23, 23);
            this.deleteSelectedMeshButton.TabIndex = 6;
            this.deleteSelectedMeshButton.Text = "-";
            this.deleteSelectedMeshButton.UseVisualStyleBackColor = true;
            this.deleteSelectedMeshButton.Click += new System.EventHandler(this.DeleteSelectedMeshButton_Click);
            // 
            // lblSelectedSubMeshes
            // 
            this.lblSelectedSubMeshes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSelectedSubMeshes.AutoSize = true;
            this.lblSelectedSubMeshes.Location = new System.Drawing.Point(3, 101);
            this.lblSelectedSubMeshes.Name = "lblSelectedSubMeshes";
            this.lblSelectedSubMeshes.Size = new System.Drawing.Size(111, 13);
            this.lblSelectedSubMeshes.TabIndex = 7;
            this.lblSelectedSubMeshes.Text = "Selected SubMeshes:";
            // 
            // WheelControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "WheelControl";
            this.Size = new System.Drawing.Size(276, 500);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.ComboBox wheelSelectComboBox;
        private System.Windows.Forms.Button subMeshSelectButton;
        private System.Windows.Forms.Label lblWhellSelect;
        private System.Windows.Forms.Label lblSelectSubMesh;
        private System.Windows.Forms.ListBox selectedMeshesListBox;
        private System.Windows.Forms.Button deleteSelectedMeshButton;
        private System.Windows.Forms.Label lblSelectedSubMeshes;
    }
}
