using ShootyShootyBangBang.ClientServer.NetPackets;
using ShootyShootyBangBangEngine.Controllers;
using ShootyShootyBangBangEngine.Helpers;
using ShootyShootyBangBangEngine.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootyShootyBangBang.Server
{
    class ServerPacketHandler : PacketHandlerBase
    {
        ServerControllers m_svControllers;

        public void SetControllers(ServerControllers controllers)
        {
            m_svControllers = controllers;
        }

        public void Initialize(RPCDispatcher dispatcher)
        {
            dispatcher.Functions[typeof(HelloWorldPacket)] = OnPacketHelloWorld;
        }

        protected void OnPacketHelloWorld(RPCData data)
        {
            Console.WriteLine("Hello world (server)");
            m_svControllers.Net.SendRPC(new HelloWorldPacket(), data.OriginalMessage.SenderConnectionId);
        }
    }
}
