using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmallBang
{
    public partial class SmallBangForm : Form
    {
        object lockObj = new object();

        bool shown = true;
        Timer timerShowWindow;
        Timer timerNewEmails;
        Timer timerReCluster;
        ClusterCollection cc;
        EmailsFromMicrosoftGraph efmg;
        Cluster selectedCluster = null;

        public SmallBangForm(EmailsFromMicrosoftGraph _efmg)
        {
            InitializeComponent();
            efmg = _efmg;
            List<Email> emails = efmg.GetNewEmails();
            cc = new ClusterCollection(emails, efmg.currentUser);
            Reorder();
        }

        private void Reorder()
        {
            lock (lockObj)
            {
                cc.clusters = cc.clusters.OrderBy(
                    o => -(double)(o.new_emails + 0.0) / (o.people.Count + 1.0) * (o.sent_emails + 1.0))
                    .ThenBy(o => -o.sent_emails * o.people.Count)
                    .ThenBy(o => -o.people.Count).ToList();
                clusterListBox.Items.Clear();
                foreach (var c in cc.clusters)
                {
                    clusterListBox.Items.Add(c);
                }

                for(int i=0;i<cc.clusters.Count;i++)
                {
                    if(cc.clusters[i] == selectedCluster)
                    {
                        clusterListBox.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Left = 0;
            pictureBox1.Top = 0;

            textBox1.Width = 300;
            textBox1.Left = 0;
            textBox1.Top = pictureBox1.Height;

            Location = new Point(0, 0);
            Size = new Size(300, Screen.PrimaryScreen.Bounds.Height);
            clusterListBox.Left = 0;
            clusterListBox.Top = pictureBox1.Height + textBox1.Height;
            clusterListBox.Width = Size.Width;
            clusterListBox.Height = Size.Height;

            timerShowWindow = new Timer();
            timerShowWindow.Interval = 100;
            timerShowWindow.Tick += new EventHandler(timerShowWindow_Tick);
            timerShowWindow.Enabled = true;

            timerNewEmails = new Timer();
            timerNewEmails.Interval = 60 * 1000;
            timerNewEmails.Tick += new EventHandler(timerNewEmails_Tick);
            timerNewEmails.Enabled = true;

            timerReCluster = new Timer();
            timerReCluster.Interval = 60 * 60 * 1000;
            timerReCluster.Tick += new EventHandler(timerReCluster_Tick);
            timerReCluster.Enabled = true;

            this.TopMost = true;
        }

        private void timerReCluster_Tick(object sender, EventArgs e)
        {
            ClusterCollection ccNew = null;
            Task x = Task.Run(() =>
                {
                    ccNew = new ClusterCollection(efmg.GetAllEmails(), efmg.currentUser);
                });
            x.Wait();

            lock (lockObj)
            {
                cc = ccNew;
            }
            Reorder();
        }

        private void timerNewEmails_Tick(object sender, EventArgs e)
        {
            lock(lockObj)
            { 
                cc.InsertNewEmails(efmg.GetNewEmails());
            }
            Reorder();
        }

        private void timerShowWindow_Tick(object sender, EventArgs e)
        {
            Point p = Cursor.Position;
            if (shown)
            {
                if (!(p.X >= -20 && p.X <= this.Size.Width))
                {
                    Hide();
                    shown = false;
                    clusterListBox.SelectedIndex = -1;
                }
            }
            else
            {
                if (p.X >= -20 && p.X <= 10)
                {
                    shown = true;
                    Width = 300;
                    clusterListBox.SelectedIndex = -1;
                    Location = new Point(0, 0);
                    Refresh();
                    Show();
                    clusterListBox.Focus();
                }
            }
        }

        private void clusterListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index != -1)
            {
                ListBox listBox = sender as ListBox;
                Cluster cluster = listBox.Items[e.Index] as Cluster;

                e.DrawBackground();
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(219, 231, 249)), e.Bounds);
                }
                else
                {
                    e.Graphics.FillRectangle(Brushes.White, e.Bounds);
                }
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(219, 231, 249)), e.Bounds);
                cluster.DrawItem(e);
            }
        }

        private void clusterListBox_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = 40;
        }

        private void clusterListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(clusterListBox.SelectedIndex != -1)
            {
                selectedCluster = clusterListBox.Items[clusterListBox.SelectedIndex] as Cluster;
                RedrawCluster();
            }
        }

        private void RedrawCluster()
        {
            lock (lockObj)
            {
                emailListBox.Items.Clear();
                for (int i = 0; i < selectedCluster.emails.Count; i++)
                {
                    emailListBox.Items.Add(selectedCluster.emails[i]);
                }
            }
            Width = 600;
            button1.Top = 0;
            button1.Left = 300;
            emailListBox.Left = 300;
            emailListBox.Top = button1.Height;
            emailListBox.Width = 300;
            emailListBox.Height = this.clusterListBox.Height;
            emailListBox.Visible = true;
        }

        private void emailListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index != -1)
            {
                ListBox listBox = sender as ListBox;
                Email email = listBox.Items[e.Index] as Email;

                e.DrawBackground();
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(219, 231, 249)), e.Bounds);
                }
                else
                {
                    e.Graphics.FillRectangle(Brushes.White, e.Bounds);
                }
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(219, 231, 249)), e.Bounds);
                email.DrawItem(e);
            }
        }

        private void emailListBox_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = 45;
        }

        private void clusterListBox_MouseEnter(object sender, EventArgs e)
        {
            if (!clusterListBox.Focused)
            {
                clusterListBox.Focus();
            }
        }

        private void emailListBox_MouseEnter(object sender, EventArgs e)
        {
            if (!emailListBox.Focused)
            {
                emailListBox.Focus();
            }
        }

        private void emailListBox_Click(object sender, EventArgs e)
        {
            if (emailListBox.SelectedIndex != -1)
            {
                Email em = emailListBox.Items[emailListBox.SelectedIndex] as Email;
                em.isRead = true;
                Hide();
                shown = false;
                System.Diagnostics.Process.Start(em.emailLink);
                selectedCluster.Recount();
                Reorder();
            }
        }

        private void SmallBangForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            timerShowWindow.Stop();
            timerNewEmails.Stop();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lock (lockObj)
            {
                selectedCluster.MarkEmailsAsRead(efmg);
                Reorder();
                emailListBox.Focus();
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == 13 && textBox1.Text.Trim().Length != 0)
            {
                selectedCluster = efmg.SearchEmails(textBox1.Text.Trim());
                RedrawCluster();
            }
        }
    }
}
