using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System;

namespace SmallBang
{
    public class Cluster
    {
        public List<Email> emails;

        Dictionary<string, int> companies;
        public Dictionary<string, int> people;

        public int new_emails;
        public int sent_emails;

        int[] counts;

        public Cluster(int amountOfPeople)
        {
            emails = new List<Email>();
            counts = new int[amountOfPeople];
        }

        public void MarkEmailsAsRead(EmailsFromMicrosoftGraph efmg)
        {
            foreach(var e in emails)
            {
                if(!e.isRead)
                {
                    efmg.SetEmailToRead(e.emailId);
                }
                e.isRead = true;
            }
            new_emails = 0;
        }

        public void AddEmail(Email e)
        {
            emails.Insert(0, e);
            for (int i = 0; i < counts.Length; i++)
            {
                if (e.userVector[i])
                {
                    counts[i]++;
                }
            }
            if(!e.isRead)
            {
                new_emails++;
            }
            if(e.isSent)
            {
                sent_emails++;
            }
        }

        public void RemoveEmail(int which)
        {
            Email e = emails[which];
            emails.RemoveAt(which);
            for(int i=0;i<counts.Length;i++)
            {
                if(e.userVector[i])
                {
                    counts[i]--;
                }
            }
        }

        public double deltaIfRemoved(int which, int[] countsAll)
        {
            double ret = 0.0;
            for(int i=0;i<counts.Length;i++)
            {
                if(emails[which].userVector[i])
                {
                    ret -= Math.Pow((double)(counts[i] + 0.0) / (countsAll[i] + 0.0), 10.0);
                    ret += Math.Pow((double)(counts[i] - 1.0) / (countsAll[i] + 0.0), 10.0);
                }
            }

            return ret;
        }

        public double deltaIfAdded(Email e, int[] countsAll)
        {
            double ret = 0.0;
            for (int i = 0; i < counts.Length; i++)
            {
                if (e.userVector[i])
                {
                    ret += Math.Pow((double)(counts[i] + 1.0) / (countsAll[i] + 0.0), 10.0);
                    ret -= Math.Pow((double)(counts[i] + 0.0) / (countsAll[i] + 0.0), 10.0);
                }
            }

            return ret;
        }

        public void Process(string currentUser)
        {
            emails = emails.OrderBy(o => -o.emailStamp.Ticks).ToList();

            Recount();

            people = new Dictionary<string, int>();
            companies = new Dictionary<string, int>();
            foreach (var e in emails)
            {
                processEmailAddress(e.emailFrom, currentUser);
                foreach (var ea in e.emailTo)
                {
                    processEmailAddress(ea, currentUser);
                }
                foreach (var ea in e.emailCc)
                {
                    processEmailAddress(ea, currentUser);
                }
            }
        }

        public void Recount()
        {
            new_emails = 0;
            sent_emails = 0;
            foreach (var e in emails)
            {
                if (!e.isRead)
                {
                    new_emails++;
                }
                if (e.isSent)
                {
                    sent_emails++;
                }
            }
        }

        private void processEmailAddress(string emailAddress, string currentUser)
        {
            if(emailAddress == currentUser)
            {
                return;
            }
            string[] parts = emailAddress.Split('@');
            string company = parts[1].Substring(0, parts[1].LastIndexOf("."));
            if(!companies.ContainsKey(company))
            {
                companies[company] = 0;
            }
            companies[company]++;
            if (!people.ContainsKey(parts[0]))
            {
                people[parts[0]] = 0;
            }
            people[parts[0]]++;
        }

        private string getClusterName()
        {
            var x = companies.ToList();
            x = x.OrderBy(o => -o.Value).ToList();
            string ret = "";
            foreach(var c in x)
            {
                ret += c.Key;
                ret += ", ";
            }

            return ret;
        }

        private string getPeople()
        {
            var x = people.ToList();
            x = x.OrderBy(o => -o.Value).ToList();
            string ret = "";
            foreach (var c in x)
            {
                ret += c.Key;
                ret += ", ";
            }

            return ret;
        }

        public void DrawItem(DrawItemEventArgs e)
        {
            int margin = 2;
            int numberWidth = 30;

            Rectangle layoutRect = new Rectangle(
                margin, e.Bounds.Top + margin, 
                e.Bounds.Width - 2 * margin - numberWidth, e.Bounds.Height / 2 - margin);
            e.Graphics.DrawString(
                getClusterName(), 
                new Font("Georgia", 10.0F, FontStyle.Bold),
                new SolidBrush(Color.FromArgb(23, 67, 137)), 
                layoutRect, new StringFormat());

            layoutRect = new Rectangle(
                margin, e.Bounds.Top + margin + 20,
                e.Bounds.Width - 2 * margin - numberWidth, e.Bounds.Height * 4 / 9 - margin);
            e.Graphics.DrawString(
                getPeople(),
                new Font("Georgia", 8.0F, FontStyle.Regular),
                new SolidBrush(Color.FromArgb(23, 67, 137)),
                layoutRect, new StringFormat());

            if (new_emails != 0)
            {
                layoutRect = new Rectangle(
                    e.Bounds.Width - margin - numberWidth, e.Bounds.Top + margin,
                    numberWidth, e.Bounds.Height - margin);
                e.Graphics.DrawString(
                    new_emails.ToString(),
                    new Font("Georgia", 10.0F, FontStyle.Bold),
                    new SolidBrush(Color.FromArgb(204, 26, 82)),
                    layoutRect, new StringFormat());
            }

            layoutRect = new Rectangle(
                e.Bounds.Width - margin - numberWidth, e.Bounds.Top + margin + 20,
                numberWidth, e.Bounds.Height - margin - 20);
            e.Graphics.DrawString(
                emails.Count.ToString(),
                new Font("Georgia", 8.0F, FontStyle.Regular),
                new SolidBrush(Color.FromArgb(23, 67, 137)),
                layoutRect, new StringFormat());
        }
    }
}
