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
            EntryBox = new TextBox();
            lstUsers = new ListView();
            SuspendLayout();
            // 
            // txtLog
            // 
            txtLog.Location = new Point(143, 29);
            txtLog.Name = "txtLog";
            txtLog.Size = new Size(656, 358);
            txtLog.TabIndex = 0;
            txtLog.Text = "";
            txtLog.TextChanged += textBox1_TextChanged;
            // 
            // EntryBox
            // 
            EntryBox.Location = new Point(158, 405);
            EntryBox.Name = "EntryBox";
            EntryBox.Size = new Size(621, 23);
            EntryBox.TabIndex = 1;
            EntryBox.TextChanged += textBox1_TextChanged_1;
            EntryBox.KeyDown += EntryBox_KeyDown;
            // 
            // lstUsers
            // 
            lstUsers.Location = new Point(0, 29);
            lstUsers.Name = "lstUsers";
            lstUsers.Size = new Size(137, 423);
            lstUsers.TabIndex = 2;
            lstUsers.UseCompatibleStateImageBehavior = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lstUsers);
            Controls.Add(EntryBox);
            Controls.Add(txtLog);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox txtLog;
        private TextBox EntryBox;
        private ListView lstUsers;
    }
}
