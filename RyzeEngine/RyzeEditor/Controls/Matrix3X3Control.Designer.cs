namespace RyzeEditor.Controls
{
    partial class Matrix3x3Control
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
            this.numSpinM33 = new System.Windows.Forms.NumericUpDown();
            this.numSpinM32 = new System.Windows.Forms.NumericUpDown();
            this.numSpinM23 = new System.Windows.Forms.NumericUpDown();
            this.numSpinM22 = new System.Windows.Forms.NumericUpDown();
            this.lblM33 = new System.Windows.Forms.Label();
            this.lblM32 = new System.Windows.Forms.Label();
            this.lblM23 = new System.Windows.Forms.Label();
            this.lblM22 = new System.Windows.Forms.Label();
            this.numSpinM31 = new System.Windows.Forms.NumericUpDown();
            this.lblM31 = new System.Windows.Forms.Label();
            this.numSpinM21 = new System.Windows.Forms.NumericUpDown();
            this.lblM21 = new System.Windows.Forms.Label();
            this.lblM13 = new System.Windows.Forms.Label();
            this.lblM12 = new System.Windows.Forms.Label();
            this.numSpinM11 = new System.Windows.Forms.NumericUpDown();
            this.numSpinM12 = new System.Windows.Forms.NumericUpDown();
            this.numSpinM13 = new System.Windows.Forms.NumericUpDown();
            this.lblM11 = new System.Windows.Forms.Label();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSpinM33)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpinM32)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpinM23)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpinM22)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpinM31)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpinM21)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpinM11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpinM12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpinM13)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.AutoSize = true;
            this.tableLayoutPanel.ColumnCount = 6;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.Controls.Add(this.numSpinM33, 5, 4);
            this.tableLayoutPanel.Controls.Add(this.numSpinM32, 3, 4);
            this.tableLayoutPanel.Controls.Add(this.numSpinM23, 5, 2);
            this.tableLayoutPanel.Controls.Add(this.numSpinM22, 3, 2);
            this.tableLayoutPanel.Controls.Add(this.lblM33, 4, 4);
            this.tableLayoutPanel.Controls.Add(this.lblM32, 2, 4);
            this.tableLayoutPanel.Controls.Add(this.lblM23, 4, 2);
            this.tableLayoutPanel.Controls.Add(this.lblM22, 2, 2);
            this.tableLayoutPanel.Controls.Add(this.numSpinM31, 1, 4);
            this.tableLayoutPanel.Controls.Add(this.lblM31, 0, 4);
            this.tableLayoutPanel.Controls.Add(this.numSpinM21, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.lblM21, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.lblM13, 4, 0);
            this.tableLayoutPanel.Controls.Add(this.lblM12, 2, 0);
            this.tableLayoutPanel.Controls.Add(this.numSpinM11, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.numSpinM12, 3, 0);
            this.tableLayoutPanel.Controls.Add(this.numSpinM13, 5, 0);
            this.tableLayoutPanel.Controls.Add(this.lblM11, 0, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 5;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(392, 99);
            this.tableLayoutPanel.TabIndex = 4;
            // 
            // numSpinM33
            // 
            this.numSpinM33.DecimalPlaces = 3;
            this.numSpinM33.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numSpinM33.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numSpinM33.InterceptArrowKeys = false;
            this.numSpinM33.Location = new System.Drawing.Point(298, 75);
            this.numSpinM33.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numSpinM33.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.numSpinM33.Name = "numSpinM33";
            this.numSpinM33.Size = new System.Drawing.Size(91, 20);
            this.numSpinM33.TabIndex = 17;
            // 
            // numSpinM32
            // 
            this.numSpinM32.DecimalPlaces = 3;
            this.numSpinM32.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numSpinM32.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numSpinM32.InterceptArrowKeys = false;
            this.numSpinM32.Location = new System.Drawing.Point(168, 75);
            this.numSpinM32.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numSpinM32.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.numSpinM32.Name = "numSpinM32";
            this.numSpinM32.Size = new System.Drawing.Size(89, 20);
            this.numSpinM32.TabIndex = 16;
            // 
            // numSpinM23
            // 
            this.numSpinM23.DecimalPlaces = 3;
            this.numSpinM23.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numSpinM23.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numSpinM23.InterceptArrowKeys = false;
            this.numSpinM23.Location = new System.Drawing.Point(298, 39);
            this.numSpinM23.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numSpinM23.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.numSpinM23.Name = "numSpinM23";
            this.numSpinM23.Size = new System.Drawing.Size(91, 20);
            this.numSpinM23.TabIndex = 15;
            // 
            // numSpinM22
            // 
            this.numSpinM22.DecimalPlaces = 3;
            this.numSpinM22.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numSpinM22.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numSpinM22.InterceptArrowKeys = false;
            this.numSpinM22.Location = new System.Drawing.Point(168, 39);
            this.numSpinM22.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numSpinM22.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.numSpinM22.Name = "numSpinM22";
            this.numSpinM22.Size = new System.Drawing.Size(89, 20);
            this.numSpinM22.TabIndex = 14;
            // 
            // lblM33
            // 
            this.lblM33.AutoSize = true;
            this.lblM33.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblM33.Location = new System.Drawing.Point(263, 72);
            this.lblM33.Name = "lblM33";
            this.lblM33.Size = new System.Drawing.Size(29, 27);
            this.lblM33.TabIndex = 13;
            this.lblM33.Text = "M33";
            this.lblM33.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblM32
            // 
            this.lblM32.AutoSize = true;
            this.lblM32.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblM32.Location = new System.Drawing.Point(133, 72);
            this.lblM32.Name = "lblM32";
            this.lblM32.Size = new System.Drawing.Size(29, 27);
            this.lblM32.TabIndex = 12;
            this.lblM32.Text = "M32";
            this.lblM32.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblM23
            // 
            this.lblM23.AutoSize = true;
            this.lblM23.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblM23.Location = new System.Drawing.Point(263, 36);
            this.lblM23.Name = "lblM23";
            this.lblM23.Size = new System.Drawing.Size(29, 26);
            this.lblM23.TabIndex = 11;
            this.lblM23.Text = "M23";
            this.lblM23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblM22
            // 
            this.lblM22.AutoSize = true;
            this.lblM22.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblM22.Location = new System.Drawing.Point(133, 36);
            this.lblM22.Name = "lblM22";
            this.lblM22.Size = new System.Drawing.Size(29, 26);
            this.lblM22.TabIndex = 10;
            this.lblM22.Text = "M22";
            this.lblM22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numSpinM31
            // 
            this.numSpinM31.DecimalPlaces = 3;
            this.numSpinM31.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numSpinM31.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numSpinM31.InterceptArrowKeys = false;
            this.numSpinM31.Location = new System.Drawing.Point(38, 75);
            this.numSpinM31.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numSpinM31.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.numSpinM31.Name = "numSpinM31";
            this.numSpinM31.Size = new System.Drawing.Size(89, 20);
            this.numSpinM31.TabIndex = 9;
            // 
            // lblM31
            // 
            this.lblM31.AutoSize = true;
            this.lblM31.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblM31.Location = new System.Drawing.Point(3, 72);
            this.lblM31.Name = "lblM31";
            this.lblM31.Size = new System.Drawing.Size(29, 27);
            this.lblM31.TabIndex = 8;
            this.lblM31.Text = "M31";
            this.lblM31.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numSpinM21
            // 
            this.numSpinM21.DecimalPlaces = 3;
            this.numSpinM21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numSpinM21.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numSpinM21.InterceptArrowKeys = false;
            this.numSpinM21.Location = new System.Drawing.Point(38, 39);
            this.numSpinM21.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numSpinM21.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.numSpinM21.Name = "numSpinM21";
            this.numSpinM21.Size = new System.Drawing.Size(89, 20);
            this.numSpinM21.TabIndex = 7;
            // 
            // lblM21
            // 
            this.lblM21.AutoSize = true;
            this.lblM21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblM21.Location = new System.Drawing.Point(3, 36);
            this.lblM21.Name = "lblM21";
            this.lblM21.Size = new System.Drawing.Size(29, 26);
            this.lblM21.TabIndex = 6;
            this.lblM21.Text = "M21";
            this.lblM21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblM13
            // 
            this.lblM13.AutoSize = true;
            this.lblM13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblM13.Location = new System.Drawing.Point(263, 0);
            this.lblM13.Name = "lblM13";
            this.lblM13.Size = new System.Drawing.Size(29, 26);
            this.lblM13.TabIndex = 5;
            this.lblM13.Text = "M13";
            this.lblM13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblM12
            // 
            this.lblM12.AutoSize = true;
            this.lblM12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblM12.Location = new System.Drawing.Point(133, 0);
            this.lblM12.Name = "lblM12";
            this.lblM12.Size = new System.Drawing.Size(29, 26);
            this.lblM12.TabIndex = 4;
            this.lblM12.Text = "M12";
            this.lblM12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numSpinM11
            // 
            this.numSpinM11.DecimalPlaces = 3;
            this.numSpinM11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numSpinM11.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numSpinM11.InterceptArrowKeys = false;
            this.numSpinM11.Location = new System.Drawing.Point(38, 3);
            this.numSpinM11.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numSpinM11.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.numSpinM11.Name = "numSpinM11";
            this.numSpinM11.Size = new System.Drawing.Size(89, 20);
            this.numSpinM11.TabIndex = 0;
            // 
            // numSpinM12
            // 
            this.numSpinM12.DecimalPlaces = 3;
            this.numSpinM12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numSpinM12.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numSpinM12.InterceptArrowKeys = false;
            this.numSpinM12.Location = new System.Drawing.Point(168, 3);
            this.numSpinM12.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numSpinM12.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.numSpinM12.Name = "numSpinM12";
            this.numSpinM12.Size = new System.Drawing.Size(89, 20);
            this.numSpinM12.TabIndex = 1;
            // 
            // numSpinM13
            // 
            this.numSpinM13.DecimalPlaces = 3;
            this.numSpinM13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numSpinM13.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numSpinM13.InterceptArrowKeys = false;
            this.numSpinM13.Location = new System.Drawing.Point(298, 3);
            this.numSpinM13.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numSpinM13.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.numSpinM13.Name = "numSpinM13";
            this.numSpinM13.Size = new System.Drawing.Size(91, 20);
            this.numSpinM13.TabIndex = 2;
            // 
            // lblM11
            // 
            this.lblM11.AutoSize = true;
            this.lblM11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblM11.Location = new System.Drawing.Point(3, 0);
            this.lblM11.Name = "lblM11";
            this.lblM11.Size = new System.Drawing.Size(29, 26);
            this.lblM11.TabIndex = 3;
            this.lblM11.Text = "M11";
            this.lblM11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Matrix3x3Control
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "Matrix3x3Control";
            this.Size = new System.Drawing.Size(392, 99);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSpinM33)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpinM32)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpinM23)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpinM22)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpinM31)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpinM21)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpinM11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpinM12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpinM13)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label lblM13;
        private System.Windows.Forms.Label lblM12;
        private System.Windows.Forms.NumericUpDown numSpinM11;
        private System.Windows.Forms.NumericUpDown numSpinM12;
        private System.Windows.Forms.NumericUpDown numSpinM13;
        private System.Windows.Forms.Label lblM11;
        private System.Windows.Forms.NumericUpDown numSpinM21;
        private System.Windows.Forms.Label lblM21;
        private System.Windows.Forms.NumericUpDown numSpinM31;
        private System.Windows.Forms.Label lblM31;
        private System.Windows.Forms.Label lblM33;
        private System.Windows.Forms.Label lblM32;
        private System.Windows.Forms.Label lblM23;
        private System.Windows.Forms.Label lblM22;
        private System.Windows.Forms.NumericUpDown numSpinM33;
        private System.Windows.Forms.NumericUpDown numSpinM32;
        private System.Windows.Forms.NumericUpDown numSpinM23;
        private System.Windows.Forms.NumericUpDown numSpinM22;
    }
}
