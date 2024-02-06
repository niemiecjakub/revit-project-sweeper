namespace ProjectSweeper.StairModule
{
    partial class StairModuleUI
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.SideInputBox = new System.Windows.Forms.TextBox();
            this.runButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "SIDE";
            // 
            // SideInputBox
            // 
            this.SideInputBox.Location = new System.Drawing.Point(66, 36);
            this.SideInputBox.Name = "SideInputBox";
            this.SideInputBox.Size = new System.Drawing.Size(134, 20);
            this.SideInputBox.TabIndex = 1;
            this.SideInputBox.TextChanged += new System.EventHandler(this.SideInputBox_TextChanged);
            // 
            // runButton
            // 
            this.runButton.Location = new System.Drawing.Point(16, 79);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(184, 32);
            this.runButton.TabIndex = 2;
            this.runButton.Text = "RUN";
            this.runButton.UseVisualStyleBackColor = true;
            this.runButton.Click += new System.EventHandler(this.runButton_Click);
            // 
            // StairModuleUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(214, 136);
            this.Controls.Add(this.runButton);
            this.Controls.Add(this.SideInputBox);
            this.Controls.Add(this.label1);
            this.Name = "StairModuleUI";
            this.Text = "StairModuleUI";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox SideInputBox;
        private System.Windows.Forms.Button runButton;
    }
}