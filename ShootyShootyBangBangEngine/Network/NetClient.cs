using System;
using System.Net;

namespace ShootyShootyBangBangEngine.Network
{
    public class NetClient : NetClientServer
    {
        public long ServerConnectionId { get; private set; }
        public IPEndPoint ConnectedEndPoint { get; private set; }
        public bool CanSend { get; set; }

        public bool IsConnected { get { return ConnectedEndPoint != null; } }

        public NetClient()
        {
            CanSend = true;
            host = new ENet.Host();
            host.Initialize(null, 1);

            OnStatusChanged += Client_OnStatusChanged;
            OnData += Client_OnData;
        }

        void Client_OnData(object sender, NetWrapIncomingMessage e)
        {
            Console.WriteLine("Client: Data received; " + e.ToString());
        }

        void Client_OnStatusChanged(object sender, NetWrapConnectionStatus e)
        {
            switch (e.Status)
            {
                case NetWrapConnectionStatusEnum.Connected:
                    ServerConnectionId = e.ConnectionId;
                    ConnectedEndPoint = e.EndPoint;
                    break;
                case NetWrapConnectionStatusEnum.Disconnected:
                case NetWrapConnectionStatusEnum.TimedOut:
                    ServerConnectionId = -1;
                    ConnectedEndPoint = null;
                    break;
            }

            Console.WriteLine("Client: (" + e.EndPoint.ToString() + ") Status changed to " + e.Status.ToString());
        }

        public void SendRPC(object obj, NetWrapOrdering ordering = NetWrapOrdering.ReliableOrdered)
        {
            if (!CanSend)
                return;

            SendPacket(CreateRPCPacket(obj), ordering);
        }

        public void SendPacket(NetWrapOutgoingMessage msg, NetWrapOrdering ordering = NetWrapOrdering.ReliableOrdered)
        {
            if (!CanSend)
                return;

            SendPacket(msg, ServerConnectionId, ordering);
        }

        public void Connect(string ip, int port)
        {
            ConnectedEndPoint = null;
            peer = host.Connect(ip, port, 1234, 0);
        }
    }
}
