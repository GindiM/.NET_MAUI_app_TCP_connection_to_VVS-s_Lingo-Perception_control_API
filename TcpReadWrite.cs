using System.Net.Sockets;
using System.Text;
using System.Net.NetworkInformation;
using System.Net;


namespace Wand
{
    public static class TcpReadWrite
    {
        public static string ReadFromStream(NetworkStream stream)
        {
            string bufferToString = "";

            // recieve
            if (stream.DataAvailable)
            {
                byte[] buffer = new byte[1024];
                int a = stream.Read(buffer, 0, buffer.Length);

                for (int i = 0; i < buffer.Length; i++) //char filter asciiWise
                {
                    if (buffer[i] < 32 || buffer[i] > 126)
                    {
                        buffer[i] = 0;
                    }
                }
                bufferToString = ((Encoding.UTF8.GetString(buffer)));

                if (bufferToString.Contains("\0"))
                    bufferToString = bufferToString.Replace("\0", "");
                //stream.Flush();
                return bufferToString;
            }
            return bufferToString;
        }

        public static void WriteToStream(NetworkStream stream, string message)
        {
            if (message != null && message.Length > 0)
            {
                byte[] data = System.Text.Encoding.ASCII.GetBytes(message + "|\r\n");
                stream.Write(data, 0, data.Length);
                //stream.Flush(); //warning! might flush incoming data
            }
        }



        //---other---

        public static bool IsLanOn()
        {

            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface nic in nics)
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    return true;
                }
            }
            return false;
        }


        public static bool IsAddressAvailable(string address)
        {
            Ping ping = new Ping();
            PingReply reply = ping.Send(address);

            if (reply.Status.ToString().Contains("Success"))
                return true;
            else return false;
        }

        public static bool isWifiOn()
        {
            ConnectionProfile cp = new ConnectionProfile();

            if (cp == ConnectionProfile.WiFi || cp == ConnectionProfile.Ethernet)
            {
                return true;
            }
            return false;
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