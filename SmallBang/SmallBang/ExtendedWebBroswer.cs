using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmallBang
{
    public class ExtendedWebBrowser : WebBrowser
    {
        // Define constants from winuser.h
        private const int WM_PARENTNOTIFY = 0x210;
        private const int WM_DESTROY = 2;

        public event FormClosingEventHandler Closing;

        static public List<Message> mms = new List<Message>();

        protected override void WndProc(ref Message m)
        {
            mms.Add(m);
            switch (m.Msg)
            {
                case WM_PARENTNOTIFY:
                    if (!DesignMode)
                    {
                        if (m.WParam.ToInt32() == WM_DESTROY)
                        {
                            Closing(this, null);
                        }
                    }
                    DefWndProc(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}
