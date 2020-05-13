using ShootyShootyBangBang.Networking.ClientServer.NetPackets;
using ShootyShootyBangBangEngine.Controllers;
using ShootyShootyBangBangEngine.GameObjects.Cameras;
using ShootyShootyBangBangEngine.GameObjects.Components;
using ShootyShootyBangBangEngine.Helpers;
using ShootyShootyBangBangEngine.Network;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootyShootyBangBang.Networking.Client
{
    class ClientPacketHandler : PacketHandlerBase
    {
        Implementations.SSBBClientControllers m_clControllers;

        public void SetControllers(Implementations.SSBBClientControllers controllers)
        {
            m_clControllers = controllers;
        }

        public void Initialize(RPCDispatcher dispatcher)
        {
            dispatcher.Functions[typeof(HelloWorldPacket)] = OnPacketHelloWorld;
            dispatcher.Functions[typeof(SpawnPlayerServerPacket)] = OnSpawnPlayerServerPacket;
            dispatcher.Functions[typeof(ServerUpdatePacket)] = OnServerUpdatePacket;
        }

        protected void OnPacketHelloWorld(RPCData data)
        {
            Console.WriteLine("Hello world (client)");
        }

        protected void OnSpawnPlayerServerPacket(RPCData data)
        {
            var packet = data.DeserializedObject as SpawnPlayerServerPacket;
            GameObjects.Client.ClientCharacter playerCharacter = new GameObjects.Client.PlayerControlledCharacter(packet.id, m_clControllers.GetSSBBRenderPipeline(), MathHelpers.OpenTkVecToSystemVec(packet.position), new Vector2(32, 32), m_clControllers.GetTextureManager().GetOrCreateTexture("arrow", "Textures/Arrow.png"), m_clControllers.GetShaderManager().GetDefaultShader());
            if (packet.aiType != GameObjects.Client.Ai.AiFactory.AiType.AT_None)
            {
                var aiComp = new ComponentAiSystem();
                aiComp.AddState(GameObjects.Client.Ai.AiFactory.CreateBaseAiState(packet.aiType, playerCharacter.GetMovementSpeed()));
                playerCharacter.GetComponents().AddComponent(aiComp);
            }

            m_clControllers.GetRootScene().AddGameObject(playerCharacter);

            var camera = new FollowCamera(playerCharacter.GetId());
            camera.SetExtends(new Vector2(-800, -600), new Vector2(800, 600));
            m_clControllers.GetRootScene().AddGameObject(camera);
            m_clControllers.SetCamera(camera);
        }

        protected void OnServerUpdatePacket(RPCData data)
        {
            var packet = data.DeserializedObject as ServerUpdatePacket;
            foreach(var repData in packet.ReplicationData)
            {
                var repObj = m_clControllers.GetRootScene().GetGameObject(repData.CharacterId);
                if (repObj == null)
                {
                    repObj = new GameObjects.Client.ClientCharacter(repData.CharacterId, m_clControllers.GetSSBBRenderPipeline(), new Vector2(), new Vector2(32, 32), m_clControllers.GetTextureManager().GetOrCreateTexture("arrow", "Textures/Arrow.png"), m_clControllers.GetShaderManager().GetDefaultShader());
                    m_clControllers.GetRootScene().AddGameObject(repObj);
                }
                var replicator = repObj.GetComponents().GetComponent<ComponentReplicator>();
                replicator.OnReplicate(m_clControllers, repObj, repData);
            }
        }
    }
}
