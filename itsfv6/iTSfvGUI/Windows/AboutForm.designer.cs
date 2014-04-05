using HelpersLib;
using iTSfvGUI.Properties;
namespace iTSfvGUI
{
    partial class AboutForm
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
            this.lblProductName = new System.Windows.Forms.Label();
            this.lblWebsite = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblCopyright = new System.Windows.Forms.Label();
            this.lblBugs = new System.Windows.Forms.Label();
            this.pbMikeURL = new System.Windows.Forms.PictureBox();
            this.pbAU = new System.Windows.Forms.PictureBox();
            this.lblMike = new System.Windows.Forms.Label();
            this.pbLogo = new System.Windows.Forms.PictureBox();
            this.txtDetails = new System.Windows.Forms.RichTextBox();
            this.uclUpdate = new UpdateCheckerLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pbMikeURL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAU)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // lblProductName
            // 
            this.lblProductName.AutoSize = true;
            this.lblProductName.BackColor = System.Drawing.Color.Transparent;
            this.lblProductName.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblProductName.ForeColor = System.Drawing.Color.Black;
            this.lblProductName.Location = new System.Drawing.Point(272, 8);
            this.lblProductName.Name = "lblProductName";
            this.lblProductName.Size = new System.Drawing.Size(134, 24);
            this.lblProductName.TabIndex = 0;
            this.lblProductName.Text = "ShareX 1.0.0.0";
            this.lblProductName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblWebsite
            // 
            this.lblWebsite.AutoSize = true;
            this.lblWebsite.BackColor = System.Drawing.Color.Transparent;
            this.lblWebsite.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblWebsite.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblWebsite.ForeColor = System.Drawing.Color.Black;
            this.lblWebsite.Location = new System.Drawing.Point(272, 64);
            this.lblWebsite.Name = "lblWebsite";
            this.lblWebsite.Size = new System.Drawing.Size(67, 13);
            this.lblWebsite.TabIndex = 2;
            this.lblWebsite.Text = "Project page";
            this.lblWebsite.Click += new System.EventHandler(this.lblWebsite_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.ForeColor = System.Drawing.Color.Black;
            this.btnClose.Location = new System.Drawing.Point(530, 542);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(67, 31);
            this.btnClose.TabIndex = 8;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblCopyright
            // 
            this.lblCopyright.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCopyright.AutoSize = true;
            this.lblCopyright.BackColor = System.Drawing.Color.Transparent;
            this.lblCopyright.ForeColor = System.Drawing.Color.Black;
            this.lblCopyright.Location = new System.Drawing.Point(8, 558);
            this.lblCopyright.Name = "lblCopyright";
            this.lblCopyright.Size = new System.Drawing.Size(51, 13);
            this.lblCopyright.TabIndex = 7;
            this.lblCopyright.Text = "Copyright";
            this.lblCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBugs
            // 
            this.lblBugs.AutoSize = true;
            this.lblBugs.BackColor = System.Drawing.Color.Transparent;
            this.lblBugs.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblBugs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblBugs.ForeColor = System.Drawing.Color.Black;
            this.lblBugs.Location = new System.Drawing.Point(344, 64);
            this.lblBugs.Name = "lblBugs";
            this.lblBugs.Size = new System.Drawing.Size(100, 13);
            this.lblBugs.TabIndex = 3;
            this.lblBugs.Text = "Bugs / Suggestions";
            this.lblBugs.Click += new System.EventHandler(this.lblBugs_Click);
            // 
            // pbMikeURL
            // 
            this.pbMikeURL.BackColor = System.Drawing.Color.Transparent;
            this.pbMikeURL.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbMikeURL.Image = global::iTSfvGUI.Properties.Resources.application_browser;
            this.pbMikeURL.Location = new System.Drawing.Point(304, 91);
            this.pbMikeURL.Name = "pbMikeURL";
            this.pbMikeURL.Size = new System.Drawing.Size(16, 16);
            this.pbMikeURL.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbMikeURL.TabIndex = 14;
            this.pbMikeURL.TabStop = false;
            this.pbMikeURL.Click += new System.EventHandler(this.pbMikeURL_Click);
            // 
            // pbAU
            // 
            this.pbAU.BackColor = System.Drawing.Color.Transparent;
            this.pbAU.Image = global::iTSfvGUI.Properties.Resources.au;
            this.pbAU.Location = new System.Drawing.Point(280, 91);
            this.pbAU.Name = "pbAU";
            this.pbAU.Size = new System.Drawing.Size(16, 16);
            this.pbAU.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbAU.TabIndex = 12;
            this.pbAU.TabStop = false;
            // 
            // lblMike
            // 
            this.lblMike.AutoSize = true;
            this.lblMike.BackColor = System.Drawing.Color.Transparent;
            this.lblMike.ForeColor = System.Drawing.Color.Black;
            this.lblMike.Location = new System.Drawing.Point(328, 93);
            this.lblMike.Name = "lblMike";
            this.lblMike.Size = new System.Drawing.Size(87, 13);
            this.lblMike.TabIndex = 5;
            this.lblMike.Text = "Michael Delpach";
            // 
            // pbLogo
            // 
            this.pbLogo.Image = global::iTSfvGUI.Properties.Resources.itsfv_logo;
            this.pbLogo.Location = new System.Drawing.Point(8, 8);
            this.pbLogo.Name = "pbLogo";
            this.pbLogo.Size = new System.Drawing.Size(264, 264);
            this.pbLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbLogo.TabIndex = 19;
            this.pbLogo.TabStop = false;
            // 
            // txtDetails
            // 
            this.txtDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDetails.Location = new System.Drawing.Point(8, 280);
            this.txtDetails.Name = "txtDetails";
            this.txtDetails.ReadOnly = true;
            this.txtDetails.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtDetails.Size = new System.Drawing.Size(586, 254);
            this.txtDetails.TabIndex = 6;
            this.txtDetails.Text = "";
            this.txtDetails.WordWrap = false;
            this.txtDetails.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.txtDetails_LinkClicked);
            // 
            // uclUpdate
            // 
            this.uclUpdate.Location = new System.Drawing.Point(272, 35);
            this.uclUpdate.Name = "uclUpdate";
            this.uclUpdate.Size = new System.Drawing.Size(250, 24);
            this.uclUpdate.TabIndex = 1;
            // 
            // AboutForm
            // 
            this.AcceptButton = this.btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 582);
            this.Controls.Add(this.txtDetails);
            this.Controls.Add(this.uclUpdate);
            this.Controls.Add(this.pbLogo);
            this.Controls.Add(this.pbMikeURL);
            this.Controls.Add(this.pbAU);
            this.Controls.Add(this.lblMike);
            this.Controls.Add(this.lblBugs);
            this.Controls.Add(this.lblCopyright);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblWebsite);
            this.Controls.Add(this.lblProductName);
            this.MinimumSize = new System.Drawing.Size(620, 620);
            this.Name = "AboutForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - About";
            this.Shown += new System.EventHandler(this.AboutForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pbMikeURL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAU)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion Windows Form Designer generated code

        private System.Windows.Forms.Label lblProductName;
        private System.Windows.Forms.Label lblWebsite;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblCopyright;
        private System.Windows.Forms.Label lblBugs;
        private System.Windows.Forms.PictureBox pbMikeURL;
        private System.Windows.Forms.PictureBox pbAU;
        private System.Windows.Forms.Label lblMike;
        private System.Windows.Forms.PictureBox pbLogo;
        private UpdateCheckerLabel uclUpdate;
        private System.Windows.Forms.RichTextBox txtDetails;
    }
}