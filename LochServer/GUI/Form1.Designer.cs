namespace Loch
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lstUsers = new ListView();
            txtLog = new RichTextBox();
            SuspendLayout();
            // 
            // lstUsers
            // 
            lstUsers.Location = new Point(0, 29);
            lstUsers.Name = "lstUsers";
            lstUsers.Size = new Size(137, 421);
            lstUsers.TabIndex = 1;
            lstUsers.UseCompatibleStateImageBehavior = false;
            // 
            // txtLog
            // 
            txtLog.Location = new Point(143, 29);
            txtLog.Name = "txtLog";
            txtLog.ScrollBars = RichTextBoxScrollBars.Vertical;
            txtLog.Size = new Size(656, 358);
            txtLog.TabIndex = 2;
            txtLog.Text = "";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(txtLog);
            Controls.Add(lstUsers);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion
        private ListView lstUsers;
        private RichTextBox txtLog;
    }
}
