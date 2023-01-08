using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wand
{
    internal static class DetailFile
    {
        const string defaultString0 = "|||false|false|";
        const string defaultString1 = "address|port|username|password|saveDetails|savePassword";
        const string defaultString2 = "192.168.1.12|10001|admin|1234|false|false";
        const string defaultString3 = "192.168.1.12|10001|admin|1234|true|true";
        internal static string path0 = FileSystem.Current.AppDataDirectory;
        internal static string fullPath = System.IO.Path.Combine(path0, "details.txt");

        static internal void CreateDefaultFile(string path)
        {
            File.WriteAllText(fullPath, defaultString0);
        }
        static internal void CreateFile(string path, string data)
        {
            File.WriteAllText(fullPath, data);
        }
        static internal string ReadFile(string path)
        {
            return File.ReadAllText(path);
        }
        static internal void WriteToFile(string path, string data)
        {
            File.WriteAllText(path, data);
        }





    }
}
