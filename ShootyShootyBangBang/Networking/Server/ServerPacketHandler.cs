using ShootyShootyBangBang.Networking.ClientServer.NetPackets;
using ShootyShootyBangBangEngine.Controllers;
using ShootyShootyBangBangEngine.GameObjects.Components;
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

        Dictionary<long, Guid> m_netIdToCharacterId = new Dictionary<long, Guid>();

        public void SetControllers(ServerControllers controllers)
        {
            m_svControllers = controllers;
        }

        public void Initialize(RPCDispatcher dispatcher)
        {
            dispatcher.Functions[typeof(HelloWorldPacket)] = OnPacketHelloWorld;
            dispatcher.Functions[typeof(CharacterUpdateClientPacket)] = OnPacketCharacterUpdate;
        }

        public void OnConnect(object sender, long connectionId)
        {
            var character = new GameObjects.ClientServer.SSBBCharacter(new OpenTK.Vector2(), ComponentReplicator.PeerType.PT_Server);
            m_svControllers.GetRootScene().AddGameObject(character);
            m_netIdToCharacterId[connectionId] = character.GetId();
            m_svControllers.Net.SendRPC(new SpawnPlayerServerPacket() { id = character.GetId(), position = new OpenTK.Vector2() }, connectionId);
        }

        protected void OnPacketHelloWorld(RPCData data)
        {
            Console.WriteLine("Hello world (server)");
            m_svControllers.Net.SendRPC(new HelloWorldPacket(), data.OriginalMessage.SenderConnectionId);
        }

        protected void OnPacketCharacterUpdate(RPCData data)
        {
            var packet = (CharacterUpdateClientPacket)data.DeserializedObject;
            if (m_netIdToCharacterId.TryGetValue(data.OriginalMessage.SenderConnectionId, out var characterId))
            {
                var characterObj = m_svControllers.GetRootScene().GetGameObject(characterId);
                if (characterObj != null)
                {
                    var replicator = characterObj.GetComponents().GetComponent<ComponentReplicator>();
                    replicator.OnReplicate(m_svControllers, characterObj, packet.replicationData);
                }
                else
                    Console.WriteLine("Client has no character");
            }
            else
                Console.WriteLine("Update packet from unknown client!!");
        }
    }
}
