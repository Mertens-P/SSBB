using ShootyShootyBangBang.ClientServer.NetPackets;
using ShootyShootyBangBangEngine.Helpers;
using ShootyShootyBangBangEngine.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootyShootyBangBang.Client
{
    class ClientPacketHandler : PacketHandlerBase
    {
        public void Initialize(RPCDispatcher dispatcher)
        {
            dispatcher.Functions[typeof(HelloWorldPacket)] = OnPacketHelloWorld;
        }

        protected void OnPacketHelloWorld(RPCData data)
        {
            Console.WriteLine("Hello world (client)");
        }
    }
}
