using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RWDargatzUtility.Testing.Utility
{
    public static class FileRetrieval
    {
        public static string getDataFile(string filename)
        {
            return getFileString(Path.Combine("Files", filename));
        }

        public static string getFileString(string path)
        {
            path = Path.Combine(getProjectDirectory(), path);
            StreamReader reader = new StreamReader(path, true);
            string temp = reader.ReadToEnd();
            reader.Close();
            return temp;
        }

        public static string getProjectDirectory()
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                return Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName, "Testing");
            }
            else
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
            }
        }

    }
}
