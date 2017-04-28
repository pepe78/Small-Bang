namespace SmallBang
{
    partial class SmallBangForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SmallBangForm));
            this.clusterListBox = new System.Windows.Forms.ListBox();
            this.emailListBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // clusterListBox
            // 
            this.clusterListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.clusterListBox.Font = new System.Drawing.Font("Microsoft Tai Le", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clusterListBox.FormattingEnabled = true;
            this.clusterListBox.Location = new System.Drawing.Point(52, 63);
            this.clusterListBox.Name = "clusterListBox";
            this.clusterListBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.clusterListBox.Size = new System.Drawing.Size(120, 95);
            this.clusterListBox.TabIndex = 0;
            this.clusterListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.clusterListBox_DrawItem);
            this.clusterListBox.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.clusterListBox_MeasureItem);
            this.clusterListBox.SelectedIndexChanged += new System.EventHandler(this.clusterListBox_SelectedIndexChanged);
            this.clusterListBox.MouseEnter += new System.EventHandler(this.clusterListBox_MouseEnter);
            // 
            // emailListBox
            // 
            this.emailListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.emailListBox.FormattingEnabled = true;
            this.emailListBox.Location = new System.Drawing.Point(110, 135);
            this.emailListBox.Name = "emailListBox";
            this.emailListBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.emailListBox.Size = new System.Drawing.Size(120, 95);
            this.emailListBox.TabIndex = 1;
            this.emailListBox.Visible = false;
            this.emailListBox.Click += new System.EventHandler(this.emailListBox_Click);
            this.emailListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.emailListBox_DrawItem);
            this.emailListBox.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.emailListBox_MeasureItem);
            this.emailListBox.SelectedIndexChanged += new System.EventHandler(this.emailListBox_SelectedIndexChanged);
            this.emailListBox.MouseEnter += new System.EventHandler(this.emailListBox_MouseEnter);
            // 
            // SmallBangForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.emailListBox);
            this.Controls.Add(this.clusterListBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SmallBangForm";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SmallBangForm_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox clusterListBox;
        private System.Windows.Forms.ListBox emailListBox;
    }
}

