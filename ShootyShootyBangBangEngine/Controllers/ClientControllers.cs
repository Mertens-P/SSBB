using ShootyShootyBangBangEngine.Network;
using ShootyShootyBangBangEngine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootyShootyBangBangEngine.Controllers
{
    public class ClientControllers : RenderControllers
    {
        NetClient m_netClient;
        PacketHandlerBase m_packetHandler;
        string m_ip;
        int m_port;

        public NetClient GetNetClient() { return m_netClient; }

        public ClientControllers(RenderPipelineBase renderPipeline, PacketHandlerBase packetHandler, string ip = "127.0.0.1", int port = 4805)
            : base(renderPipeline)
        {
            m_packetHandler = packetHandler;
            m_ip = ip;
            m_port = port;
        }

        public override void Init()
        {
            base.Init();
            m_netClient = new NetClient();
            m_packetHandler.Initialize(m_netClient.RpcDispatcher);
            m_netClient.Connect(m_ip, m_port);
            Console.WriteLine("Send connect package");

            Task.Run(() =>
            {
                while (!m_netClient.IsConnected)
                {
                    m_netClient.Update();
                    System.Threading.Thread.Sleep(100);
                }
                Console.WriteLine("Client connected to server");
                //while (true)
                //{
                //System.Threading.Thread.Sleep(1000);
                //try
                //{
                //    //var msg = CardGameCode.Events.MessageGenerator.GetHelloWorldPacket();
                //    //netClient.SendRPC(msg);
                //    //CardGameCode.Events.MessageGenerator.SendHelloWorldPacket(netClient);
                //}
                //catch (Exception e)
                //{
                //    UnityEngine.Debug.Log("Failed sending message: " + e);
                //}
                //}

                //// invoke to main thread using coroutines
                //lock (Game.Configuration.EngineProvider.Engine.coroutines)
                //{
                //    Game.Configuration.EngineProvider.Engine.coroutines.StartCoroutine(Game.Framework.Coroutines.RepeatWhileTrue(() => {
                //        var client = new GameClient(netClient, "Single-player");
                //        GameState.StackPush(client);
                //        return false;
                //    }));
                //}

            });
        }

        public override void OnUpdate(double dt)
        {
            base.OnUpdate(dt);
            m_netClient.Update();
        }
    }
}
