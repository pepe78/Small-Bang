using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Specialized;
using System.Web.Configuration;
using System.Windows.Forms;

namespace SmallBang
{
    public class EmailsFromMicrosoftGraph
    {
        object lock_obj = new object();

        public string currentUser;
        List<Email> allEmails;
        List<Email> newEmails;
        HashSet<string> alreadyProcessedEmails;

        string accessToken;
        string refreshToken;
        string code;
        Timer timer;

        public EmailsFromMicrosoftGraph()
        {
            allEmails = new List<Email>();

            OfficeLogin ol = new OfficeLogin();
            ol.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ol.webBrowser1.Navigate("https://login.microsoftonline.com/common/oauth2/v2.0/authorize?client_id=c8a2c8b2-9113-4385-97d0-83c2d6b6a956&response_type=code&redirect_uri=https%3A%2F%2Flogin.microsoftonline.com%2Fcommon%2Foauth2%2Fnativeclient&response_mode=fragment&state=12345&nonce=678910&scope=offline_access%20https%3A%2F%2Fgraph.microsoft.com%2Fmail.read%20https%3A%2F%2Fgraph.microsoft.com%2Fmail.readwrite");
            ol.ShowDialog();
            code = ol.code;

            int exp_in = 0;
            lock (lock_obj)
            {
                exp_in = GetAccessToken();
            }

            timer = new Timer();
            timer.Interval = exp_in * 1000 / 2;
            timer.Enabled = true;
            timer.Tick += timer_Tick;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Enabled = false;

            int exp_in = 0;
            lock (lock_obj)
            {
                exp_in = RefreshToken();
            }
            timer = new Timer();
            timer.Interval = exp_in * 1000 / 2;
            timer.Enabled = true;
            timer.Tick += timer_Tick;
        }

        private int GetAccessToken()
        {
            DObject o = GetTokens("https://login.microsoftonline.com/common/oauth2/v2.0/token?",
                "client_id=c8a2c8b2-9113-4385-97d0-83c2d6b6a956&scope=offline_access%20https%3A%2F%2Fgraph.microsoft.com%2Fmail.read%20https%3A%2F%2Fgraph.microsoft.com%2Fmail.readwrite&redirect_uri=https%3A%2F%2Flogin.microsoftonline.com%2Fcommon%2Foauth2%2Fnativeclient&grant_type=authorization_code&code="
                + code);
            accessToken = o["access_token"].ToString();
            refreshToken = o["refresh_token"].ToString();

            return o["expires_in"].GetInt();
        }

        private int RefreshToken()
        {
            DObject o = GetTokens("https://login.microsoftonline.com/common/oauth2/v2.0/token?",
                "client_id=c8a2c8b2-9113-4385-97d0-83c2d6b6a956&scope=offline_access%20https%3A%2F%2Fgraph.microsoft.com%2Fmail.read%20https%3A%2F%2Fgraph.microsoft.com%2Fmail.readwrite&redirect_uri=https%3A%2F%2Flogin.microsoftonline.com%2Fcommon%2Foauth2%2Fnativeclient&grant_type=refresh_token&refresh_token="
                + refreshToken);
            accessToken = o["access_token"].ToString();
            refreshToken = o["refresh_token"].ToString();

            return o["expires_in"].GetInt();
        }

        private DObject GetTokens(string requestUrl, string postData)
        {
            NameValueCollection appSettings = WebConfigurationManager.AppSettings;

            byte[] postDataEncoded = System.Text.Encoding.UTF8.GetBytes(postData);

            WebRequest req = HttpWebRequest.Create(requestUrl);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = postDataEncoded.Length;

            Stream requestStream = req.GetRequestStream();
            requestStream.Write(postDataEncoded, 0, postDataEncoded.Length);

            WebResponse res = req.GetResponse();

            string responseBody = null;

            using (StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.UTF8))
            {
                responseBody = sr.ReadToEnd();
            }

            return Deserializer.Deserialize(responseBody);
        }

        public List<Email> GetAllEmails()
        {
            DateTime curMinusMonth = DateTime.Now.Subtract(new TimeSpan(31, 0, 0, 0));
            for(int i=0;i<allEmails.Count;i++)
            {
                if(allEmails[i].emailStamp < curMinusMonth)
                {
                    allEmails.RemoveAt(i);
                    i--;
                }
            }
            return allEmails;
        }

        public List<Email> GetNewEmails()
        {
            newEmails = new List<Email>();
            if (alreadyProcessedEmails == null)
            {
                alreadyProcessedEmails = new HashSet<string>();
            }
            GetEmailsInner();

            return newEmails;
        }

        private void GetEmailsInner()
        {
            lock (lock_obj)
            {
                DateTime ct = DateTime.Now;
                var myUri = new Uri("https://graph.microsoft.com/v1.0/me/messages?$orderby=sentDateTime%20desc");

                while (true)
                {
                    var myWebRequest = WebRequest.Create(myUri);
                    var myHttpWebRequest = (HttpWebRequest)myWebRequest;
                    myHttpWebRequest.PreAuthenticate = true;
                    myHttpWebRequest.Headers.Add("Authorization", "Bearer " + accessToken);
                    myHttpWebRequest.Accept = "application/json";

                    var myWebResponse = myWebRequest.GetResponse();
                    var responseStream = myWebResponse.GetResponseStream();

                    var myStreamReader = new StreamReader(responseStream, Encoding.Default);
                    var json = myStreamReader.ReadToEnd();

                    DObject o = Deserializer.Deserialize(json);

                    if (currentUser == null)
                    {
                        string _cu = "#users('";
                        currentUser = o["@odata.context"].ToString();
                        currentUser = currentUser.Substring(currentUser.IndexOf(_cu) + _cu.Length);
                        currentUser = currentUser.Substring(0, currentUser.IndexOf("'"));
                        currentUser = currentUser.Replace("%40", "@").ToLower();
                    }
                    responseStream.Close();
                    myWebResponse.Close();

                    Email e = null;
                    foreach (DObject oo in o["value"])
                    {
                        try
                        {
                            e = new Email(oo, currentUser);
                            if (e.emailStamp < ct.Subtract(new TimeSpan(31, 0, 0, 0)) ||
                                alreadyProcessedEmails.Contains(e.emailId))
                            {
                                return;
                            }
                            newEmails.Add(e);
                            allEmails.Add(e);
                            alreadyProcessedEmails.Add(e.emailId);
                        }
                        catch
                        {
                        }
                    }

                    DObject next;
                    if (o.TryGetValue("@odata.nextLink", out next))
                    {
                        myUri = new Uri(next.ToString());
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}
