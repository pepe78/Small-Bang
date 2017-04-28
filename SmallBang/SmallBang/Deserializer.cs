using System;
using System.Collections;
using System.Collections.Generic;

namespace SmallBang
{
    public class DObject : IEnumerable<DObject>
    {
        Dictionary<string, DObject> dict = new Dictionary<string, DObject>();
        List<DObject> list = new List<DObject>();
        string str;
        bool b;
        int rn;

        public void SetInt(int _rn)
        {
            rn = _rn;
        }

        public int GetInt()
        {
            return rn;
        }

        public bool getBool()
        {
            return b;
        }

        public void Add(DObject obj)
        {
            list.Add(obj);
        }

        public DObject this[int i]
        {
            get
            {
                return list[i];
            }
        }

        public DObject this[string str]
        {
            get
            {
                return dict[str];
            }
            set
            {
                dict[str] = (DObject)value;
            }
        }

        public IEnumerator<DObject> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return str.ToString();
        }

        public void SetString(string _str)
        {
            str = _str;
        }

        public void SetBool(bool _b)
        {
            b = _b;
        } 

        public bool TryGetValue(string val, out DObject o)
        {
            if(dict.ContainsKey(val))
            {
                o = dict[val];
                return true;
            }
            o = null;
            return false;
        }
    }

    public class Deserializer
    {
        static public DObject Deserialize(string json)
        {
            DObject ret = new DObject();

            if(json.Length == 0)
            {
                return null;
            }
            if (json[0] == '{')
            {
                if (json[json.Length - 1] != '}')
                {
                    throw new Exception("unexpected");
                }
                json = json.Substring(1, json.Length - 2);
                int inner = 0;
                int lastStart = 0;
                int lastMiddle = 0;
                bool apst = false;
                for (int i = 0; i <= json.Length; i++)
                {
                    if (i == json.Length || json[i] == ',')
                    {
                        if (inner == 0 && !apst)
                        {
                            DObject key = Deserialize(json.Substring(lastStart, lastMiddle - lastStart));
                            DObject value = Deserialize(json.Substring(lastMiddle + 1, i - lastMiddle - 1));
                            ret[key.ToString()] = value;
                            lastStart = i + 1;
                        }
                        if (i == json.Length)
                        {
                            break;
                        }
                    }
                    if (json[i] == ':')
                    {
                        if (inner == 0 && !apst)
                        {
                            lastMiddle = i;
                        }
                    }
                    if (json[i] == '"' && (i==0 || json[i - 1] != '\\'))
                    {
                        apst = !apst;
                    }
                    if (!apst)
                    {
                        if (json[i] == '{' || json[i] == '[')
                        {
                            inner++;
                        }
                        if (json[i] == '}' || json[i] == ']')
                        {
                            inner--;
                        }
                    }
                }
            }
            else if (json[0] == '[')
            {
                if (json[json.Length - 1] != ']')
                {
                    throw new Exception("unexpected");
                }
                json = json.Substring(1, json.Length - 2);
                int inner = 0;
                int lastStart = 0;
                bool apst = false;
                for(int i=0;i<=json.Length;i++)
                {
                    if(i == json.Length || json[i]==',')
                    {
                        if(inner == 0 && !apst)
                        {
                            DObject tmp = Deserialize(json.Substring(lastStart, i - lastStart));
                            if (tmp != null)
                            {
                                ret.Add(tmp);
                            }
                            lastStart = i + 1;
                        }
                        if(i==json.Length)
                        {
                            break;
                        }
                    }
                    if(json[i] == '"' && (i == 0 || json[i - 1] != '\\'))
                    {
                        apst = !apst;
                    }
                    if (!apst)
                    {
                        if (json[i] == '{' || json[i] == '[')
                        {
                            inner++;
                        }
                        if (json[i] == '}' || json[i] == ']')
                        {
                            inner--;
                        }
                    }
                }
            }
            else if (json[0] == '\"')
            {
                if(json[json.Length-1] != '\"')
                {
                    throw new Exception("unexpected");
                }
                json = json.Substring(1, json.Length - 2);
                ret.SetString(json.Replace("\\\"", "\""));
            }
            else if (json.CompareTo("null")==0)
            {
            }
            else if (json.CompareTo("true") == 0)
            {
                ret.SetBool(true);
            }
            else if (json.CompareTo("false") == 0)
            {
                ret.SetBool(false);
            }
            else
            {
                int tmp;
                if (int.TryParse(json, out tmp))
                {
                    ret.SetInt(tmp);
                }
                else
                {
                    throw new Exception("unexpected");
                }
            }

            return ret;
        }
    }
}
