namespace RyzeEditor.Controls
{
	partial class VectorUpDown
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
            this.numSpinXAxis = new System.Windows.Forms.NumericUpDown();
            this.numSpinYAxis = new System.Windows.Forms.NumericUpDown();
            this.numSpinZAxis = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.lblZ = new System.Windows.Forms.Label();
            this.lblY = new System.Windows.Forms.Label();
            this.lblX = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numSpinXAxis)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpinYAxis)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpinZAxis)).BeginInit();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // numSpinXAxis
            // 
            this.numSpinXAxis.DecimalPlaces = 3;
            this.numSpinXAxis.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numSpinXAxis.InterceptArrowKeys = false;
            this.numSpinXAxis.Location = new System.Drawing.Point(23, 3);
            this.numSpinXAxis.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numSpinXAxis.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.numSpinXAxis.Name = "numSpinXAxis";
            this.numSpinXAxis.Size = new System.Drawing.Size(69, 20);
            this.numSpinXAxis.TabIndex = 0;
            this.numSpinXAxis.ValueChanged += new System.EventHandler(this.NumSpinXAxis_ValueChanged);
            // 
            // numSpinYAxis
            // 
            this.numSpinYAxis.DecimalPlaces = 3;
            this.numSpinYAxis.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numSpinYAxis.InterceptArrowKeys = false;
            this.numSpinYAxis.Location = new System.Drawing.Point(119, 3);
            this.numSpinYAxis.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numSpinYAxis.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.numSpinYAxis.Name = "numSpinYAxis";
            this.numSpinYAxis.Size = new System.Drawing.Size(69, 20);
            this.numSpinYAxis.TabIndex = 1;
            this.numSpinYAxis.ValueChanged += new System.EventHandler(this.NumSpinYAxis_ValueChanged);
            // 
            // numSpinZAxis
            // 
            this.numSpinZAxis.DecimalPlaces = 3;
            this.numSpinZAxis.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numSpinZAxis.InterceptArrowKeys = false;
            this.numSpinZAxis.Location = new System.Drawing.Point(215, 3);
            this.numSpinZAxis.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numSpinZAxis.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.numSpinZAxis.Name = "numSpinZAxis";
            this.numSpinZAxis.Size = new System.Drawing.Size(70, 20);
            this.numSpinZAxis.TabIndex = 2;
            this.numSpinZAxis.ValueChanged += new System.EventHandler(this.NumSpinZAxis_ValueChanged);
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.AutoSize = true;
            this.tableLayoutPanel.ColumnCount = 6;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.Controls.Add(this.lblZ, 4, 0);
            this.tableLayoutPanel.Controls.Add(this.lblY, 2, 0);
            this.tableLayoutPanel.Controls.Add(this.numSpinXAxis, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.numSpinYAxis, 3, 0);
            this.tableLayoutPanel.Controls.Add(this.numSpinZAxis, 5, 0);
            this.tableLayoutPanel.Controls.Add(this.lblX, 0, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 1;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(288, 26);
            this.tableLayoutPanel.TabIndex = 3;
            // 
            // lblZ
            // 
            this.lblZ.AutoSize = true;
            this.lblZ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblZ.Location = new System.Drawing.Point(195, 0);
            this.lblZ.Name = "lblZ";
            this.lblZ.Size = new System.Drawing.Size(14, 26);
            this.lblZ.TabIndex = 5;
            this.lblZ.Text = "Z";
            this.lblZ.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblY
            // 
            this.lblY.AutoSize = true;
            this.lblY.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblY.Location = new System.Drawing.Point(99, 0);
            this.lblY.Name = "lblY";
            this.lblY.Size = new System.Drawing.Size(14, 26);
            this.lblY.TabIndex = 4;
            this.lblY.Text = "Y";
            this.lblY.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblX
            // 
            this.lblX.AutoSize = true;
            this.lblX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblX.Location = new System.Drawing.Point(3, 0);
            this.lblX.Name = "lblX";
            this.lblX.Size = new System.Drawing.Size(14, 26);
            this.lblX.TabIndex = 3;
            this.lblX.Text = "X";
            this.lblX.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // VectorUpDown
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "VectorUpDown";
            this.Size = new System.Drawing.Size(288, 26);
            ((System.ComponentModel.ISupportInitialize)(this.numSpinXAxis)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpinYAxis)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpinZAxis)).EndInit();
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.NumericUpDown numSpinXAxis;
		private System.Windows.Forms.NumericUpDown numSpinYAxis;
		private System.Windows.Forms.NumericUpDown numSpinZAxis;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
		private System.Windows.Forms.Label lblX;
		private System.Windows.Forms.Label lblZ;
		private System.Windows.Forms.Label lblY;
	}
}
