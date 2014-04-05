namespace iTSfvGUI
{
    partial class OptionsWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsWindow));
            this.pgOptions = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // pgOptions
            // 
            this.pgOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgOptions.Location = new System.Drawing.Point(0, 0);
            this.pgOptions.Name = "pgOptions";
            this.pgOptions.Size = new System.Drawing.Size(555, 423);
            this.pgOptions.TabIndex = 0;
            // 
            // OptionsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(555, 423);
            this.Controls.Add(this.pgOptions);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OptionsWindow";
            this.Text = "Options";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid pgOptions;
    }
}