using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace SmallBang
{
    public class ClusterCollection
    {
        public List<Cluster> clusters;
        public List<Email> all_emails;
        public Dictionary<string, int> personToFeatureId;
        string current_user;
        public int[] countsAll;

        private void ProcessEmail(string ea)
        {
            if(!personToFeatureId.ContainsKey(ea) && current_user.CompareTo(current_user) != 0)
            {
                personToFeatureId[ea] = personToFeatureId.Count;
            }
        }

        public ClusterCollection(List<Email> _emails, string _current_user)
        {
            current_user = _current_user;
            personToFeatureId = new Dictionary<string, int>();
            all_emails = _emails;
            all_emails = all_emails.OrderBy(o => -o.emailStamp.Ticks).ToList();
            foreach (var e in all_emails)
            {
                ProcessEmail(e.emailFrom);
                foreach (var ea in e.emailTo)
                {
                    ProcessEmail(ea);
                }
                foreach (var ea in e.emailCc)
                {
                    ProcessEmail(ea);
                }
            }
            SetUserDictionary();
            DoClustering();

            //DisplayAsBitmap();
        }

        private void DisplayAsBitmap()
        {
            Bitmap b = new Bitmap(clusters[0].emails[0].userVector.Length,
                clusters.Count + all_emails.Count);
            int pos = 0;
            for (int i = 0; i < clusters.Count; i++)
            {
                for (int j = 0; j < clusters[i].emails.Count; j++)
                {
                    for (int k = 0; k < clusters[i].emails[j].userVector.Length; k++)
                    {
                        if (clusters[i].emails[j].userVector[k])
                        {
                            b.SetPixel(k, pos, Color.Red);
                        }
                        else
                        {
                            b.SetPixel(k, pos, Color.Blue);
                        }
                    }
                    pos++;
                }
                for (int k = 0; k < clusters[i].emails[0].userVector.Length; k++)
                {
                    b.SetPixel(k, pos, Color.Green);
                }
                pos++;
            }
            b.Save("tmp.png");
            System.Diagnostics.Process.Start("tmp.png");
        }

        private void SetUserDictionary()
        {
            for (int i = 0; i < all_emails.Count; i++)
            {
                InsertNewUser(all_emails[i].emailFrom);
                foreach (var u in all_emails[i].emailTo)
                {
                    InsertNewUser(u);
                }
                foreach (var u in all_emails[i].emailCc)
                {
                    InsertNewUser(u);
                }
            }
        }

        private void DoClustering()
        {
            SetUserVectorsAndCounts();
            ConstructSameVectorsClusters();
            DoPOPCClustering();

            for (int i = 0; i < clusters.Count; i++)
            {
                clusters[i].Process(current_user);
            }
        }

        private void DoPOPCClustering()
        {
            bool changed = true;
            while (changed)
            {
                changed = false;
                for (int i = 0; i < clusters.Count; i++)
                {
                    for (int j = 0; j < clusters[i].emails.Count; j++)
                    {
                        int largestGainWhere = -1;
                        double largestGain = double.MinValue;
                        double deltaBase = clusters[i].deltaIfRemoved(j, countsAll);
                        for (int k = 0; k < clusters.Count; k++)
                        {
                            if (i != k)
                            {
                                double delta = deltaBase + clusters[k].deltaIfAdded(
                                    clusters[i].emails[j], countsAll);
                                if (delta > largestGain)
                                {
                                    largestGain = delta;
                                    largestGainWhere = k;
                                }
                            }
                        }
                        if (largestGain > 0)
                        {
                            Email e = clusters[i].emails[j];
                            clusters[i].RemoveEmail(j);
                            clusters[largestGainWhere].AddEmail(e);
                            j--;
                            changed = true;
                        }
                    }
                    if (clusters[i].emails.Count == 0)
                    {
                        clusters.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        private void ConstructSameVectorsClusters()
        {
            clusters = new List<Cluster>();
            for (int i = 0; i < all_emails.Count; i++)
            {
                int j = 0;
                for (; j < clusters.Count; j++)
                {
                    if (sameVectors(all_emails[i].userVector, clusters[j].emails[0].userVector))
                    {
                        break;
                    }
                }
                if (j == clusters.Count)
                {
                    Cluster cl = new Cluster(personToFeatureId.Count);
                    cl.AddEmail(all_emails[i]);
                    clusters.Add(cl);
                }
                else
                {
                    clusters[j].AddEmail(all_emails[i]);
                }
            }
        }

        private void SetUserVectorsAndCounts()
        {
            countsAll = new int[personToFeatureId.Count];
            for (int i = 0; i < all_emails.Count; i++)
            {
                all_emails[i].SetUserVector(personToFeatureId);
                for (int j = 0; j < personToFeatureId.Count; j++)
                {
                    if (all_emails[i].userVector[j])
                    {
                        countsAll[j]++;
                    }
                }
            }
        }

        private bool sameVectors(bool[] v1, bool[] v2)
        {
            for(int i=0;i<v1.Length;i++)
            {
                if(v1[i]!=v2[i])
                {
                    return false;
                }
            }
            return true;
        }

        private void InsertNewUser(string user)
        {
            if(!personToFeatureId.ContainsKey(user) && user.CompareTo(current_user) != 0)
            {
                personToFeatureId[user] = personToFeatureId.Count;
            }
        }

    }
}
