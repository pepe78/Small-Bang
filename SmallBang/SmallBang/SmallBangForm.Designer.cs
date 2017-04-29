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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // clusterListBox
            // 
            this.clusterListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.clusterListBox.Font = new System.Drawing.Font("Microsoft Tai Le", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clusterListBox.FormattingEnabled = true;
            this.clusterListBox.Location = new System.Drawing.Point(27, 122);
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
            this.emailListBox.Location = new System.Drawing.Point(369, 122);
            this.emailListBox.Name = "emailListBox";
            this.emailListBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.emailListBox.Size = new System.Drawing.Size(120, 95);
            this.emailListBox.TabIndex = 1;
            this.emailListBox.Visible = false;
            this.emailListBox.Click += new System.EventHandler(this.emailListBox_Click);
            this.emailListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.emailListBox_DrawItem);
            this.emailListBox.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.emailListBox_MeasureItem);
            this.emailListBox.MouseEnter += new System.EventHandler(this.emailListBox_MouseEnter);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::SmallBang.Properties.Resources.small_bang;
            this.pictureBox1.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.InitialImage")));
            this.pictureBox1.Location = new System.Drawing.Point(27, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(300, 30);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // button1
            // 
            this.button1.Image = global::SmallBang.Properties.Resources.sb_mark_read;
            this.button1.Location = new System.Drawing.Point(333, 66);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(300, 30);
            this.button1.TabIndex = 4;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(27, 79);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(300, 20);
            this.textBox1.TabIndex = 5;
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // SmallBangForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(653, 261);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.emailListBox);
            this.Controls.Add(this.clusterListBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SmallBangForm";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SmallBangForm_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox clusterListBox;
        private System.Windows.Forms.ListBox emailListBox;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
    }
}

