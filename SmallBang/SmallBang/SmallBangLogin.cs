using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace SmallBang
{
    public partial class SmallBangLogin : Form
    {
        ThreadHelper th;
        Thread t;
        System.Windows.Forms.Timer t1;

        public SmallBangLogin()
        {
            InitializeComponent();
        }

        private void StartGrabbing()
        {
            t1 = new System.Windows.Forms.Timer();
            t1.Interval = 1000;
            t1.Tick += new System.EventHandler(done);
            t1.Enabled = true;

            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 1;
            t = new Thread(new ParameterizedThreadStart(ThreadStart));
            t.Start(th);
        }

        public void done(object sender, EventArgs e)
        {
            if (t != null && !t.IsAlive)
            {
                t.Join();
                Hide();
                th.sbf.Show();
                t = null;
            }
            if(th != null && th.sbf != null && th.sbf.IsDisposed)
            {
                Close();
            }
        }

        private void ThreadStart(object obj)
        {
            ThreadHelper th = obj as ThreadHelper;
            th.sbf = new SmallBangForm(th.ef);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            th = new ThreadHelper();
            th.ef = new EmailsFromMicrosoftGraph();

            StartGrabbing();
        }
    }

    public class ThreadHelper
    {
        public EmailsFromMicrosoftGraph ef;
        public SmallBangForm sbf;
    }
}
