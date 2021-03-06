﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace SmallBang
{
    public class EmailsFromMicrosoftGraph
    {
        static object lock_obj = new object();
        static Semaphore semaphore = new Semaphore(1, 1);

        public string currentUser;
        List<Email> allEmails;
        List<Email> newEmails;
        HashSet<string> alreadyProcessedEmails;

        string accessToken;
        string refreshToken;
        string code;
        System.Windows.Forms.Timer timer;

        string clientId = "c8a2c8b2-9113-4385-97d0-83c2d6b6a956";
        string scope = "offline_access%20https%3A%2F%2Fgraph.microsoft.com%2Fmail.read%20https%3A%2F%2Fgraph.microsoft.com%2Fmail.readwrite";
        string redirectUri = "https%3A%2F%2Flogin.microsoftonline.com%2Fcommon%2Foauth2%2Fnativeclient";
        string graphUriBase = "https://login.microsoftonline.com/common/oauth2/v2.0/";
        string messagesBaseUri = "https://graph.microsoft.com/v1.0/me/messages";

        public EmailsFromMicrosoftGraph()
        {
            allEmails = new List<Email>();

            int exp_in = RelogIn();

            timer = new System.Windows.Forms.Timer();
            timer.Interval = exp_in * 1000 / 2;
            timer.Enabled = true;
            timer.Tick += timer_Tick;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Enabled = false;

            int exp_in = 0;
            exp_in = RefreshToken();
            timer = new System.Windows.Forms.Timer();
            timer.Interval = exp_in * 1000 / 2;
            timer.Enabled = true;
            timer.Tick += timer_Tick;
        }

        private int GetAccessToken()
        {
            DObject o = WebRequestToken(
                graphUriBase + 
                "token?",
                "client_id=" + 
                clientId + 
                "&scope=" + 
                scope + 
                "&redirect_uri=" + 
                redirectUri + 
                "&grant_type=authorization_code&code=" + 
                code);
            accessToken = o["access_token"].ToString();
            refreshToken = o["refresh_token"].ToString();

            return o["expires_in"].GetInt();
        }

        private int RefreshToken()
        {
            DObject o = WebRequestToken(
                graphUriBase + 
                "token?",
                "client_id=" + 
                clientId + 
                "&scope=" + 
                scope + 
                "&redirect_uri=" +
                redirectUri + 
                "&grant_type=refresh_token&refresh_token="
                + refreshToken);
            accessToken = o["access_token"].ToString();
            refreshToken = o["refresh_token"].ToString();

            return o["expires_in"].GetInt();
        }

        private DObject WebRequestToken(string requestUrl, string postData, 
            string method = "POST", string contentType = "application/x-www-form-urlencoded")
        {
            try
            {
                lock (lock_obj)
                {
                    byte[] postDataEncoded = Encoding.UTF8.GetBytes(postData);

                    WebRequest req = HttpWebRequest.Create(requestUrl);
                    req.Method = method;
                    req.ContentType = contentType;
                    req.ContentLength = postDataEncoded.Length;

                    Stream requestStream = req.GetRequestStream();
                    requestStream.Write(postDataEncoded, 0, postDataEncoded.Length);
                    requestStream.Close();

                    WebResponse res = req.GetResponse();
                    StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
                    string responseBody = sr.ReadToEnd();
                    sr.Close();

                    return Deserializer.Deserialize(responseBody);
                }
            }
            catch
            {
                code = null;
                RelogIn();
                return WebRequestToken(requestUrl, postData, method, contentType);
            }
        }

        public Cluster SearchEmails(string searchTerm)
        {
            Cluster c = new Cluster(0);
            string myUri = messagesBaseUri + "?$search=\"" + searchTerm + "\"";
            while (true)
            {
                DObject d =
                    WebRequestPreAuthenticate(
                        myUri, "", "");

                Email e = null;
                foreach (DObject oo in d["value"])
                {
                    try
                    {
                        e = new Email(oo, currentUser);
                        c.AddEmail(e);
                    }
                    catch
                    {
                    }
                }

                DObject next;
                if (d.TryGetValue("@odata.nextLink", out next))
                {
                    myUri = next.ToString();
                }
                else
                {
                    break;
                }
            }

            return c;
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

        public void SetEmailToRead(string emailId)
        {
            WebRequestPreAuthenticate(
                messagesBaseUri + 
                "/" + 
                emailId, 
                "PATCH", 
                "{\"isRead\":true}");
        }

        private DObject WebRequestPreAuthenticate(string uri, string method, string postcode)
        {
            try
            {
                string json = "";

                lock (lock_obj)
                {
                    Uri myUri = new Uri(uri);

                    WebRequest myWebRequest = WebRequest.Create(myUri);
                    HttpWebRequest myHttpWebRequest = (HttpWebRequest)myWebRequest;
                    if (method.Length != 0)
                    {
                        myHttpWebRequest.Method = method;
                    }
                    myHttpWebRequest.PreAuthenticate = true;
                    myHttpWebRequest.Headers.Add("Authorization", "Bearer " + accessToken);
                    myHttpWebRequest.Accept = "application/json";

                    if (postcode.Length != 0)
                    {
                        byte[] postDataEncoded = Encoding.UTF8.GetBytes(postcode);
                        myHttpWebRequest.ContentType = "application/json";
                        Stream requestStream = myHttpWebRequest.GetRequestStream();
                        requestStream.Write(postDataEncoded, 0, postDataEncoded.Length);
                        requestStream.Close();
                    }

                    WebResponse myWebResponse = myWebRequest.GetResponse();
                    Stream responseStream = myWebResponse.GetResponseStream();

                    StreamReader myStreamReader = new StreamReader(responseStream, Encoding.Default);
                    json = myStreamReader.ReadToEnd();

                    responseStream.Close();
                    myWebResponse.Close();
                }

                return Deserializer.Deserialize(json);
            }
            catch
            {
                code = null;
                RelogIn();
                return WebRequestPreAuthenticate(uri, method, postcode);
            }
        }

        private int RelogIn()
        {
            int ret = 0;

            semaphore.WaitOne();
            if (code == null)
            {
                OfficeLogin ol = new OfficeLogin();
                ol.WindowState = FormWindowState.Maximized;
                ol.webBrowser1.Navigate(
                    graphUriBase +
                    "authorize?client_id=" +
                    clientId +
                    "&response_type=code&redirect_uri=" +
                    redirectUri +
                    "&response_mode=fragment&state=12345&nonce=678910&scope=" +
                    scope);
                ol.BringToFront();
                ol.TopMost = true;
                ol.ShowDialog();
                code = ol.code;
                ret = GetAccessToken();
            }
            semaphore.Release();
            return ret;
        }

        private void GetEmailsInner()
        {
            DateTime ct = DateTime.Now;
            string myUri = messagesBaseUri + "?$orderby=sentDateTime%20desc";

            while (true)
            {
                DObject o = WebRequestPreAuthenticate(myUri, "", "");

                if (currentUser == null)
                {
                    string _cu = "#users('";
                    currentUser = o["@odata.context"].ToString();
                    currentUser = currentUser.Substring(currentUser.IndexOf(_cu) + _cu.Length);
                    currentUser = currentUser.Substring(0, currentUser.IndexOf("'"));
                    currentUser = currentUser.Replace("%40", "@").ToLower();
                }

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
                        alreadyProcessedEmails.Add(e.emailId);
                        newEmails.Add(e);
                        allEmails.Add(e);
                    }
                    catch
                    {
                    }
                }

                DObject next;
                if (o.TryGetValue("@odata.nextLink", out next))
                {
                    myUri = next.ToString();
                }
                else
                {
                    break;
                }
            }
        }
    }
}
