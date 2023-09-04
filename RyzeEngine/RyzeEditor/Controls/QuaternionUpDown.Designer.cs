namespace RyzeEditor.Controls
{
	partial class QuaternionUpDown
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
            this.numSpinWAxis = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.lblX = new System.Windows.Forms.Label();
            this.lblY = new System.Windows.Forms.Label();
            this.lblZ = new System.Windows.Forms.Label();
            this.lblW = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numSpinXAxis)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpinYAxis)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpinZAxis)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpinWAxis)).BeginInit();
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
            this.numSpinXAxis.Location = new System.Drawing.Point(46, 6);
            this.numSpinXAxis.Margin = new System.Windows.Forms.Padding(6);
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
            this.numSpinXAxis.Size = new System.Drawing.Size(138, 31);
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
            this.numSpinYAxis.Location = new System.Drawing.Point(261, 6);
            this.numSpinYAxis.Margin = new System.Windows.Forms.Padding(6);
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
            this.numSpinYAxis.Size = new System.Drawing.Size(138, 31);
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
            this.numSpinZAxis.Location = new System.Drawing.Point(476, 6);
            this.numSpinZAxis.Margin = new System.Windows.Forms.Padding(6);
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
            this.numSpinZAxis.Size = new System.Drawing.Size(140, 31);
            this.numSpinZAxis.TabIndex = 2;
            this.numSpinZAxis.ValueChanged += new System.EventHandler(this.NumSpinZAxis_ValueChanged);
            // 
            // numSpinWAxis
            // 
            this.numSpinWAxis.DecimalPlaces = 3;
            this.numSpinWAxis.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numSpinWAxis.InterceptArrowKeys = false;
            this.numSpinWAxis.Location = new System.Drawing.Point(691, 6);
            this.numSpinWAxis.Margin = new System.Windows.Forms.Padding(6);
            this.numSpinWAxis.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numSpinWAxis.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.numSpinWAxis.Name = "numSpinWAxis";
            this.numSpinWAxis.Size = new System.Drawing.Size(140, 31);
            this.numSpinWAxis.TabIndex = 2;
            this.numSpinWAxis.ValueChanged += new System.EventHandler(this.NumSpinWAxis_ValueChanged);
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.AutoSize = true;
            this.tableLayoutPanel.ColumnCount = 8;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.Controls.Add(this.lblX, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.lblY, 2, 0);
            this.tableLayoutPanel.Controls.Add(this.lblZ, 4, 0);
            this.tableLayoutPanel.Controls.Add(this.lblW, 6, 0);
            this.tableLayoutPanel.Controls.Add(this.numSpinXAxis, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.numSpinYAxis, 3, 0);
            this.tableLayoutPanel.Controls.Add(this.numSpinZAxis, 5, 0);
            this.tableLayoutPanel.Controls.Add(this.numSpinWAxis, 7, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(6);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 1;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(861, 43);
            this.tableLayoutPanel.TabIndex = 3;
            // 
            // lblX
            // 
            this.lblX.AutoSize = true;
            this.lblX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblX.Location = new System.Drawing.Point(6, 0);
            this.lblX.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblX.Name = "lblX";
            this.lblX.Size = new System.Drawing.Size(28, 43);
            this.lblX.TabIndex = 3;
            this.lblX.Text = "X";
            this.lblX.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblY
            // 
            this.lblY.AutoSize = true;
            this.lblY.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblY.Location = new System.Drawing.Point(221, 0);
            this.lblY.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblY.Name = "lblY";
            this.lblY.Size = new System.Drawing.Size(28, 43);
            this.lblY.TabIndex = 4;
            this.lblY.Text = "Y";
            this.lblY.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblZ
            // 
            this.lblZ.AutoSize = true;
            this.lblZ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblZ.Location = new System.Drawing.Point(436, 0);
            this.lblZ.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblZ.Name = "lblZ";
            this.lblZ.Size = new System.Drawing.Size(28, 43);
            this.lblZ.TabIndex = 5;
            this.lblZ.Text = "Z";
            this.lblZ.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblW
            // 
            this.lblW.AutoSize = true;
            this.lblW.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblW.Location = new System.Drawing.Point(651, 0);
            this.lblW.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblW.Name = "lblW";
            this.lblW.Size = new System.Drawing.Size(28, 43);
            this.lblW.TabIndex = 5;
            this.lblW.Text = "A";
            this.lblW.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // QuaternionUpDown
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.tableLayoutPanel);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "QuaternionUpDown";
            this.Size = new System.Drawing.Size(861, 43);
            ((System.ComponentModel.ISupportInitialize)(this.numSpinXAxis)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpinYAxis)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpinZAxis)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpinWAxis)).EndInit();
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.NumericUpDown numSpinXAxis;
		private System.Windows.Forms.NumericUpDown numSpinYAxis;
		private System.Windows.Forms.NumericUpDown numSpinZAxis;
        private System.Windows.Forms.NumericUpDown numSpinWAxis;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
		private System.Windows.Forms.Label lblX;
		private System.Windows.Forms.Label lblZ;
		private System.Windows.Forms.Label lblY;
        private System.Windows.Forms.Label lblW;
    }
}
