using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SmallBang
{
    public partial class SmallBangForm : Form
    {
        bool shown = true;
        Timer t1;
        ClusterCollection cc;

        public SmallBangForm(ClusterCollection _cc)
        {
            InitializeComponent();
            cc = _cc;
            Reorder();
        }

        private void Reorder()
        {
            cc.clusters = cc.clusters.OrderBy(o => -o.new_emails).
                ThenBy(o => -o.sent_emails * o.people.Count).
                ThenBy(o => -o.people.Count).ToList();
            clusterListBox.Items.Clear();
            foreach (var c in cc.clusters)
            {
                clusterListBox.Items.Add(c);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Location = new Point(0, 0);
            Size = new Size(300, Screen.PrimaryScreen.Bounds.Height);
            clusterListBox.Left = 0;
            clusterListBox.Top = 0;
            clusterListBox.Width = Size.Width;
            clusterListBox.Height = Size.Height;

            t1 = new Timer();
            t1.Interval = 100;
            t1.Tick += new EventHandler(timer1_Tick);
            t1.Enabled = true;
            this.TopMost = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
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
                if (p.X >= -20 && p.X <= 20)
                {
                    Width = 300;
                    clusterListBox.SelectedIndex = -1;
                    Location = new Point(0, 0);
                    Refresh();
                    Show();
                    clusterListBox.Focus();
                    shown = true;
                }
            }
        }

        private void clusterListBox_DrawItem(object sender, DrawItemEventArgs e)
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

        private void clusterListBox_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = 40;
        }

        private void clusterListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(clusterListBox.SelectedIndex != -1)
            {
                Width = 600;

                emailListBox.Items.Clear();
                Cluster cl = clusterListBox.Items[clusterListBox.SelectedIndex] as Cluster;
                for (int i = 0; i < cl.emails.Count; i++)
                {
                    emailListBox.Items.Add(cl.emails[i]);
                }
                emailListBox.Left = 300;
                emailListBox.Top = 0;
                emailListBox.Width = 300;
                emailListBox.Height = this.clusterListBox.Height;
                emailListBox.Visible = true;
            }
        }

        private void emailListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void emailListBox_DrawItem(object sender, DrawItemEventArgs e)
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

        private void emailListBox_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = 45;
        }

        private void clusterListBox_MouseEnter(object sender, EventArgs e)
        {
            clusterListBox.Focus();
        }

        private void emailListBox_MouseEnter(object sender, EventArgs e)
        {
            emailListBox.Focus();
        }

        private void emailListBox_Click(object sender, EventArgs e)
        {
            Cluster cl = clusterListBox.Items[clusterListBox.SelectedIndex] as Cluster;
            Email em = emailListBox.Items[emailListBox.SelectedIndex] as Email;
            em.isRead = true;
            Hide();
            shown = false;
            System.Diagnostics.Process.Start(em.emailLink);
            cl.Recount();
            Reorder();
        }

        private void SmallBangForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            t1.Stop();
        }
    }
}
