using OpenTK;
using ShootyShootyBangBang.Networking.ClientServer.NetPackets;
using ShootyShootyBangBangEngine.Controllers;
using ShootyShootyBangBangEngine.GameObjects.Cameras;
using ShootyShootyBangBangEngine.Helpers;
using ShootyShootyBangBangEngine.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootyShootyBangBang.Networking.Client
{
    class ClientPacketHandler : PacketHandlerBase
    {
        ClientControllers m_clControllers;

        public void SetControllers(ClientControllers controllers)
        {
            m_clControllers = controllers;
        }

        public void Initialize(RPCDispatcher dispatcher)
        {
            dispatcher.Functions[typeof(HelloWorldPacket)] = OnPacketHelloWorld;
            dispatcher.Functions[typeof(SpawnPlayerServerPacket)] = OnSpawnPlayerServerPacket;
        }

        protected void OnPacketHelloWorld(RPCData data)
        {
            Console.WriteLine("Hello world (client)");
        }

        protected void OnSpawnPlayerServerPacket(RPCData data)
        {
            var packet = data.DeserializedObject as SpawnPlayerServerPacket;
            var playerCharacter = new GameObjects.Client.PlayerControlledCharacter(packet.id, m_clControllers, packet.position, new OpenTK.Vector2(32, 32), m_clControllers.GetTextureManager().GetOrCreateTexture("player", "Textures/Circle_blue.png"), m_clControllers.GetShaderManager().GetDefaultShader());
            m_clControllers.GetRootScene().AddGameObject(playerCharacter);

            var camera = new FollowCamera(playerCharacter.GetId());
            camera.SetExtends(new Vector2(-800, -600), new Vector2(800, 600));
            m_clControllers.GetRootScene().AddGameObject(camera);
            m_clControllers.SetCamera(camera);
        }
    }
}
