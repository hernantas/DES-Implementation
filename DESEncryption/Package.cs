using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DESEncryption
{
    public class Package
    {
        private Dictionary<string, string> header = new Dictionary<string,string>();
        private string content;

        public Package()
        {
        }

        public Package(string s)
        {
            this.SetByString(s);
        }

        public void SetHeader(string key, string value)
        {
            header[key] = value;
        }

        public void SetContent(string value)
        {
            content = value;
        }

        public string GetHeader(string key)
        {
            return header[key];
        }

        public string GetContent()
        {
            return content;
        }

        public void SetByString(string s)
        {
            string[] split = s.Split(';');

            foreach(string sp in split)
            {
                string[] keyval = sp.Split('=');

                if (keyval.Length == 2)
                {
                    string key = keyval[0];
                    string val = keyval[1];
                    header[key] = val;
                }
                else
                    content = keyval[0];
            }
        }

        public string GetString()
        {
            string build = "";

            foreach (KeyValuePair<string, string> kvp in header)
            {
                if (build == "")
                    build = kvp.Key + "=" + kvp.Value;
                else
                    build += ";" + kvp.Key + "=" + kvp.Value;
            }

            build += ";" + content;

            return build;
        }

        public void Print()
        {
            foreach (KeyValuePair<string, string> kvp in header)
            {
                Console.WriteLine(kvp.Key + "=" + kvp.Value);
            }

            Console.WriteLine("> "+content);
        }
    }
}
