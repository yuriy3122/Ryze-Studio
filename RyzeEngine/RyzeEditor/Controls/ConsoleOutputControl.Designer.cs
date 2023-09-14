namespace RyzeEditor.Controls
{
    partial class ConsoleOutputControl
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
            this.OutputListBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // OutputListBox
            // 
            this.OutputListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OutputListBox.FormattingEnabled = true;
            this.OutputListBox.Location = new System.Drawing.Point(0, 0);
            this.OutputListBox.Name = "OutputListBox";
            this.OutputListBox.ScrollAlwaysVisible = true;
            this.OutputListBox.Size = new System.Drawing.Size(650, 240);
            this.OutputListBox.TabIndex = 0;
            // 
            // ConsoleOutputControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.OutputListBox);
            this.Name = "ConsoleOutputControl";
            this.Size = new System.Drawing.Size(650, 240);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox OutputListBox;
    }
}
