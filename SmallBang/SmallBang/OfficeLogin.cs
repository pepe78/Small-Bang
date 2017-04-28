using System.Windows.Forms;

namespace SmallBang
{
    public partial class OfficeLogin : Form
    {
        public string code;

        public OfficeLogin()
        {
            InitializeComponent();
        }

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            string url = "https://login.microsoftonline.com/common/oauth2/nativeclient#code=";
            if (webBrowser1.Document.Url.ToString().StartsWith(url))
            {
                code = webBrowser1.Document.Url.ToString().Substring(url.Length);
                code = code.Substring(0, code.IndexOf("&"));
                this.Close();
            }
        }
    }
}
