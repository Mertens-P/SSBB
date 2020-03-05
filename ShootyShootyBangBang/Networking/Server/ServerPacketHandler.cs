using ShootyShootyBangBang.Networking.ClientServer.NetPackets;
using ShootyShootyBangBangEngine.Controllers;
using ShootyShootyBangBangEngine.Helpers;
using ShootyShootyBangBangEngine.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootyShootyBangBang.Networking.Server
{
    class ServerPacketHandler : PacketHandlerServer
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

        public void OnConnect(object sender, long connectionId)
        {
            var character = new GameObjects.ClientServer.SSBBCharacter(new OpenTK.Vector2());
            m_svControllers.GetRootScene().AddGameObject(character);
            m_svControllers.Net.SendRPC(new SpawnPlayerServerPacket() { id = character.GetId(), position = new OpenTK.Vector2() }, connectionId);
        }

        protected void OnPacketHelloWorld(RPCData data)
        {
            Console.WriteLine("Hello world (server)");
            m_svControllers.Net.SendRPC(new HelloWorldPacket(), data.OriginalMessage.SenderConnectionId);
        }
    }
}
