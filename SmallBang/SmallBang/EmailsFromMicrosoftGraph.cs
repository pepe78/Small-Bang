using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Specialized;
using System.Web.Configuration;

namespace SmallBang
{
    public class EmailsFromMicrosoftGraph
    {
        public string currentUser;
        public List<Email> emails;

        string accessToken;
        string refreshToken;
        string code;

        public EmailsFromMicrosoftGraph()
        {
            OfficeLogin ol = new OfficeLogin();
            ol.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ol.webBrowser1.Navigate("https://login.microsoftonline.com/common/oauth2/v2.0/authorize?client_id=c8a2c8b2-9113-4385-97d0-83c2d6b6a956&response_type=code&redirect_uri=https%3A%2F%2Flogin.microsoftonline.com%2Fcommon%2Foauth2%2Fnativeclient&response_mode=fragment&state=12345&nonce=678910&scope=offline_access%20https%3A%2F%2Fgraph.microsoft.com%2Fmail.read");
            ol.ShowDialog();
            code = ol.code;
            GetAccessToken();
            RefreshToken();
        }

        private void GetAccessToken()
        {
            DObject o = GetTokens("https://login.microsoftonline.com/common/oauth2/v2.0/token?",
                "client_id=c8a2c8b2-9113-4385-97d0-83c2d6b6a956&scope=offline_access%20https%3A%2F%2Fgraph.microsoft.com%2Fmail.read&redirect_uri=https%3A%2F%2Flogin.microsoftonline.com%2Fcommon%2Foauth2%2Fnativeclient&grant_type=authorization_code&code="
                + code);
            accessToken = o["access_token"].ToString();
            refreshToken = o["refresh_token"].ToString();
        }

        private void RefreshToken()
        {
            DObject o = GetTokens("https://login.microsoftonline.com/common/oauth2/v2.0/token?",
                "client_id=c8a2c8b2-9113-4385-97d0-83c2d6b6a956&scope=offline_access%20https%3A%2F%2Fgraph.microsoft.com%2Fmail.read&redirect_uri=https%3A%2F%2Flogin.microsoftonline.com%2Fcommon%2Foauth2%2Fnativeclient&grant_type=refresh_token&refresh_token="
                + refreshToken);
            accessToken = o["access_token"].ToString();
            refreshToken = o["refresh_token"].ToString();
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

        public List<Email> GetEmails()
        {
            var myUri = new Uri("https://graph.microsoft.com/v1.0/me/messages?$orderby=sentDateTime%20desc");
            emails = new List<Email>();

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

                foreach (DObject oo in o["value"])
                {
                    try
                    {
                        Email e;
                        e = new Email(oo, currentUser);
                        emails.Add(e);
                    }
                    catch
                    {
                    }
                }
                responseStream.Close();
                myWebResponse.Close();

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

            return emails;
        }
    }
}
