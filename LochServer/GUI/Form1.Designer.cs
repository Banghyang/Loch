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
            txtLog = new RichTextBox();
            lstUsers = new ListView();
            SuspendLayout();
            // 
            // txtLog
            // 
            txtLog.Location = new Point(143, 29);
            txtLog.Name = "txtLog";
            txtLog.Size = new Size(657, 358);
            txtLog.TabIndex = 0;
            txtLog.Text = "";
            // 
            // lstUsers
            // 
            lstUsers.Location = new Point(0, 29);
            lstUsers.Name = "lstUsers";
            lstUsers.Size = new Size(137, 421);
            lstUsers.TabIndex = 1;
            lstUsers.UseCompatibleStateImageBehavior = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lstUsers);
            Controls.Add(txtLog);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion

        private RichTextBox txtLog;
        private ListView lstUsers;
    }
}
