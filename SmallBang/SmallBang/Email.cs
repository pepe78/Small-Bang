using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SmallBang
{
    public class Email
    {
        public string emailId;
        public string emailSubject;
        public string emailFrom;
        public List<string> emailTo;
        public List<string> emailCc;
        public DateTime emailStamp;
        public bool isSent;
        public bool isRead;
        public string emailLink;

        public bool[] userVector;

        public void SetUserVector(Dictionary<string, int> userToId)
        {
            userVector = new bool[userToId.Count];
            SetUserActive(emailFrom, userToId);
            foreach(var u in emailTo)
            {
                SetUserActive(u, userToId);
            }
            foreach (var u in emailCc)
            {
                SetUserActive(u, userToId);
            }
        }

        private void SetUserActive(string user, Dictionary<string, int> userToId)
        {
            if(userToId.ContainsKey(user))
            {
                userVector[userToId[user]] = true;
            }
        }

        public Email(DObject obj, string currentUser)
        {
            emailId = obj["id"].ToString();
            emailSubject = obj["subject"].ToString();
            emailFrom = obj["sender"]["emailAddress"]["address"].ToString().ToLower();
            emailTo = new List<string>();
            foreach(DObject et in obj["toRecipients"])
            {
                emailTo.Add(et["emailAddress"]["address"].ToString().ToLower());
            }
            emailCc = new List<string>();
            foreach (DObject et in obj["ccRecipients"])
            {
                emailTo.Add(et["emailAddress"]["address"].ToString().ToLower());
            }
            emailStamp = DateTime.Parse(
                            obj["sentDateTime"].ToString());

            if(emailFrom.CompareTo(currentUser)==0)
            {
                isSent = true;
            }
            isRead = obj["isRead"].getBool();
            emailLink = obj["webLink"].ToString();
        }

        public void DrawItem(DrawItemEventArgs e)
        {
            int margin = 2;
            int numberWidth = 35;

            if(!isRead)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(255, 210, 210)), e.Bounds);
            }
            if (isSent)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(215, 255, 201)), e.Bounds);
            }
            Rectangle layoutRect = new Rectangle(
                margin, e.Bounds.Top + margin,
                e.Bounds.Width - 2 * margin - numberWidth, e.Bounds.Height / 3);
            e.Graphics.DrawString(
                emailSubject,
                new Font("Times New Roman", 10.0F, FontStyle.Bold),
                new SolidBrush(Color.FromArgb(23, 67, 137)),
                layoutRect, new StringFormat());

            layoutRect = new Rectangle(
                margin, e.Bounds.Top + margin + 15,
                e.Bounds.Width - 2 * margin - numberWidth, e.Bounds.Height / 3);
            e.Graphics.DrawString(
                emailFrom,
                new Font("Times New Roman", 8.0F, FontStyle.Regular),
                new SolidBrush(Color.FromArgb(23, 67, 137)),
                layoutRect, new StringFormat());

            layoutRect = new Rectangle(
                margin, e.Bounds.Top + margin + 28,
                e.Bounds.Width - 2 * margin - numberWidth, e.Bounds.Height / 3);
            e.Graphics.DrawString(
                getEmailTo(),
                new Font("Times New Roman", 8.0F, FontStyle.Regular),
                new SolidBrush(Color.FromArgb(23, 67, 137)),
                layoutRect, new StringFormat());


            layoutRect = new Rectangle(
                e.Bounds.Width - margin - numberWidth, e.Bounds.Top + margin,
                numberWidth, e.Bounds.Height);
            e.Graphics.DrawString(
                emailStamp.ToString("MM/dd yyyy HH:mm"),
                new Font("Times New Roman", 8.0F, FontStyle.Regular),
                new SolidBrush(Color.FromArgb(23, 67, 137)),
                layoutRect, new StringFormat());
        }

        private string getEmailTo()
        {
            string ret = "";
            for (int i=0;i<emailTo.Count;i++)
            {
                ret += emailTo[i].Substring(0, emailTo[i].IndexOf("@"));
                if (i != emailTo.Count - 1)
                {
                    ret += ", ";
                }
            }
            return ret;
        }
    }
}
