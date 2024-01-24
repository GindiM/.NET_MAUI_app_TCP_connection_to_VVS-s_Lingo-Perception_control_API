using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net;

namespace Wand
{
    internal class Methods
    {
        static internal bool IsValidIpv4Format(string ip)
        {
            string pattern = @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
            return Regex.IsMatch(ip, pattern);
        }

        static internal string RunStatusCheck(NetworkStream stream, string server, int interval) //run asynchronous
        {
            string runningPreset = "none";

            // make a msg queue in TcpReadWrite
            // each writing from different threads will go there
            // a thread is running in 30-tick interval and checking for queue's length
            // msgs will get dequeued, sent to stream and then wiped by their turn.
            return runningPreset;
        }


        static internal bool StatusReaderServer(string str) //status|server|success|playing|presetname
        {
            if (str == null || str.Length < 1) return false;
            if (str.Contains("playing"))
            {
                return true;
            }
            return false;
        }
        static internal string StatusReaderPreset(string str) //status|server|success|playing|presetName 
        {
            if (str == null || str.Length < 1) return null;
            if (str.Contains("playing"))
            {
                string presetName = str.Split('|')[4];
                return presetName;
            }
            return null;   //method returns preset's name unless it's stopped/unavailable/null/damaged string
        }

        //todo
        static internal Button CreateButton(EventHandler ev, string buttonText = default, LayoutOptions horizontalOptions = default, string backGroundColorAsHex = default, Thickness margin = default) // should return 
        {
            Button button = new Button();
            button.Text = buttonText;
            button.HorizontalOptions = horizontalOptions;
            button.Margin = margin;

            if (backGroundColorAsHex == default)
                button.BackgroundColor = default(Color);
            else button.BackgroundColor = Color.FromArgb(backGroundColorAsHex);

            button.Clicked += ev;

            return button;
        }
        static internal string[] ServerlistReader(string str)
        {
            string[] returningArray = new string[str.Length];
            return str.Split("");
        }
        static internal string[] PresetlistReader(string str)
        {
            string[] returningArray = new string[str.Length];
            return str.Split("");
        }

        public static string GetIPv4Address(string hostname)
        {
            IPAddress ipAddress = null;

            try
            {
                // Resolve the hostname to an IP address
                IPHostEntry hostEntry = Dns.GetHostEntry(hostname);
                ipAddress = hostEntry.AddressList.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

                if (ipAddress == null)
                {
                    // If no IPv4 address found, return null or throw an exception, depending on your use case
                    // return null;
                    throw new Exception("No IPv4 address found for the specified hostname.");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during hostname resolution
                // and return null or throw an exception, depending on your use case
                // return null;
                throw ex;
            }

            return ipAddress.ToString();
        }
    }

}

