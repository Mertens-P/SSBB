using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ShootyShootyBangBangEngine.Network
{
    public class NetServer : NetClientServer
    {
        int m_maxConnections;
        bool m_enableUPnP;
        List<int> m_UPnPPorts = new List<int>();

        public int MaxConnections
        {
            get { return m_maxConnections; }
        }

        public NetServer(string upnpIdent, int cport, int maxConnections = 64, bool enableUPnP = true)
        {
            m_enableUPnP = enableUPnP;
            m_maxConnections = maxConnections;

            UPnPOpenPort(cport);

            host = new ENet.Host();
            host.InitializeServer(cport, maxConnections);
            listenPort = cport;

            OnData += Server_OnData;
            OnStatusChanged += Server_OnStatusChanged;
        }

        ~NetServer()
        {
            Console.WriteLine(DateTime.Now + " " + "Stopping server");
        }

        void Server_OnStatusChanged(object sender, NetWrapConnectionStatus e)
        {
            Console.WriteLine(DateTime.Now + " " + "Server: (" + e.EndPoint.ToString() + ") Status changed to " + e.Status.ToString());
        }

        void Server_OnData(object sender, NetWrapIncomingMessage e)
        {
            Console.WriteLine(DateTime.Now + " " + "Server: data received (" + GetConnectionEndPoint(e.SenderConnectionId) + ") " + e.ToString());
        }

        public void UPnPOpenPort(int cport)
        {
            if (!m_enableUPnP)
                return;
            
            var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
            string dir = System.IO.Path.GetDirectoryName(entryAssembly.Location);
            Task.Run(() => {
                try
                {
                    var startInfo = new ProcessStartInfo();
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.CreateNoWindow = true;
                    startInfo.FileName = System.IO.Path.Combine(dir, "IRHelper.exe");
                    startInfo.Arguments = "upnp-open " + cport.ToString();
                    Process.Start(startInfo);
                }
                catch (Exception)
                {
                    Console.WriteLine("Failed to start UPnP helper.");
                }
            });
            m_UPnPPorts.Add(cport);
        }

        public void UPnPCleanup()
        {
            if (!m_enableUPnP)
                return;

            string dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            foreach (var port in m_UPnPPorts)
            {
                Task.Run(() => {
                    int cport = port;
                    try
                    {
                        var startInfo = new ProcessStartInfo();
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        startInfo.CreateNoWindow = true;
                        startInfo.FileName = System.IO.Path.Combine(dir, "IRHelper.exe");
                        startInfo.Arguments = "upnp-close " + cport.ToString();
                        Process.Start(startInfo);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Failed to start UPnP helper.");
                    }
                });
            }

            m_UPnPPorts.Clear();
        }
    }
}
